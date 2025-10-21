using SwiftlyS2.Core.Menus;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Core.Menu.Options;

internal class ButtonMenuOption : IOption
{
    public string Text { get; set; }
    public Action<IPlayer>? OnClick { get; set; }
    public Func<IPlayer, bool>? VisibilityCheck { get; set; }
    public Func<IPlayer, bool>? EnabledCheck { get; set; }
    public Func<IPlayer, bool>? ValidationCheck { get; set; }
    public Action<IPlayer>? OnValidationFailed { get; set; }
    public IMenuTextSize Size { get; set; }
    public bool CloseOnSelect { get; set; }
    public IMenu? Menu { get; set; }

    public bool Visible => true;
    public bool Enabled => true;

    public ButtonMenuOption(string text, Action<IPlayer>? onClick = null, IMenuTextSize size = IMenuTextSize.Medium)
    {
        Text = text;
        OnClick = onClick;
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

        if (!CanInteract(player))
        {
            return $"<font class='{sizeClass}' color='grey'>{Text}</font>";
        }

        return $"<font class='{sizeClass}'>{Text}</font>";
    }
    public IMenuTextSize GetTextSize()
    {
        return Size;
    }

}