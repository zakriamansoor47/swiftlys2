using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.Menus;

namespace SwiftlyS2.Core.Menu.Options;

internal class SeparatorMenuOption : IOption
{
    public string Text { get; set; }
    public bool Visible => true;
    public bool Enabled => false;

    public IMenu? Menu { get; set; }

    public SeparatorMenuOption()
    {
        Text = "─────────────────────";
    }

    public bool ShouldShow(IPlayer player) => true;
    public bool CanInteract(IPlayer player) => false;

    public string GetDisplayText(IPlayer player)
    {
        return $"<font color='{Menu!.RenderColor.ToHex(true)}'>{Text}</font>";
    }

    public IMenuTextSize GetTextSize()
    {
        return IMenuTextSize.Small;
    }
}