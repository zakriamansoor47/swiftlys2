using SwiftlyS2.Core.Menus;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Core.Menu.Options;

public class TextMenuOption : IOption
{
    public string Text { get; set; }
    public ITextAlign Alignment { get; set; }
    public IMenuTextSize Size { get; set; }
    public Func<IPlayer, bool>? VisibilityCheck { get; set; }
    public Func<string>? DynamicText { get; set; }
    public IMenu? Menu { get; set; }

    public bool Visible => true;
    public bool Enabled => false;

    public TextMenuOption(string text, ITextAlign alignment = ITextAlign.Left, IMenuTextSize size = IMenuTextSize.Medium)
    {
        Text = text;
        Alignment = alignment;
        Size = size;
    }

    public TextMenuOption(Func<string> dynamicText, ITextAlign alignment = ITextAlign.Left, IMenuTextSize size = IMenuTextSize.Medium)
    {
        Text = "";
        DynamicText = dynamicText;
        Alignment = alignment;
        Size = size;
    }

    public bool ShouldShow(IPlayer player)
    {
        return VisibilityCheck?.Invoke(player) ?? true;
    }

    public bool CanInteract(IPlayer player)
    {
        return true;
    }

    public string GetDisplayText(IPlayer player)
    {
        var text = DynamicText?.Invoke() ?? Text;

        var sizeClass = MenuSizeHelper.GetSizeClass(Size);

        text = $"<font class='{sizeClass}'>{text}</font>";

        return Alignment switch
        {
            ITextAlign.Center => $"<center>{text}</center>",
            ITextAlign.Right => $"<div align='right'>{text}</div>",
            _ => text
        };
    }

    public IMenuTextSize GetTextSize()
    {
        return Size;
    }
}

public enum TextAlign
{
    Left,
    Center,
    Right
}