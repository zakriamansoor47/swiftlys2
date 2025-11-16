using System.Reflection;
using System.Runtime.Loader;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Core.Services;
using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Core.Modules.Plugins;

namespace SwiftlyS2.Core.Plugins;

internal class PluginManager
{
    private IServiceProvider _Provider { get; init; }
    private RootDirService _RootDirService { get; init; }
    private ILogger _Logger { get; init; }
    private List<PluginContext> _Plugins { get; } = new();
    private FileSystemWatcher? _Watcher { get; set; }
    private InterfaceManager _InterfaceManager { get; set; } = new();
    private List<Type> _SharedTypes { get; set; } = new();
    private DataDirectoryService _DataDirectoryService { get; init; }
    private DateTime lastRead = DateTime.MinValue;

    public PluginManager(
        IServiceProvider provider,
        ILogger<PluginManager> logger,
        RootDirService rootDirService,
        DataDirectoryService dataDirectoryService
    )
    {
        _Provider = provider;
        _RootDirService = rootDirService;
        _Logger = logger;
        _DataDirectoryService = dataDirectoryService;
        _Watcher = new FileSystemWatcher {
            Path = rootDirService.GetPluginsRoot(),
            Filter = "*.dll",
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite,
        };

        _Watcher.Changed += HandlePluginChange;
        _Watcher.EnableRaisingEvents = true;

        Initialize();
    }

    public void Initialize()
    {
        AppDomain.CurrentDomain.AssemblyResolve += ( sender, e ) =>
        {
            var loadingAssemblyName = new AssemblyName(e.Name).Name ?? string.Empty;
            return loadingAssemblyName.Equals("SwiftlyS2.CS2", StringComparison.OrdinalIgnoreCase)
                ? Assembly.GetExecutingAssembly()
                : AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => loadingAssemblyName == a.GetName().Name);
        };
        LoadExports();
        LoadPlugins();
    }

    public void HandlePluginChange( object sender, FileSystemEventArgs e )
    {
        try
        {
            if (!NativeServerHelpers.UseAutoHotReload())
            {
                return;
            }

            // Windows FileSystemWatcher triggers multiple (open, write, close) events for a single file change
            if (DateTime.Now - lastRead < TimeSpan.FromSeconds(1))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(e.FullPath)))
            {
                return;
            }

            foreach (var plugin in _Plugins)
            {
                if (Path.GetFileName(plugin?.PluginDirectory) == Path.GetFileName(Path.GetDirectoryName(e.FullPath)))
                {
                    var pluginId = plugin?.Metadata?.Id;
                    if (!string.IsNullOrWhiteSpace(pluginId))
                    {
                        lastRead = DateTime.Now;
                        ReloadPlugin(pluginId);
                    }

                    break;
                }
            }
        }
        catch (Exception ex)
        {
            if (!GlobalExceptionHandler.Handle(ex))
            {
                return;
            }
            _Logger.LogError(ex, "Error handling plugin change");
        }
    }

    private void PopulateSharedManually( string startDirectory )
    {
        var pluginDirs = Directory.GetDirectories(startDirectory);

        foreach (var pluginDir in pluginDirs)
        {
            var dirName = Path.GetFileName(pluginDir);
            if (dirName.StartsWith('[') && dirName.EndsWith(']'))
            {
                PopulateSharedManually(pluginDir);
            }
            else
            {
                if (Directory.Exists(Path.Combine(pluginDir, "resources", "exports")))
                {
                    var exportFiles = Directory.GetFiles(Path.Combine(pluginDir, "resources", "exports"), "*.dll");
                    foreach (var exportFile in exportFiles)
                    {
                        try
                        {
                            var assembly = Assembly.LoadFrom(exportFile);
                            var exports = assembly.GetTypes();
                            foreach (var export in exports)
                            {
                                _SharedTypes.Add(export);
                            }
                        }
                        catch (Exception innerEx)
                        {
                            if (!GlobalExceptionHandler.Handle(innerEx))
                            {
                                continue;
                            }
                            _Logger.LogWarning(innerEx, "Failed to load export assembly: {Path}", exportFile);
                        }
                    }
                }
            }
        }
    }

    private void LoadExports()
    {
        var resolver = new DependencyResolver(_Logger);

        try
        {
            resolver.AnalyzeDependencies(_RootDirService.GetPluginsRoot());
            _Logger.LogInformation("{Graph}", resolver.GetDependencyGraphVisualization());
            var loadOrder = resolver.GetLoadOrder();

            _Logger.LogInformation("Loading {Count} export assemblies in dependency order.", loadOrder.Count);

            foreach (var exportFile in loadOrder)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(exportFile);
                    var exports = assembly.GetTypes();
                    _Logger.LogDebug("Loaded {Count} types from {Path}", exports.Length, Path.GetFileName(exportFile));

                    foreach (var export in exports)
                    {
                        _SharedTypes.Add(export);
                    }
                }
                catch (Exception ex)
                {
                    if (!GlobalExceptionHandler.Handle(ex))
                    {
                        continue;
                    }
                    _Logger.LogWarning(ex, "Failed to load export assembly: {Path}", exportFile);
                }
            }

            _Logger.LogInformation("Successfully loaded {Count} shared types.", _SharedTypes.Count);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Circular dependency"))
        {
            _Logger.LogError(ex, "Circular dependency detected in plugin exports. Loading exports without dependency resolution.");
            PopulateSharedManually(_RootDirService.GetPluginsRoot());
        }
        catch (Exception ex)
        {
            if (!GlobalExceptionHandler.Handle(ex)) return;
            _Logger.LogError(ex, "Unexpected error during export loading");
        }
    }

    private void LoadPluginsFromFolder( string directory )
    {
        var pluginDirs = Directory.GetDirectories(directory);

        foreach (var pluginDir in pluginDirs)
        {
            var dirName = Path.GetFileName(pluginDir);
            if (dirName.StartsWith('[') && dirName.EndsWith(']'))
            {
                LoadPluginsFromFolder(pluginDir);
            }
            else
            {
                try
                {
                    var context = LoadPlugin(pluginDir, false);
                    if (context != null && context.Status == PluginStatus.Loaded)
                    {
                        _Logger.LogInformation("Loaded plugin {Id}", context.Metadata!.Id);
                    }
                }
                catch (Exception e)
                {
                    if (!GlobalExceptionHandler.Handle(e))
                    {
                        continue;
                    }
                    _Logger.LogWarning(e, "Error loading plugin: {Path}", pluginDir);
                    continue;
                }
            }
        }
    }

    private void LoadPlugins()
    {
        LoadPluginsFromFolder(_RootDirService.GetPluginsRoot());
        RebuildSharedServices();

        _Plugins
            .Where(p => p.Status == PluginStatus.Loaded)
            .ToList()
            .ForEach(p => p.Plugin!.OnAllPluginsLoaded());
    }

    public List<PluginContext> GetPlugins()
    {
        return _Plugins;
    }

    private void RebuildSharedServices()
    {
        _InterfaceManager.Dispose();

        var loadedPlugins = _Plugins
            .Where(p => p.Status == PluginStatus.Loaded)
            .ToList();

        loadedPlugins.ForEach(p => p.Plugin?.ConfigureSharedInterface(_InterfaceManager));
        _InterfaceManager.Build();

        loadedPlugins.ForEach(p => p.Plugin?.UseSharedInterface(_InterfaceManager));
        loadedPlugins.ForEach(p => p.Plugin?.OnSharedInterfaceInjected(_InterfaceManager));
    }

    public PluginContext? LoadPlugin( string dir, bool hotReload )
    {
        PluginContext context = new() {
            PluginDirectory = dir,
            Status = PluginStatus.Loading,
        };
        _Plugins.Add(context);

        var entrypointDll = Path.Combine(dir, Path.GetFileName(dir) + ".dll");

        if (!File.Exists(entrypointDll))
        {
            _Logger.LogWarning("Plugin entrypoint DLL not found: {Path}", entrypointDll);
            context.Status = PluginStatus.Error;
            return null;
        }

        var loader = PluginLoader.CreateFromAssemblyFile(
            assemblyFile: entrypointDll,
            sharedTypes: [typeof(BasePlugin), .. _SharedTypes],
            config =>
            {
                config.IsUnloadable = true;
                config.LoadInMemory = true;
                var currentContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
                if (currentContext != null)
                {
                    config.DefaultContext = currentContext;
                    config.PreferSharedTypes = true;
                }
            }
        );

        var assembly = loader.LoadDefaultAssembly();
        var pluginType = assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(BasePlugin)));
        if (pluginType == null)
        {
            _Logger.LogWarning("Plugin type not found: {Path}", entrypointDll);
            context.Status = PluginStatus.Error;
            return null;
        }

        var metadata = pluginType.GetCustomAttribute<PluginMetadata>();
        if (metadata == null)
        {
            _Logger.LogWarning("Plugin metadata not found: {Path}", entrypointDll);
            context.Status = PluginStatus.Error;
            return null;
        }

        context.Metadata = metadata;
        _DataDirectoryService.EnsurePluginDataDirectory(metadata.Id);

        var core = new SwiftlyCore(metadata.Id, Path.GetDirectoryName(entrypointDll)!, metadata, pluginType, _Provider, _DataDirectoryService.GetPluginDataDirectory(metadata.Id));
        core.InitializeType(pluginType);
        var plugin = (BasePlugin)Activator.CreateInstance(pluginType, [core])!;
        core.InitializeObject(plugin);

        try
        {
            plugin.Load(hotReload);
        }
        catch (Exception e)
        {
            if (!GlobalExceptionHandler.Handle(e))
            {
                context.Status = PluginStatus.Error;
                return null;
            }
            _Logger.LogWarning(e, "Error loading plugin {Path}", entrypointDll);
            try
            {
                plugin.Unload();
                loader?.Dispose();
                core?.Dispose();
            }
            catch (Exception)
            {
            }
            context.Status = PluginStatus.Error;
            return null;
        }

        context.Status = PluginStatus.Loaded;
        context.Core = core;
        context.Plugin = plugin;
        context.Loader = loader;
        return context;
    }

    public bool UnloadPlugin( string id )
    {
        var context = _Plugins
            .Where(p => p.Status == PluginStatus.Loaded)
            .FirstOrDefault(p => p.Metadata?.Id == id);

        if (context == null)
        {
            _Logger.LogWarning("Plugin not found or not loaded: {Id}", id);
            return false;
        }

        context.Dispose();
        context.Status = PluginStatus.Unloaded;
        return true;
    }

    public bool LoadPluginById( string id )
    {
        var context = _Plugins
            .Where(p => p.Status == PluginStatus.Unloaded)
            .FirstOrDefault(p => p.Metadata?.Id == id);

        var result = false;
        if (context != null)
        {
            var directory = context.PluginDirectory!;
            _ = _Plugins.Remove(context);
            _ = LoadPlugin(directory, true);
            result = true;
        }

        RebuildSharedServices();
        return result;
    }

    public void ReloadPlugin( string id )
    {
        _Logger.LogInformation("Reloading plugin {Id}", id);

        if (!UnloadPlugin(id))
        {
            _Logger.LogWarning("Plugin not found or not loaded: {Id}", id);
            return;
        }

        if (!LoadPluginById(id))
        {
            _Logger.LogWarning("Failed to load plugin {Id}", id);
            return;
        }

        _Logger.LogInformation("Reloaded plugin {Id}", id);
    }
}