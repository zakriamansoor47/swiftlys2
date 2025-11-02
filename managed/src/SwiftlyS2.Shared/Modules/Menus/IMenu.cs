using System.Collections.Concurrent;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Shared.Menus;

/// <summary>
/// Represents a menu interface that provides functionality for creating and managing interactive menus for players.
/// Supports customizable options, events, and rendering behavior.
/// </summary>
public interface IMenu
{
    /// <summary>
    /// Gets or sets the title of the menu that will be displayed to players.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets the list of options available in this menu.
    /// Each option represents a selectable item that players can interact with.
    /// </summary>
    public List<IOption> Options { get; }

    /// <summary>
    /// Gets a value indicating whether the menu has associated sounds for interactions.
    /// </summary>
    public bool HasSound { get; set; }

    /// <summary>
    /// Gets or sets the parent menu for hierarchical menu navigation.
    /// When set, allows players to navigate back to the parent menu.
    /// </summary>
    public IMenu? Parent { get; set; }

    /// <summary>
    /// Gets or sets a concurrent dictionary that tracks auto-close cancellation tokens for each player.
    /// Used to manage automatic menu closing functionality per player.
    /// </summary>
    public ConcurrentDictionary<IPlayer, CancellationTokenSource?> AutoCloseCancelTokens { get; set; }

    /// <summary>
    /// Gets or sets custom button overrides for menu navigation.
    /// Allows customization of default menu control buttons.
    /// </summary>
    public IMenuButtonOverrides? ButtonOverrides { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of options visible at once in the menu.
    /// When there are more options than this limit, the menu will be paginated.
    /// </summary>
    public int MaxVisibleOptions { get; set; }

    /// <summary>
    /// Gets or sets whether the player should be frozen while the menu is open.
    /// When true, prevents player movement during menu interaction.
    /// </summary>
    public bool? ShouldFreeze { get; set; }

    /// <summary>
    /// Gets or sets whether the menu should automatically close when an option is selected.
    /// When true, the menu closes after any option selection.
    /// </summary>
    public bool? CloseOnSelect { get; set; }

    /// <summary>
    /// Gets or sets the color used for rendering the menu.
    /// Affects the visual appearance of the menu display.
    /// </summary>
    public Color RenderColor { get; set; }

    /// <summary>
    /// Gets or sets the menu manager responsible for handling this menu.
    /// Provides access to menu management functionality and state.
    /// </summary>
    public IMenuManager MenuManager { get; set; }

    /// <summary>
    /// Gets or sets the time in seconds after which the menu will automatically close.
    /// Set to 0 or negative value to disable auto-close functionality.
    /// </summary>
    public float AutoCloseAfter { get; set; }

    /// <summary>
    /// Gets a value indicating whether the menu should be re-rendered on each game tick.
    /// </summary>
    public bool RenderOntick { get; set; }

    /// <summary>
    /// Gets the menu builder used to construct and configure this menu.
    /// Provides fluent API for menu construction and modification.
    /// </summary>
    public IMenuBuilder Builder { get; }

    /// <summary>
    /// Event triggered when the menu is opened for a player.
    /// Provides the player instance as event argument.
    /// </summary>
    event Action<IPlayer>? OnOpen;

    /// <summary>
    /// Event triggered when the menu is closed for a player.
    /// Provides the player instance as event argument.
    /// </summary>
    event Action<IPlayer>? OnClose;

    /// <summary>
    /// Event triggered when a player moves their selection within the menu.
    /// Provides the player instance as event argument.
    /// </summary>
    event Action<IPlayer>? OnMove;

    /// <summary>
    /// Event triggered when a player selects a menu option.
    /// Provides both the player instance and the selected option as event arguments.
    /// </summary>
    event Action<IPlayer, IOption>? OnItemSelected;

    /// <summary>
    /// Event triggered when a player hovers over a menu option.
    /// Provides both the player instance and the hovered option as event arguments.
    /// </summary>
    event Action<IPlayer, IOption>? OnItemHovered;

    /// <summary>
    /// Event triggered before the menu is rendered for a player.
    /// Allows for last-minute modifications or preparations before display.
    /// </summary>
    event Action<IPlayer>? BeforeRender;

    /// <summary>
    /// Event triggered after the menu has been rendered for a player.
    /// Useful for post-render operations or logging.
    /// </summary>
    event Action<IPlayer>? AfterRender;

    /// <summary>
    /// Shows the menu to the specified player.
    /// Displays the menu interface and begins player interaction.
    /// </summary>
    /// <param name="player">The player to show the menu to.</param>
    public void Show(IPlayer player);

    /// <summary>
    /// Closes the menu for the specified player.
    /// Hides the menu interface and ends player interaction.
    /// </summary>
    /// <param name="player">The player to close the menu for.</param>
    public void Close(IPlayer player);

    /// <summary>
    /// Moves the player's selection by the specified offset.
    /// Positive values move down, negative values move up in the menu.
    /// </summary>
    /// <param name="player">The player whose selection to move.</param>
    /// <param name="offset">The number of positions to move the selection.</param>
    public void MoveSelection(IPlayer player, int offset);

    /// <summary>
    /// Activates the currently selected option for the specified player.
    /// Triggers the selected option's action or behavior.
    /// </summary>
    /// <param name="player">The player whose current selection to use.</param>
    public void UseSelection(IPlayer player);

    /// <summary>
    /// Handles slide option interaction for the specified player.
    /// Used for options that support left/right navigation or value adjustment.
    /// </summary>
    /// <param name="player">The player interacting with the slide option.</param>
    /// <param name="isRight">True if sliding right, false if sliding left.</param>
    public void UseSlideOption(IPlayer player, bool isRight);

    /// <summary>
    /// Forces a re-render of the menu for the specified player.
    /// Updates the menu display with current state and options.
    /// </summary>
    /// <param name="player">The player to re-render the menu for.</param>
    public void Rerender(IPlayer player);

    /// <summary>
    /// Determines whether the currently selected option is selectable for the specified player.
    /// Returns true if the option can be activated, false if it's disabled or non-interactive.
    /// </summary>
    /// <param name="player">The player to check the current selection for.</param>
    /// <returns>True if the current option is selectable, false otherwise.</returns>
    public bool IsCurrentOptionSelectable(IPlayer player);

    /// <summary>
    /// Determines whether the currently selected option is a slider for the specified player.
    /// Returns true if the option is a slider type, false otherwise.
    /// </summary>
    public bool IsOptionSlider(IPlayer player);

    /// <summary>
    /// Sets the freeze state for the specified player while the menu is active.
    /// Controls whether the player can move while interacting with the menu.
    /// </summary>
    /// <param name="player">The player to set the freeze state for.</param>
    /// <param name="freeze">True to freeze the player, false to unfreeze.</param>
    public void SetFreezeState(IPlayer player, bool freeze);

    /// <summary>
    /// Gets or sets the vertical scroll style for the menu navigation.
    /// Determines how the selection arrow moves when navigating through options.
    /// </summary>
    public MenuVerticalScrollStyle VerticalScrollStyle { get; set; }

    /// <summary>
    /// Gets or sets the horizontal text display style for menu options.
    /// Controls maximum text width and overflow behavior. Null means no horizontal restrictions.
    /// </summary>
    public MenuHorizontalStyle? HorizontalStyle { get; set; }
}

/// <summary>
/// Defines the vertical scroll behavior style for menu navigation.
/// </summary>
public enum MenuVerticalScrollStyle
{
    /// <summary>
    /// Linear vertical scrolling mode where the selection indicator moves within the visible area.
    /// Content displays linearly without wrapping, indicator adjusts position as selection changes.
    /// </summary>
    LinearScroll,

    /// <summary>
    /// Attempts to always keep the selection indicator at the preset center position.
    /// Content scrolls vertically in a circular manner around the center, allowing wrap-around display (e.g., 7 8 1 2 3).
    /// </summary>
    CenterFixed,

    /// <summary>
    /// Waits for the selection indicator to reach the preset center, then maintains it there.
    /// Indicator adjusts position at the edges but stays centered during mid-range vertical navigation.
    /// </summary>
    WaitingCenter
}

/// <summary>
/// Defines the horizontal text overflow behavior for menu options.
/// </summary>
public enum MenuHorizontalOverflowStyle
{
    /// <summary>
    /// Truncates text at the end when it exceeds the maximum width, keeping the start portion.
    /// Example: "Very Long Text Item" becomes "Very Long..."
    /// </summary>
    TruncateEnd,

    /// <summary>
    /// Truncates text from both ends when it exceeds the maximum width, keeping the middle portion.
    /// Example: "Very Long Text Item" becomes "Long Text"
    /// </summary>
    TruncateBothEnds,

    /// <summary>
    /// Scrolls text to the left with fade-out effect.
    /// Text scrolls left and gradually fades out at the left edge.
    /// </summary>
    ScrollLeftFade,

    /// <summary>
    /// Scrolls text to the right with fade-out effect.
    /// Text scrolls right and gradually fades out at the right edge.
    /// </summary>
    ScrollRightFade,

    /// <summary>
    /// Scrolls text to the left in a continuous loop.
    /// Text exits from the left edge and re-enters from the right edge.
    /// </summary>
    ScrollLeftLoop,

    /// <summary>
    /// Scrolls text to the right in a continuous loop.
    /// Text exits from the right edge and re-enters from the left edge.
    /// </summary>
    ScrollRightLoop
}

/// <summary>
/// Horizontal text display style configuration for menu options.
/// </summary>
/// <param name="MaxWidth">The maximum display width for menu option text in relative units.</param>
/// <param name="OverflowStyle">The overflow behavior to apply when text exceeds MaxWidth.</param>
public readonly record struct MenuHorizontalStyle(float MaxWidth, MenuHorizontalOverflowStyle OverflowStyle = MenuHorizontalOverflowStyle.TruncateEnd)
{
    /// <summary>
    /// Creates a horizontal style with truncate end behavior (most common).
    /// </summary>
    public static MenuHorizontalStyle Truncate(float maxWidth) => new(maxWidth, MenuHorizontalOverflowStyle.TruncateEnd);

    /// <summary>
    /// Creates a horizontal style with truncate both ends behavior.
    /// </summary>
    public static MenuHorizontalStyle TruncateBothEnds(float maxWidth) => new(maxWidth, MenuHorizontalOverflowStyle.TruncateBothEnds);

    /// <summary>
    /// Creates a horizontal style with scroll left fade behavior.
    /// </summary>
    public static MenuHorizontalStyle ScrollLeftFade(float maxWidth) => new(maxWidth, MenuHorizontalOverflowStyle.ScrollLeftFade);

    /// <summary>
    /// Creates a horizontal style with scroll right fade behavior.
    /// </summary>
    public static MenuHorizontalStyle ScrollRightFade(float maxWidth) => new(maxWidth, MenuHorizontalOverflowStyle.ScrollRightFade);

    /// <summary>
    /// Creates a horizontal style with scroll left loop behavior.
    /// </summary>
    public static MenuHorizontalStyle ScrollLeftLoop(float maxWidth) => new(maxWidth, MenuHorizontalOverflowStyle.ScrollLeftLoop);

    /// <summary>
    /// Creates a horizontal style with scroll right loop behavior.
    /// </summary>
    public static MenuHorizontalStyle ScrollRightLoop(float maxWidth) => new(maxWidth, MenuHorizontalOverflowStyle.ScrollRightLoop);
}