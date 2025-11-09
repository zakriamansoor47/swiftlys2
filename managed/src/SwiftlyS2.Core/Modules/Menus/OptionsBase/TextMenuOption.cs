using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Core.Menus.OptionsBase;

public sealed class TextMenuOption : MenuOptionBase
{
    public TextMenuOption( string text )
    {
        Text = text;
    }

    public TextMenuOption( string text, MenuOptionTextSize textSize )
    {
        Text = text;
        TextSize = textSize;
    }

    public TextMenuOption( string text, MenuOptionTextStyle textStyle )
    {
        Text = text;
        TextStyle = textStyle;
    }

    public TextMenuOption( string text, MenuOptionTextSize textSize, MenuOptionTextStyle textStyle )
    {
        Text = text;
        TextSize = textSize;
        TextStyle = textStyle;
    }

    public override string GetDisplayText( IPlayer player, int displayLine = 0 )
    {
        return base.GetDisplayText(player, displayLine);
    }
}