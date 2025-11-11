using System.Globalization;
using System.Collections.Concurrent;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Sounds;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Core.Menus;

internal sealed class MenuManagerAPI : IMenuManagerAPI
{
    /// <summary>
    /// The SwiftlyS2 core instance.
    /// </summary>
    public ISwiftlyCore Core { get; init; }

    /// <summary>
    /// Global Configuration settings for all menus.
    /// </summary>
    public MenuManagerConfiguration Configuration { get; init; }

    /// <summary>
    /// Fired when a menu is closed for a player.
    /// </summary>
    public event EventHandler<MenuManagerEventArgs>? MenuClosed;

    /// <summary>
    /// Fired when a menu is opened for a player.
    /// </summary>
    public event EventHandler<MenuManagerEventArgs>? MenuOpened;

    private readonly ConcurrentDictionary<IPlayer, IMenuAPI> openMenus = new();
    private readonly SoundEvent useSound = new();
    private readonly SoundEvent exitSound = new();
    private readonly SoundEvent scrollSound = new();
    private readonly KeyBind buttonsScroll;
    private readonly KeyBind buttonsScrollBack;
    private readonly KeyBind buttonsExit;
    private readonly KeyBind buttonsUse;

    private static readonly Dictionary<string, KeyBind> StringToKeyBind = new() {
        ["mouse1"] = KeyBind.Mouse1,
        ["mouse2"] = KeyBind.Mouse2,
        ["space"] = KeyBind.Space,
        ["ctrl"] = KeyBind.Ctrl,
        ["w"] = KeyBind.W,
        ["a"] = KeyBind.A,
        ["s"] = KeyBind.S,
        ["d"] = KeyBind.D,
        ["e"] = KeyBind.E,
        ["esc"] = KeyBind.Esc,
        ["r"] = KeyBind.R,
        ["alt"] = KeyBind.Alt,
        ["shift"] = KeyBind.Shift,
        ["weapon1"] = KeyBind.Weapon1,
        ["weapon2"] = KeyBind.Weapon2,
        ["grenade1"] = KeyBind.Grenade1,
        ["grenade2"] = KeyBind.Grenade2,
        ["tab"] = KeyBind.Tab,
        ["f"] = KeyBind.F,
    };

    public MenuManagerAPI( ISwiftlyCore core )
    {
        Core = core;

        var settings = NativeEngineHelpers.GetMenuSettings().Trim().Split('\x01');
        Configuration = new MenuManagerConfiguration {
            NavigationPrefix = settings[0],
            InputMode = settings[1],
            ButtonsUse = settings[2],
            ButtonsScroll = settings[3],
            ButtonsScrollBack = settings[4],
            ButtonsExit = settings[5],
            SoundUseName = settings[6],
            SoundUseVolume = float.Parse(settings[7], CultureInfo.InvariantCulture),
            SoundScrollName = settings[8],
            SoundScrollVolume = float.Parse(settings[9], CultureInfo.InvariantCulture),
            SoundExitName = settings[10],
            SoundExitVolume = float.Parse(settings[11], CultureInfo.InvariantCulture),
            ItemsPerPage = int.Parse(settings[12]),
        };

        scrollSound.Name = Configuration.SoundScrollName;
        scrollSound.Volume = Configuration.SoundScrollVolume;
        useSound.Name = Configuration.SoundUseName;
        useSound.Volume = Configuration.SoundUseVolume;
        exitSound.Name = Configuration.SoundExitName;
        exitSound.Volume = Configuration.SoundExitVolume;
        buttonsScroll = StringToKeyBind.GetValueOrDefault(Configuration.ButtonsScroll.Trim().ToLower());
        buttonsScrollBack = StringToKeyBind.GetValueOrDefault(Configuration.ButtonsScrollBack.Trim().ToLower());
        buttonsExit = StringToKeyBind.GetValueOrDefault(Configuration.ButtonsExit.Trim().ToLower());
        buttonsUse = StringToKeyBind.GetValueOrDefault(Configuration.ButtonsUse.Trim().ToLower());

        openMenus.Clear();

        Core.Event.OnClientKeyStateChanged += KeyStateChange;
        Core.Event.OnClientDisconnected += OnClientDisconnected;
        Core.Event.OnMapUnload += OnMapUnload;
    }

    ~MenuManagerAPI()
    {
        CloseAllMenus();

        Core.Event.OnClientKeyStateChanged -= KeyStateChange;
        Core.Event.OnClientDisconnected -= OnClientDisconnected;
        Core.Event.OnMapUnload -= OnMapUnload;
    }

    private void KeyStateChange( IOnClientKeyStateChangedEvent @event )
    {
        if (openMenus.IsEmpty)
        {
            return;
        }

        var player = Core.PlayerManager.GetPlayer(@event.PlayerId);
        var menu = GetCurrentMenu(player);

        if (menu == null || !player.IsValid || player.IsFakeClient || !@event.Pressed)
        {
            return;
        }

        if (Configuration.InputMode.Trim().Equals("button", StringComparison.CurrentCultureIgnoreCase))
        {
            var scrollKey = menu.KeybindOverrides.Move ?? buttonsScroll;
            var scrollBackKey = menu.KeybindOverrides.MoveBack ?? buttonsScrollBack;
            var exitKey = menu.KeybindOverrides.Exit ?? buttonsExit;
            var useKey = menu.KeybindOverrides.Select ?? buttonsUse;

            if (scrollKey.HasFlag(@event.Key.ToKeyBind()))
            {
                _ = menu.MoveToOptionIndex(player, menu.GetCurrentOptionIndex(player) + 1);

                if (menu.Configuration.PlaySound)
                {
                    scrollSound.Recipients.AddRecipient(@event.PlayerId);
                    scrollSound.Emit();
                    scrollSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (scrollBackKey.HasFlag(@event.Key.ToKeyBind()))
            {
                _ = menu.MoveToOptionIndex(player, menu.GetCurrentOptionIndex(player) - 1);

                if (menu.Configuration.PlaySound)
                {
                    scrollSound.Recipients.AddRecipient(@event.PlayerId);
                    scrollSound.Emit();
                    scrollSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (exitKey.HasFlag(@event.Key.ToKeyBind()))
            {
                CloseMenuForPlayer(player, menu);

                if (menu.Configuration.PlaySound)
                {
                    exitSound.Recipients.AddRecipient(@event.PlayerId);
                    exitSound.Emit();
                    exitSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (useKey.HasFlag(@event.Key.ToKeyBind()))
            {
                var option = menu.GetCurrentOption(player);
                if (option != null && option.Enabled && option.GetEnabled(player))
                {
                    _ = Task.Run(async () => await option.OnClickAsync(player));

                    if (menu.Configuration.PlaySound && option.PlaySound)
                    {
                        useSound.Recipients.AddRecipient(@event.PlayerId);
                        useSound.Emit();
                        useSound.Recipients.RemoveRecipient(@event.PlayerId);
                    }
                }
            }
        }
        else if (Configuration.InputMode.Trim().Equals("wasd", StringComparison.CurrentCultureIgnoreCase))
        {
            if (KeyBind.W.HasFlag(@event.Key.ToKeyBind()))
            {
                _ = menu.MoveToOptionIndex(player, menu.GetCurrentOptionIndex(player) - 1);

                if (menu.Configuration.PlaySound)
                {
                    scrollSound.Recipients.AddRecipient(@event.PlayerId);
                    scrollSound.Emit();
                    scrollSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (KeyBind.S.HasFlag(@event.Key.ToKeyBind()))
            {
                _ = menu.MoveToOptionIndex(player, menu.GetCurrentOptionIndex(player) + 1);

                if (menu.Configuration.PlaySound)
                {
                    scrollSound.Recipients.AddRecipient(@event.PlayerId);
                    scrollSound.Emit();
                    scrollSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (KeyBind.A.HasFlag(@event.Key.ToKeyBind()))
            {
                CloseMenuForPlayer(player, menu);
                if (menu.Configuration.PlaySound)
                {
                    exitSound.Recipients.AddRecipient(@event.PlayerId);
                    exitSound.Emit();
                    exitSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (KeyBind.D.HasFlag(@event.Key.ToKeyBind()))
            {
                var option = menu.GetCurrentOption(player);
                if (option != null && option.Enabled && option.GetEnabled(player))
                {
                    _ = Task.Run(async () => await option.OnClickAsync(player));

                    if (menu.Configuration.PlaySound && option.PlaySound)
                    {
                        useSound.Recipients.AddRecipient(@event.PlayerId);
                        useSound.Emit();
                        useSound.Recipients.RemoveRecipient(@event.PlayerId);
                    }
                }
            }
        }
    }

    private void OnClientDisconnected( IOnClientDisconnectedEvent @event )
    {
        var player = Core.PlayerManager.GetPlayer(@event.PlayerId);
        if (player != null)
        {
            openMenus
                .Where(kvp => kvp.Key == player)
                .ToList()
                .ForEach(kvp => CloseMenuForPlayer(player, kvp.Value));
        }
    }

    private void OnMapUnload( IOnMapUnloadEvent _ )
    {
        CloseAllMenus();
    }

    public IMenuBuilderAPI CreateBuilder()
    {
        return new MenuBuilderAPI(Core);
    }

    public IMenuAPI CreateMenu( MenuConfiguration configuration, MenuKeybindOverrides keybindOverrides, IMenuAPI? parent = null, MenuOptionScrollStyle optionScrollStyle = MenuOptionScrollStyle.CenterFixed, MenuOptionTextStyle optionTextStyle = MenuOptionTextStyle.TruncateEnd )
    {
        var bindingList = new Dictionary<string, KeyBind> {
            ["Scroll"] = keybindOverrides.Move ?? buttonsScroll,
            ["ScrollBack"] = keybindOverrides.MoveBack ?? buttonsScrollBack,
            ["Exit"] = keybindOverrides.Exit ?? buttonsExit,
            ["Use"] = keybindOverrides.Select ?? buttonsUse
        }.ToList();

        for (var i = 0; i < bindingList.Count; i++)
        {
            for (var j = i + 1; j < bindingList.Count; j++)
            {
                var binding1 = bindingList[i];
                var binding2 = bindingList[j];
                var overlap = binding1.Value & binding2.Value;

                if (overlap != 0)
                {
                    Spectre.Console.AnsiConsole.WriteException(
                        new InvalidOperationException(
                            $"Key binding conflict detected in menu '{configuration.Title}': '{binding1.Key}' and '{binding2.Key}' share overlapping keys: {overlap}"
                        )
                    );
                }
            }
        }

        return new MenuAPI(Core, configuration, keybindOverrides, null/*, parent*/, optionScrollStyle/*, optionTextStyle*/) { Parent = parent };
    }

    public IMenuAPI? GetCurrentMenu( IPlayer player )
    {
        return openMenus.TryGetValue(player, out var menu) ? menu : null;
    }

    public void OpenMenu( IMenuAPI menu )
    {
        Core.PlayerManager
            .GetAllPlayers()
            .ToList()
            .ForEach(player => OpenMenuForPlayer(player, menu));
    }

    public void OpenMenuForPlayer( IPlayer player, IMenuAPI menu )
    {
        if (GetCurrentMenu(player) != null)
        {
            CloseMenuForPlayer(player, GetCurrentMenu(player)!);
        }

        _ = openMenus.AddOrUpdate(player, menu, ( _, _ ) => menu);
        menu.ShowForPlayer(player);
        MenuOpened?.Invoke(this, new MenuManagerEventArgs { Player = player, Menu = menu });
    }

    public void CloseMenu( IMenuAPI menu )
    {
        Core.PlayerManager
            .GetAllPlayers()
            .ToList()
            .ForEach(player => CloseMenuForPlayer(player, menu));
    }

    public void CloseMenuForPlayer( IPlayer player, IMenuAPI menu )
    {
        if (openMenus.TryRemove(player, out _))
        {
            menu.CloseForPlayer(player);
            MenuClosed?.Invoke(this, new MenuManagerEventArgs { Player = player, Menu = menu });

            if (menu.Parent != null)
            {
                OpenMenuForPlayer(player, menu.Parent);
            }
        }
    }

    public void CloseAllMenus()
    {
        openMenus.ToList().ForEach(kvp =>
        {
            var currentMenu = kvp.Value;
            while (currentMenu != null)
            {
                currentMenu.CloseForPlayer(kvp.Key);
                MenuClosed?.Invoke(this, new MenuManagerEventArgs { Player = kvp.Key, Menu = currentMenu });
                currentMenu = currentMenu.Parent;
            }
            _ = openMenus.TryRemove(kvp.Key, out _);
        });
    }
}