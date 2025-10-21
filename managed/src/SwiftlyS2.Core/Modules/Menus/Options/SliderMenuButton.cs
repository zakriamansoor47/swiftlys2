using SwiftlyS2.Core.Menus;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace SwiftlyS2.Core.Menu.Options;

internal class SliderMenuButton(string text, float min = 0, float max = 10, float defaultValue = 5, float step = 1, Action<IPlayer, float>? onChange = null, IMenuTextSize size = IMenuTextSize.Medium) : IOption
{
    public string Text { get; set; } = text;
    public float Value { get; set; } = Math.Clamp(defaultValue, min, max);
    public float Min { get; set; } = min;
    public float Max { get; set; } = max;
    public float Step { get; set; } = step;
    public Action<IPlayer, float>? OnChange { get; set; } = onChange;
    public Func<IPlayer, bool>? VisibilityCheck { get; set; }
    public Func<IPlayer, bool>? EnabledCheck { get; set; }
    public IMenuTextSize Size { get; set; } = size;
    public IMenu? Menu { get; set; }

    public bool Visible => true;
    public bool Enabled => true;

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

        int totalBars = 10;
        float percentage = (Value - Min) / (Max - Min);
        int filledBars = (int)(percentage * totalBars);

        string slider = $"<font color='{Menu!.RenderColor.ToHex(true)}'>(</font>";
        for (int i = 0; i < totalBars; i++)
        {
            if (i < filledBars)
                slider += $"<font color='{Menu!.RenderColor.ToHex(true)}'>■</font>";
            else
                slider += "<font color='#666666'>□</font>";
        }
        slider += $"<font color='#ff3333'>)</font> {Value:F1}";

        if (!CanInteract(player))
        {
            return $"<font class='{sizeClass}' color='grey'>{Text}: {slider}</font>";
        }

        return $"<font class='{sizeClass}'>{Text}: {slider}</font>";
    }

    public IMenuTextSize GetTextSize()
    {
        return Size;
    }

    public void Increase(IPlayer player)
    {
        if (!CanInteract(player)) return;
        var newValue = Math.Min(Value + Step, Max);
        if (newValue != Value)
        {
            Value = newValue;
            OnChange?.Invoke(player, Value);
        }
    }
    public void Decrease(IPlayer player)
    {
        if (!CanInteract(player)) return;
        var newValue = Math.Max(Value - Step, Min);
        if (newValue != Value)
        {
            Value = newValue;
            OnChange?.Invoke(player, Value);
        }
    }
}