using System.Collections.Concurrent;
using System.Globalization;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.Sounds;

namespace SwiftlyS2.Core.Menus;

internal class MenuManager : IMenuManager
{
    public MenuSettings Settings { get; set; } = new MenuSettings();

    public event Action<IPlayer, IMenu>? OnMenuClosed;
    public event Action<IPlayer, IMenu>? OnMenuOpened;
    public event Action<IPlayer, IMenu>? OnMenuRendered;

    private static readonly Dictionary<string, KeyKind> StringToKeyKind = new()
    {
        { "mouse1", KeyKind.Mouse1 },
        { "mouse2", KeyKind.Mouse2 },
        { "space", KeyKind.Space },
        { "ctrl", KeyKind.Ctrl },
        { "w", KeyKind.W },
        { "a", KeyKind.A },
        { "s", KeyKind.S },
        { "d", KeyKind.D },
        { "e", KeyKind.E },
        { "esc", KeyKind.Esc },
        { "r", KeyKind.R },
        { "alt", KeyKind.Alt },
        { "shift", KeyKind.Shift },
        { "weapon1", KeyKind.Weapon1 },
        { "weapon2", KeyKind.Weapon2 },
        { "grenade1", KeyKind.Grenade1 },
        { "grenade2", KeyKind.Grenade2 },
        { "tab", KeyKind.Tab },
        { "f", KeyKind.F },
    };

    private ConcurrentDictionary<IPlayer, IMenu> OpenMenus { get; set; } = new();
    private ISwiftlyCore _Core { get; set; }

    private SoundEvent _useSound = new();
    private SoundEvent _exitSound = new();
    private SoundEvent _scrollSound = new();

    public MenuManager(ISwiftlyCore core)
    {
        _Core = core;
        var settings = NativeEngineHelpers.GetMenuSettings();
        var parts = settings.Split('\x01');
        Settings = new MenuSettings
        {
            NavigationPrefix = parts[0],
            InputMode = parts[1],
            ButtonsUse = parts[2],
            ButtonsScroll = parts[3],
            ButtonsExit = parts[4],
            SoundUseName = parts[5],
            SoundUseVolume = float.Parse(parts[6], CultureInfo.InvariantCulture),
            SoundScrollName = parts[7],
            SoundScrollVolume = float.Parse(parts[8], CultureInfo.InvariantCulture),
            SoundExitName = parts[9],
            SoundExitVolume = float.Parse(parts[10], CultureInfo.InvariantCulture),
            ItemsPerPage = int.Parse(parts[11]),
        };

        _scrollSound.Name = Settings.SoundScrollName;
        _scrollSound.Volume = Settings.SoundScrollVolume;

        _useSound.Name = Settings.SoundUseName;
        _useSound.Volume = Settings.SoundUseVolume;

        _exitSound.Name = Settings.SoundExitName;
        _exitSound.Volume = Settings.SoundExitVolume;

        _Core.Event.OnClientKeyStateChanged += KeyStateChange;
    }

    ~MenuManager()
    {
        foreach (var kvp in OpenMenus)
        {
            var player = kvp.Key;
            var menu = kvp.Value;
            menu.Close(player);
        }

        _Core.Event.OnClientKeyStateChanged -= KeyStateChange;
    }

    void KeyStateChange(IOnClientKeyStateChangedEvent @event)
    {
        var player = _Core.PlayerManager.GetPlayer(@event.PlayerId);
        var menu = GetMenu(player);
        if (menu == null) return;

        if (Settings.InputMode == "button")
        {
            var scrollKey = menu.ButtonOverrides?.Move ?? StringToKeyKind.GetValueOrDefault(Settings.ButtonsScroll);
            var exitKey = menu.ButtonOverrides?.Exit ?? StringToKeyKind.GetValueOrDefault(Settings.ButtonsExit);
            var useKey = menu.ButtonOverrides?.Select ?? StringToKeyKind.GetValueOrDefault(Settings.ButtonsUse);

            if (@event.Key == scrollKey && @event.Pressed)
            {
                menu.MoveSelection(player, 1);

                if (menu.HasSound)
                {
                    _scrollSound.Recipients.AddRecipient(@event.PlayerId);
                    _scrollSound.Emit();
                    _scrollSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (@event.Key == exitKey && @event.Pressed)
            {
                _Core.Menus.CloseMenuForPlayer(player);

                if (menu.HasSound)
                {
                    _exitSound.Recipients.AddRecipient(@event.PlayerId);
                    _exitSound.Emit();
                    _exitSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (@event.Key == useKey && @event.Pressed)
            {
                if (menu.IsOptionSlider(player)) menu.UseSlideOption(player, true);
                else menu.UseSelection(player);

                if (menu.HasSound)
                {
                    _useSound.Recipients.AddRecipient(@event.PlayerId);
                    _useSound.Emit();
                    _useSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
        }
        else if (Settings.InputMode == "wasd")
        {
            if (@event.Key == KeyKind.W && @event.Pressed)
            {
                menu.MoveSelection(player, -1);

                if (menu.HasSound)
                {
                    _scrollSound.Recipients.AddRecipient(@event.PlayerId);
                    _scrollSound.Emit();
                    _scrollSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (@event.Key == KeyKind.S && @event.Pressed)
            {
                menu.MoveSelection(player, 1);

                if (menu.HasSound)
                {
                    _scrollSound.Recipients.AddRecipient(@event.PlayerId);
                    _scrollSound.Emit();
                    _scrollSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (@event.Key == KeyKind.A && @event.Pressed)
            {
                CloseMenuForPlayer(player);
                if (menu.HasSound)
                {
                    _exitSound.Recipients.AddRecipient(@event.PlayerId);
                    _exitSound.Emit();
                    _exitSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
            else if (@event.Key == KeyKind.D && @event.Pressed)
            {
                if (menu.IsOptionSlider(player)) menu.UseSlideOption(player, true);
                else menu.UseSelection(player);

                if (menu.HasSound)
                {
                    _useSound.Recipients.AddRecipient(@event.PlayerId);
                    _useSound.Emit();
                    _useSound.Recipients.RemoveRecipient(@event.PlayerId);
                }
            }
        }
    }

    public void CloseMenu(IMenu menu)
    {
        foreach (var kvp in OpenMenus)
        {
            var player = kvp.Key;
            var openMenu = kvp.Value;

            if (openMenu == menu)
            {
                CloseMenuForPlayer(player);
            }
        }
    }

    public void CloseMenuByTitle(string title, bool exact = false)
    {
        foreach (var kvp in OpenMenus)
        {
            var player = kvp.Key;
            var menu = kvp.Value;

            if ((exact && menu.Title == title) || (!exact && menu.Title.Contains(title)))
            {
                CloseMenuForPlayer(player);
            }
        }
    }

    public void CloseMenuForPlayer(IPlayer player)
    {
        if (OpenMenus.TryRemove(player, out var menu))
        {
            menu.Close(player);
            OnMenuClosed?.Invoke(player, menu);
            if (menu.Parent != null)
            {
                OpenMenu(player, menu.Parent);
            }
        }
    }

    public IMenu CreateMenu(string title)
    {
        return new Menu { Title = title, MenuManager = this, MaxVisibleOptions = Settings.ItemsPerPage, _Core = _Core };
    }

    public IMenu? GetMenu(IPlayer player)
    {
        return OpenMenus.TryGetValue(player, out var menu) ? menu : null;
    }

    public void OpenMenu(IPlayer player, IMenu menu)
    {
        OpenMenus[player] = menu;
        menu.Show(player);
        OnMenuOpened?.Invoke(player, menu);
    }
}