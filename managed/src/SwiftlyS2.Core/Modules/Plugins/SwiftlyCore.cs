using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Core.Commands;
using SwiftlyS2.Core.ConsoleOutput;
using SwiftlyS2.Core.Events;
using SwiftlyS2.Core.GameEvents;
using SwiftlyS2.Core.Misc;
using SwiftlyS2.Core.NetMessages;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.GameEvents;
using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.ConsoleOutput;
using SwiftlyS2.Shared.NetMessages;
using SwiftlyS2.Shared.Services;
using SwiftlyS2.Core.AttributeParsers;
using SwiftlyS2.Core.EntitySystem;
using SwiftlyS2.Shared.EntitySystem;
using SwiftlyS2.Core.Convars;
using SwiftlyS2.Shared.Convars;
using SwiftlyS2.Core.Hooks;
using SwiftlyS2.Shared.Profiler;
using SwiftlyS2.Core.Profiler;
using SwiftlyS2.Shared.Memory;
using SwiftlyS2.Core.Memory;
using SwiftlyS2.Shared.Scheduler;
using SwiftlyS2.Core.Scheduler;
using SwiftlyS2.Core.Database;
using SwiftlyS2.Shared.Database;
using SwiftlyS2.Core.Translations;
using SwiftlyS2.Core.Permissions;
using SwiftlyS2.Shared.Permissions;
using SwiftlyS2.Core.Menus;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.Translation;
using SwiftlyS2.Core.Players;
using SwiftlyS2.Shared.CommandLine;
using SwiftlyS2.Core.CommandLine;

namespace SwiftlyS2.Core.Services;


internal class SwiftlyCore : ISwiftlyCore, IDisposable
{

  private ServiceProvider _ServiceProvider { get; init; }

  public EventSubscriber EventSubscriber { get; init; }
  public GameEventService GameEventService { get; init; }
  public NetMessageService NetMessageService { get; init; }
  public PluginConfigurationService Configuration { get; init; }
  public ILoggerFactory LoggerFactory { get; init; }
  public CommandService CommandService { get; init; }
  public ConsoleOutputService ConsoleOutputService { get; init; }
  public EntitySystemService EntitySystemService { get; init; }
  public ConVarService ConVarService { get; init; }
  public GameDataService GameDataService { get; init; }
  public PlayerManagerService PlayerManagerService { get; init; }
  public ILogger Logger { get; init; }
  public EngineService Engine { get; init; }
  public TraceManager Trace { get; init; }
  public ContextedProfilerService ProfilerService { get; init; }
  public MemoryService MemoryService { get; init; }
  public SchedulerService SchedulerService { get; init; }
  public DatabaseService DatabaseService { get; init; }
  public TranslationService TranslationService { get; init; }
  public Localizer Localizer { get; init; }
  public PermissionManager PermissionManager { get; init; }
  public RegistratorService RegistratorService { get; init; }
  public MenuManager MenuManager { get; init; }
  public CommandLineService CommandLineService { get; init; }
  public string ContextBasePath { get; init; }
  public SwiftlyCore(string contextId, string contextBaseDirectory, PluginMetadata? pluginManifest, Type contextType, IServiceProvider coreProvider)
  {

    CoreContext id = new(contextId, contextBaseDirectory, pluginManifest);
    ContextBasePath = contextBaseDirectory;

    ServiceCollection services = new();

    services
      .AddSingleton(id)
      .AddSingleton(this)
      .AddSingleton<ISwiftlyCore>(this)
      .AddSingleton(coreProvider.GetRequiredService<ProfileService>())
      .AddSingleton(coreProvider.GetRequiredService<ConfigurationService>())
      .AddSingleton(coreProvider.GetRequiredService<HookManager>())
      .AddSingleton(coreProvider.GetRequiredService<PlayerManagerService>())
      .AddSingleton(coreProvider.GetRequiredService<TraceManager>())
      .AddSingleton(coreProvider.GetRequiredService<PermissionManager>())
      .AddSingleton(coreProvider.GetRequiredService<CommandTrackerManager>())

      .AddSingleton<EventSubscriber>()
      .AddSingleton<EngineService>()
      .AddSingleton<PluginConfigurationService>()
      .AddSingleton<GameEventService>()
      .AddSingleton<NetMessageService>()
      .AddSingleton<CommandService>()
      .AddSingleton<ConsoleOutputService>()
      .AddSingleton<EntitySystemService>()
      .AddSingleton<ConVarService>()
      .AddSingleton<MemoryService>()
      .AddSingleton<GameDataService>()
      .AddSingleton<PlayerManagerService>()
      .AddSingleton<ContextedProfilerService>()
      .AddSingleton<SchedulerService>()
      .AddSingleton<DatabaseService>()
      .AddSingleton<TranslationService>()
      .AddSingleton<Localizer>(provider => provider.GetRequiredService<TranslationService>().GetLocalizer())
      .AddSingleton<RegistratorService>()
      .AddSingleton<MenuManager>()
      .AddSingleton<CommandLineService>()
      .AddSingleton<IPermissionManager>(provider => provider.GetRequiredService<PermissionManager>())

      .AddSingleton<IEventSubscriber>(provider => provider.GetRequiredService<EventSubscriber>())
      .AddSingleton<IGameEventService>(provider => provider.GetRequiredService<GameEventService>())
      .AddSingleton<INetMessageService>(provider => provider.GetRequiredService<NetMessageService>())
      .AddSingleton<IPluginConfigurationService>(provider => provider.GetRequiredService<PluginConfigurationService>())
      .AddSingleton<ICommandService>(provider => provider.GetRequiredService<CommandService>())
      .AddSingleton<IConsoleOutputService>(provider => provider.GetRequiredService<ConsoleOutputService>())
      .AddSingleton<IEntitySystemService>(provider => provider.GetRequiredService<EntitySystemService>())
      .AddSingleton<IConVarService>(provider => provider.GetRequiredService<ConVarService>())
      .AddSingleton<IGameDataService>(provider => provider.GetRequiredService<GameDataService>())
      .AddSingleton<IPlayerManagerService>(provider => provider.GetRequiredService<PlayerManagerService>())
      .AddSingleton<IMemoryService>(provider => provider.GetRequiredService<MemoryService>())
      .AddSingleton<IContextedProfilerService>(provider => provider.GetRequiredService<ContextedProfilerService>())
      .AddSingleton<ISchedulerService>(provider => provider.GetRequiredService<SchedulerService>())
      .AddSingleton<IEngineService>(provider => provider.GetRequiredService<EngineService>())
      .AddSingleton<ITraceManager>(provider => provider.GetRequiredService<TraceManager>())
      .AddSingleton<IDatabaseService>(provider => provider.GetRequiredService<DatabaseService>())
      .AddSingleton<ITranslationService>(provider => provider.GetRequiredService<TranslationService>())
      .AddSingleton<ILocalizer>(provider => provider.GetRequiredService<TranslationService>().GetLocalizer())
      .AddSingleton<IRegistratorService>(provider => provider.GetRequiredService<RegistratorService>())
      .AddSingleton<IMenuManager>(provider => provider.GetRequiredService<MenuManager>())
      .AddSingleton<ICommandLine>(provider => provider.GetRequiredService<CommandLineService>())

      .AddLogging(
        builder =>
        {
          builder.AddProvider(new SwiftlyLoggerProvider(id.Name));
        }
      );


    _ServiceProvider = services.BuildServiceProvider();

    EventSubscriber = _ServiceProvider.GetRequiredService<EventSubscriber>();
    Configuration = _ServiceProvider.GetRequiredService<PluginConfigurationService>();
    GameEventService = _ServiceProvider.GetRequiredService<GameEventService>();
    LoggerFactory = _ServiceProvider.GetRequiredService<ILoggerFactory>();
    NetMessageService = _ServiceProvider.GetRequiredService<NetMessageService>();
    CommandService = _ServiceProvider.GetRequiredService<CommandService>();
    ConsoleOutputService = _ServiceProvider.GetRequiredService<ConsoleOutputService>();
    EntitySystemService = _ServiceProvider.GetRequiredService<EntitySystemService>();
    GameDataService = _ServiceProvider.GetRequiredService<GameDataService>();
    PlayerManagerService = _ServiceProvider.GetRequiredService<PlayerManagerService>();
    ConVarService = _ServiceProvider.GetRequiredService<ConVarService>();
    MemoryService = _ServiceProvider.GetRequiredService<MemoryService>();
    Engine = _ServiceProvider.GetRequiredService<EngineService>();
    Trace = _ServiceProvider.GetRequiredService<TraceManager>();
    ProfilerService = _ServiceProvider.GetRequiredService<ContextedProfilerService>();
    SchedulerService = _ServiceProvider.GetRequiredService<SchedulerService>();
    DatabaseService = _ServiceProvider.GetRequiredService<DatabaseService>();
    TranslationService = _ServiceProvider.GetRequiredService<TranslationService>();
    Localizer = _ServiceProvider.GetRequiredService<Localizer>();
    PermissionManager = _ServiceProvider.GetRequiredService<PermissionManager>();
    RegistratorService = _ServiceProvider.GetRequiredService<RegistratorService>();
    MenuManager = _ServiceProvider.GetRequiredService<MenuManager>();
    CommandLineService = _ServiceProvider.GetRequiredService<CommandLineService>();
    Logger = LoggerFactory.CreateLogger(contextType);
  }

  public void InitializeType(Type type)
  {
    this.Parse(type);
  }

  public void InitializeObject(object instance)
  {
    RegistratorService.Register(instance);
  }

  public void Dispose()
  {
    _ServiceProvider.Dispose();
  }

  IEventSubscriber ISwiftlyCore.Event => EventSubscriber;
  IPluginConfigurationService ISwiftlyCore.Configuration => Configuration;
  ILoggerFactory ISwiftlyCore.LoggerFactory => LoggerFactory;
  IGameEventService ISwiftlyCore.GameEvent => GameEventService;
  INetMessageService ISwiftlyCore.NetMessage => NetMessageService;
  ICommandService ISwiftlyCore.Command => CommandService;
  IConsoleOutputService ISwiftlyCore.ConsoleOutput => ConsoleOutputService;
  IEntitySystemService ISwiftlyCore.EntitySystem => EntitySystemService;
  IConVarService ISwiftlyCore.ConVar => ConVarService;
  IGameDataService ISwiftlyCore.GameData => GameDataService;
  IPlayerManagerService ISwiftlyCore.PlayerManager => PlayerManagerService;
  IMemoryService ISwiftlyCore.Memory => MemoryService;
  ILogger ISwiftlyCore.Logger => Logger;
  IContextedProfilerService ISwiftlyCore.Profiler => ProfilerService;
  IEngineService ISwiftlyCore.Engine => Engine;
  ITraceManager ISwiftlyCore.Trace => Trace;
  ISchedulerService ISwiftlyCore.Scheduler => SchedulerService;
  IDatabaseService ISwiftlyCore.Database => DatabaseService;
  ITranslationService ISwiftlyCore.Translation => TranslationService;
  ILocalizer ISwiftlyCore.Localizer => Localizer;
  IPermissionManager ISwiftlyCore.Permission => PermissionManager;
  IRegistratorService ISwiftlyCore.Registrator => RegistratorService;
  IMenuManager ISwiftlyCore.Menus => MenuManager;
  string ISwiftlyCore.PluginPath => ContextBasePath;
  ICommandLine ISwiftlyCore.CommandLine => CommandLineService;
}