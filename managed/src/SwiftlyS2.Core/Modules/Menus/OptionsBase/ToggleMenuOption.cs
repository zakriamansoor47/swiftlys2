using System.Collections.Concurrent;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Core.Menus.OptionsBase;

/// <summary>
/// Represents a toggleable menu option that displays an on/off state.
/// </summary>
public sealed class ToggleMenuOption : MenuOptionBase
{
    private readonly ConcurrentDictionary<IPlayer, bool> toggled = new();

    /// <summary>
    /// Creates an instance of <see cref="ToggleMenuOption"/> with dynamic text updating capabilities.
    /// </summary>
    /// <param name="updateIntervalMs">The interval in milliseconds between text updates. Defaults to 120ms.</param>
    /// <param name="pauseIntervalMs">The pause duration in milliseconds before starting the next text update cycle. Defaults to 1000ms.</param>
    /// <remarks>
    /// When using this constructor, the <see cref="MenuOptionBase.Text"/> property must be manually set to specify the initial text.
    /// </remarks>
    public ToggleMenuOption(
        int updateIntervalMs = 120,
        int pauseIntervalMs = 1000 ) : base(updateIntervalMs, pauseIntervalMs)
    {
        PlaySound = true;

        toggled.Clear();

        Click += OnToggleClick;
    }

    /// <summary>
    /// Creates an instance of <see cref="ToggleMenuOption"/> with dynamic text updating capabilities.
    /// </summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="updateIntervalMs">The interval in milliseconds between text updates. Defaults to 120ms.</param>
    /// <param name="pauseIntervalMs">The pause duration in milliseconds before starting the next text update cycle. Defaults to 1000ms.</param>
    public ToggleMenuOption(
        string text,
        int updateIntervalMs = 120,
        int pauseIntervalMs = 1000 ) : this(updateIntervalMs, pauseIntervalMs)
    {
        Text = text;
    }

    /// <summary>
    /// Gets the display text for this option, including the toggle state indicator.
    /// </summary>
    /// <param name="player">The player viewing the option.</param>
    /// <param name="displayLine">The display line number (not used in this implementation).</param>
    /// <returns>The formatted display text with toggle state indicator.</returns>
    public override string GetDisplayText( IPlayer player, int displayLine = 0 )
    {
        var text = base.GetDisplayText(player, displayLine);
        var isToggled = toggled.GetOrAdd(player, true);
        return $"{text} {(isToggled ? "<font color='#008000'>✔</font>" : "<font color='#FF0000'>✘</font>")}";
    }

    private ValueTask OnToggleClick( object? sender, MenuOptionClickEventArgs args )
    {
        _ = toggled.AddOrUpdate(args.Player, true, ( _, current ) => !current);
        return ValueTask.CompletedTask;
    }
}