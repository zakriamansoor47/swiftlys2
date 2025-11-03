using SwiftlyS2.Core.Menus;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Core.Menu.Options;

internal class ToggleMenuOption : IOption
{
    public string Text { get; set; }
    public bool Value { get; set; }
    public Action<IPlayer, bool>? OnToggle { get; set; }
    public Action<IPlayer, IOption, bool>? OnToggleWithOption { get; set; }
    public Func<IPlayer, bool>? VisibilityCheck { get; set; }
    public Func<IPlayer, bool>? EnabledCheck { get; set; }
    public Func<IPlayer, bool>? ValidationCheck { get; set; }
    public Action<IPlayer>? OnValidationFailed { get; set; }
    public IMenuTextSize Size { get; set; }
    public bool CloseOnSelect { get; set; }
    public IMenu? Menu { get; set; }
    public MenuHorizontalStyle? OverflowStyle { get; init; }

    public bool Visible => true;
    public bool Enabled => true;

    public ToggleMenuOption(string text, bool defaultValue = false, Action<IPlayer, bool>? onToggle = null, IMenuTextSize size = IMenuTextSize.Medium)
    {
        Text = text;
        Value = defaultValue;
        OnToggle = onToggle;
        Size = size;
    }

    public ToggleMenuOption(string text, bool defaultValue, Action<IPlayer, IOption, bool>? onToggle, IMenuTextSize size = IMenuTextSize.Medium)
    {
        Text = text;
        Value = defaultValue;
        OnToggleWithOption = onToggle;
        Size = size;
    }

    public bool ShouldShow(IPlayer player)
    {
        return VisibilityCheck?.Invoke(player) ?? true;
    }

    public bool CanInteract(IPlayer player)
    {
        return EnabledCheck?.Invoke(player) ?? true;
    }

    public string GetDisplayText(IPlayer player)
    {
        var sizeClass = MenuSizeHelper.GetSizeClass(Size);

        var status = Value ? "<font color='#008000'>✔</font>" : "<font color='#FF0000'>✘</font>";

        if (!CanInteract(player))
        {
            return $"<font class='{sizeClass}' color='grey'>{Text}: {status}/</font>";
        }

        return $"<font class='{sizeClass}'>{Text}: {status}</font>";
    }

    public IMenuTextSize GetTextSize()
    {
        return Size;
    }

    public void Toggle(IPlayer player)
    {
        if (!CanInteract(player)) return;

        if (ValidationCheck != null && !ValidationCheck(player))
        {
            OnValidationFailed?.Invoke(player);
            return;
        }

        Value = !Value;
        OnToggle?.Invoke(player, Value);
        OnToggleWithOption?.Invoke(player, this, Value);
    }
}