using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Shared.Menus;

/// <summary>
/// Provides a base implementation for menu options with event-driven behavior.
/// </summary>
public abstract class MenuOptionBase : IMenuOption
{
    private string text = string.Empty;
    private bool visible = true;
    private bool enabled = true;

    /// <summary>
    /// Gets or sets the text content displayed for this menu option.
    /// </summary>
    public string Text {
        get => text;
        set {
            if (text == value)
            {
                return;
            }

            text = value;
            OnTextChanged(new MenuOptionEventArgs { Player = null!, Option = this });
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this option is visible in the menu.
    /// </summary>
    public bool Visible {
        get => visible;
        set {
            if (visible == value)
            {
                return;
            }

            visible = value;
            OnVisibilityChanged(new MenuOptionEventArgs { Player = null!, Option = this });
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this option can be interacted with.
    /// </summary>
    public bool Enabled {
        get => enabled;
        set {
            if (enabled == value)
            {
                return;
            }

            enabled = value;
            OnEnabledChanged(new MenuOptionEventArgs { Player = null!, Option = this });
        }
    }

    /// <summary>
    /// Gets or sets the menu that this option belongs to.
    /// </summary>
    public IMenuAPI? Menu { get; set; }

    /// <summary>
    /// Gets or sets an object that contains data about this option.
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// Gets or sets the text size for this option.
    /// </summary>
    public IMenuOptionTextSize TextSize { get; set; } = IMenuOptionTextSize.Medium;

    /// <summary>
    /// Gets or sets a value indicating whether a sound should play when this option is selected.
    /// </summary>
    public bool PlaySound { get; set; } = true;

    /// <summary>
    /// Occurs when the visibility of the option changes.
    /// </summary>
    public event EventHandler<MenuOptionEventArgs>? VisibilityChanged;

    /// <summary>
    /// Occurs when the enabled state of the option changes.
    /// </summary>
    public event EventHandler<MenuOptionEventArgs>? EnabledChanged;

    /// <summary>
    /// Occurs when the text of the option changes.
    /// </summary>
    public event EventHandler<MenuOptionEventArgs>? TextChanged;

    /// <summary>
    /// Occurs before a click is processed, allowing validation and cancellation.
    /// </summary>
    public event EventHandler<MenuOptionValidatingEventArgs>? Validating;

    /// <summary>
    /// Occurs when the option is clicked by a player.
    /// </summary>
    public event AsyncEventHandler<MenuOptionClickEventArgs>? Click;

    /// <summary>
    /// Occurs when a player's cursor enters this option.
    /// </summary>
    public event EventHandler<MenuOptionEventArgs>? OptionEnter;

    /// <summary>
    /// Occurs when a player's cursor leaves this option.
    /// </summary>
    public event EventHandler<MenuOptionEventArgs>? OptionLeave;

    /// <summary>
    /// Occurs before HTML markup is assembled, allowing customization of the text content.
    /// </summary>
    public event EventHandler<MenuOptionFormattingEventArgs>? BeforeFormat;

    /// <summary>
    /// Occurs after HTML markup is assembled, allowing customization of the final HTML output.
    /// </summary>
    public event EventHandler<MenuOptionFormattingEventArgs>? AfterFormat;

    /// <summary>
    /// Determines whether this option is visible to the specified player.
    /// </summary>
    /// <param name="player">The player to check visibility for.</param>
    /// <returns>True if the option is visible to the player; otherwise, false.</returns>
    public virtual bool GetVisible( IPlayer player ) => Visible;

    /// <summary>
    /// Determines whether this option is enabled for the specified player.
    /// </summary>
    /// <param name="player">The player to check enabled state for.</param>
    /// <returns>True if the option is enabled for the player; otherwise, false.</returns>
    public virtual bool GetEnabled( IPlayer player ) => Enabled;

    /// <summary>
    /// Gets the text to display for this option for the specified player.
    /// </summary>
    /// <param name="player">The player requesting the text.</param>
    /// <returns>The text to display.</returns>
    public virtual string GetText( IPlayer player ) => Text;

    /// <summary>
    /// Gets the formatted HTML markup for this option.
    /// </summary>
    /// <param name="player">The player to format for.</param>
    /// <returns>The formatted HTML string.</returns>
    public virtual string GetFormattedHtmlText( IPlayer player )
    {
        var args = new MenuOptionFormattingEventArgs {
            Player = player,
            Option = this,
            CustomText = null
        };

        BeforeFormat?.Invoke(this, args);

        var displayText = args.CustomText ?? GetText(player);
        var isEnabled = GetEnabled(player);
        var sizeClass = GetSizeClass(TextSize);

        var colorStyle = isEnabled ? "" : " color='grey'";
        var result = $"<font class='{sizeClass}'{colorStyle}>{displayText}</font>";

        args.CustomText = result;
        AfterFormat?.Invoke(this, args);

        return args.CustomText;
    }

    /// <summary>
    /// Validates whether the specified player can interact with this option.
    /// </summary>
    /// <param name="player">The player to validate.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if validation succeeds; otherwise, false.</returns>
    public virtual ValueTask<bool> OnValidatingAsync( IPlayer player )
    {
        if (Validating == null)
        {
            return ValueTask.FromResult(true);
        }

        var args = new MenuOptionValidatingEventArgs {
            Player = player,
            Option = this,
            Cancel = false
        };

        Validating?.Invoke(this, args);
        return ValueTask.FromResult(!args.Cancel);
    }

    /// <summary>
    /// Handles the click action for this option.
    /// </summary>
    /// <param name="player">The player who clicked the option.</param>
    /// <param name="closeMenu">Whether to close the menu after handling the click.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask OnClickAsync( IPlayer player, bool closeMenu = false )
    {
        if (!await OnValidatingAsync(player))
        {
            return;
        }

        if (Click != null)
        {
            var args = new MenuOptionClickEventArgs {
                Player = player,
                Option = this,
                CloseMenu = closeMenu
            };

            await Click.Invoke(this, args);

            if (args.CloseMenu)
            {
                Menu?.Close(player);
            }
        }
    }

    /// <summary>
    /// Raises the <see cref="VisibilityChanged"/> event.
    /// </summary>
    /// <param name="args">Event data.</param>
    protected virtual void OnVisibilityChanged( MenuOptionEventArgs args )
    {
        VisibilityChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the <see cref="EnabledChanged"/> event.
    /// </summary>
    /// <param name="args">Event data.</param>
    protected virtual void OnEnabledChanged( MenuOptionEventArgs args )
    {
        EnabledChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the <see cref="TextChanged"/> event.
    /// </summary>
    /// <param name="args">Event data.</param>
    protected virtual void OnTextChanged( MenuOptionEventArgs args )
    {
        TextChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the <see cref="OptionEnter"/> event.
    /// </summary>
    /// <param name="player">The player whose cursor entered the option.</param>
    protected virtual void OnOptionEnter( IPlayer player )
    {
        OptionEnter?.Invoke(this, new MenuOptionEventArgs { Player = player, Option = this });
    }

    /// <summary>
    /// Raises the <see cref="OptionLeave"/> event.
    /// </summary>
    /// <param name="player">The player whose cursor left the option.</param>
    protected virtual void OnOptionLeave( IPlayer player )
    {
        OptionLeave?.Invoke(this, new MenuOptionEventArgs { Player = player, Option = this });
    }

    private static string GetSizeClass( IMenuOptionTextSize size )
    {
        return size switch {
            IMenuOptionTextSize.ExtraSmall => "fontSize-xs",
            IMenuOptionTextSize.Small => "fontSize-s",
            IMenuOptionTextSize.SmallMedium => "fontSize-sm",
            IMenuOptionTextSize.Medium => "fontSize-m",
            IMenuOptionTextSize.MediumLarge => "fontSize-ml",
            IMenuOptionTextSize.Large => "fontSize-l",
            IMenuOptionTextSize.ExtraLarge => "fontSize-xl",
            _ => "fontSize-m"
        };
    }
}
