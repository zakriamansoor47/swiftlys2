namespace SwiftlyS2.Shared.Menus;

public interface IMenuDesignAPI
{
    /// <summary>
    /// Sets the title of the menu.
    /// </summary>
    /// <param name="title">The title to display for the menu.</param>
    /// <returns>The menu builder for method chaining.</returns>
    public IMenuBuilderAPI SetMenuTitle( string? title = null );

    /// <summary>
    /// Hides the menu title.
    /// </summary>
    /// <param name="hide">True to hide the title, false to show it.</param>
    /// <returns>The menu builder for method chaining.</returns>
    public IMenuBuilderAPI HideMenuTitle( bool hide = false );

    /// <summary>
    /// Hides the menu footer.
    /// </summary>
    /// <param name="hide">True to hide the footer, false to show it.</param>
    /// <returns>The menu builder for method chaining.</returns>
    public IMenuBuilderAPI HideMenuFooter( bool hide = false );

    /// <summary>
    /// Sets how many menu items can be displayed on screen at once. Menus with more items will be paginated.
    /// </summary>
    /// <param name="count">Maximum visible items (clamped between 1 and 5).</param>
    /// <returns>The menu builder for method chaining.</returns>
    /// <remarks>
    /// Values outside the range of 1-5 will be automatically clamped, and a warning will be logged.
    /// </remarks>
    public IMenuBuilderAPI MaxVisibleItems( int count = 5 );

    /// <summary>
    /// Sets whether to automatically increase MaxVisibleItems when <see cref="HideMenuTitle"/> or <see cref="HideMenuFooter"/> is enabled.
    /// </summary>
    /// <param name="autoIncrease">True to automatically increase the maximum visible items, false to disable.</param>
    /// <returns>The menu builder for method chaining.</returns>
    /// <remarks>
    /// This does not modify the actual MaxVisibleItems value.
    /// Instead, the increase is applied during rendering calculations only.
    /// </remarks>
    public IMenuBuilderAPI AutoIncreaseVisibleItems( bool autoIncrease = true );

    /// <summary>
    /// Sets the global option scroll style for the menu.
    /// </summary>
    /// <param name="style">The scroll style to apply to all options in the menu.</param>
    /// <returns>The menu builder for method chaining.</returns>
    public IMenuBuilderAPI SetGlobalOptionScrollStyle( MenuOptionScrollStyle style );

    // /// <summary>
    // /// Sets the global option text style for the menu.
    // /// </summary>
    // /// <param name="style">The text style to apply to all options in the menu.</param>
    // /// <returns>The menu builder for method chaining.</returns>
    // public IMenuBuilderAPI SetGlobalOptionTextStyle( MenuOptionTextStyle style );
}