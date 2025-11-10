using System.Collections.Concurrent;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Core.Menus.OptionsBase;

/// <summary>
/// Represents a menu option that opens a submenu when clicked.
/// </summary>
public sealed class SubmenuMenuOption : MenuOptionBase
{
    private IMenuAPI? submenu;
    private readonly Func<Task<IMenuAPI>>? submenuBuilderAsync;
    private readonly ConcurrentDictionary<IPlayer, bool> isLoading = new();

    /// <summary>
    /// Occurs when the submenu is ready to be opened.
    /// </summary>
    public event EventHandler<MenuManagerEventArgs>? SubmenuRequested;

    /// <summary>
    /// Creates an instance of <see cref="SubmenuMenuOption"/> with a pre-built submenu.
    /// </summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="submenu">The submenu to open when this option is clicked.</param>
    /// <param name="updateIntervalMs">The interval in milliseconds between text updates. Defaults to 120ms.</param>
    /// <param name="pauseIntervalMs">The pause duration in milliseconds before starting the next text update cycle. Defaults to 1000ms.</param>
    public SubmenuMenuOption(
        string text,
        IMenuAPI submenu,
        int updateIntervalMs = 120,
        int pauseIntervalMs = 1000 ) : base(updateIntervalMs, pauseIntervalMs)
    {
        Text = text;
        PlaySound = true;
        this.submenu = submenu;

        Click += OnSubmenuClick;
    }

    /// <summary>
    /// Creates an instance of <see cref="SubmenuMenuOption"/> with a synchronous builder.
    /// </summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="submenuBuilder">Function that builds and returns the submenu.</param>
    /// <param name="updateIntervalMs">The interval in milliseconds between text updates. Defaults to 120ms.</param>
    /// <param name="pauseIntervalMs">The pause duration in milliseconds before starting the next text update cycle. Defaults to 1000ms.</param>
    public SubmenuMenuOption(
        string text,
        Func<IMenuAPI> submenuBuilder,
        int updateIntervalMs = 120,
        int pauseIntervalMs = 1000 ) : base(updateIntervalMs, pauseIntervalMs)
    {
        Text = text;
        PlaySound = true;
        this.submenuBuilderAsync = () => Task.FromResult(submenuBuilder());

        Click += OnSubmenuClick;
    }

    /// <summary>
    /// Creates an instance of <see cref="SubmenuMenuOption"/> with an asynchronous builder.
    /// </summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="submenuBuilderAsync">Async function that builds and returns the submenu.</param>
    /// <param name="updateIntervalMs">The interval in milliseconds between text updates. Defaults to 120ms.</param>
    /// <param name="pauseIntervalMs">The pause duration in milliseconds before starting the next text update cycle. Defaults to 1000ms.</param>
    public SubmenuMenuOption(
        string text,
        Func<Task<IMenuAPI>> submenuBuilderAsync,
        int updateIntervalMs = 120,
        int pauseIntervalMs = 1000 ) : base(updateIntervalMs, pauseIntervalMs)
    {
        Text = text;
        PlaySound = true;
        this.submenuBuilderAsync = submenuBuilderAsync;

        Click += OnSubmenuClick;
    }

    public override string GetDisplayText( IPlayer player, int displayLine = 0 )
    {
        if (isLoading.TryGetValue(player, out var loading) && loading)
        {
            return "<font color='#FFA500'>Waiting...</font>";
        }

        return base.GetDisplayText(player, displayLine);
    }

    private async ValueTask OnSubmenuClick( object? sender, MenuOptionClickEventArgs args )
    {
        var menu = await GetSubmenuAsync(args.Player);
        if (menu != null)
        {
            SubmenuRequested?.Invoke(this, new MenuManagerEventArgs {
                Player = args.Player,
                Menu = menu
            });
        }
    }

    private async Task<IMenuAPI?> GetSubmenuAsync( IPlayer player )
    {
        if (submenu != null)
        {
            return submenu;
        }

        if (submenuBuilderAsync != null)
        {
            _ = isLoading.AddOrUpdate(player, true, ( _, _ ) => true);

            try
            {
                submenu = await submenuBuilderAsync.Invoke();
                return submenu;
            }
            finally
            {
                _ = isLoading.AddOrUpdate(player, false, ( _, _ ) => false);
            }
        }

        return null;
    }
}