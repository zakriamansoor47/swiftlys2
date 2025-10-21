using SwiftlyS2.Core.Menus;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Core.Menu.Options;

internal class AsyncButtonMenuOption(string text, Func<IPlayer, Task>? onClickAsync = null, IMenuTextSize size = IMenuTextSize.Medium) : IOption
{
    public string Text { get; set; } = text;
    public Func<IPlayer, Task>? OnClickAsync { get; set; } = onClickAsync;
    public Func<IPlayer, bool>? VisibilityCheck { get; set; }
    public Func<IPlayer, bool>? EnabledCheck { get; set; }
    public Func<IPlayer, bool>? ValidationCheck { get; set; }
    public Action<IPlayer>? OnValidationFailed { get; set; }
    public IMenuTextSize Size { get; set; } = size;
    public bool CloseOnSelect { get; set; }
    public IMenu? Menu { get; set; }

    public bool Visible => true;
    public bool Enabled => true;
    public bool IsLoading { get; set; }

    private string? _loadingText;

    public bool ShouldShow(IPlayer player)
    {
        return VisibilityCheck?.Invoke(player) ?? true;
    }

    public bool CanInteract(IPlayer player)
    {
        return !IsLoading && (EnabledCheck?.Invoke(player) ?? true);
    }

    public string GetDisplayText(IPlayer player)
    {
        var sizeClass = MenuSizeHelper.GetSizeClass(Size);

        if (IsLoading)
        {
            return $"<font class='{sizeClass}' color='#ffaa00'>{_loadingText ?? "Loading..."}</font>";
        }

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

    public async Task ExecuteAsync(IPlayer player, string? loadingText = null)
    {
        if (OnClickAsync == null) return;

        _loadingText = loadingText;

        try
        {
            await OnClickAsync.Invoke(player);
        }
        finally
        {
            _loadingText = null;
        }
    }

    public void SetLoadingText(string? text)
    {
        _loadingText = text;
    }
}