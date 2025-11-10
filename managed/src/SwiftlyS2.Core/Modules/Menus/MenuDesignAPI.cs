using SwiftlyS2.Shared.Menus;

namespace SwiftlyS2.Core.Menus;

internal sealed class MenuDesignAPI : IMenuDesignAPI
{
    private readonly MenuConfiguration configuration;
    private readonly IMenuBuilderAPI builder;
    private readonly Action<MenuOptionScrollStyle> setScrollStyle;
    // private readonly Action<MenuOptionTextStyle> setTextStyle;

    public MenuDesignAPI( MenuConfiguration configuration, IMenuBuilderAPI builder, Action<MenuOptionScrollStyle> setScrollStyle/*, Action<MenuOptionTextStyle> setTextStyle*/ )
    {
        this.configuration = configuration;
        this.builder = builder;
        this.setScrollStyle = setScrollStyle;
        // this.setTextStyle = setTextStyle;
    }

    public IMenuBuilderAPI SetMenuTitle( string? title = null )
    {
        configuration.Title = title ?? "Menu";
        return builder;
    }

    public IMenuBuilderAPI HideMenuTitle( bool hide = false )
    {
        configuration.HideTitle = hide;
        return builder;
    }

    public IMenuBuilderAPI HideMenuFooter( bool hide = false )
    {
        configuration.HideFooter = hide;
        return builder;
    }

    public IMenuBuilderAPI MaxVisibleItems( int count = 5 )
    {
        configuration.MaxVisibleItems = count;
        return builder;
    }

    public IMenuBuilderAPI AutoIncreaseVisibleItems( bool autoIncrease = true )
    {
        configuration.AutoIncreaseVisibleItems = autoIncrease;
        return builder;
    }

    public IMenuBuilderAPI SetGlobalOptionScrollStyle( MenuOptionScrollStyle style )
    {
        setScrollStyle(style);
        return builder;
    }

    // public IMenuBuilderAPI SetGlobalOptionTextStyle( MenuOptionTextStyle style )
    // {
    //     setTextStyle(style);
    //     return builder;
    // }
}