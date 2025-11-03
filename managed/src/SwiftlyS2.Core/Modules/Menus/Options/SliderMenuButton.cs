using SwiftlyS2.Core.Menus;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace SwiftlyS2.Core.Menu.Options;

internal class SliderMenuButton : IOption
{
    public string Text { get; set; }
    public float Value { get; set; }
    public float Min { get; set; }
    public float Max { get; set; }
    public float Step { get; set; }
    public Action<IPlayer, float>? OnChange { get; set; }
    public Action<IPlayer, IOption, float>? OnChangeWithOption { get; set; }
    public Func<IPlayer, bool>? VisibilityCheck { get; set; }
    public Func<IPlayer, bool>? EnabledCheck { get; set; }
    public IMenuTextSize Size { get; set; }
    public IMenu? Menu { get; set; }
    public MenuHorizontalStyle? OverflowStyle { get; init; }

    public bool Visible => true;
    public bool Enabled => true;

    public SliderMenuButton(string text, float min = 0, float max = 10, float defaultValue = 5, float step = 1, Action<IPlayer, float>? onChange = null, IMenuTextSize size = IMenuTextSize.Medium)
    {
        Text = text;
        Min = min;
        Max = max;
        Value = Math.Clamp(defaultValue, min, max);
        Step = step;
        OnChange = onChange;
        Size = size;
    }

    public SliderMenuButton(string text, float min, float max, float defaultValue, float step, Action<IPlayer, IOption, float>? onChange, IMenuTextSize size = IMenuTextSize.Medium)
    {
        Text = text;
        Min = min;
        Max = max;
        Value = Math.Clamp(defaultValue, min, max);
        Step = step;
        OnChangeWithOption = onChange;
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

    private static float Wrap(float value, float min, float max)
    {
        float range = max - min;
        return ((value - min) % range + range) % range + min;
    }

    public void Increase(IPlayer player)
    {
        if (!CanInteract(player)) return;

        var newValue = Wrap(Value + Step, Min, Max);

        if (newValue != Value)
        {
            Value = newValue;
            OnChange?.Invoke(player, Value);
            OnChangeWithOption?.Invoke(player, this, Value);
        }
    }
    public void Decrease(IPlayer player)
    {
        if (!CanInteract(player)) return;

        var newValue = Wrap(Value - Step, Min, Max);

        if (newValue != Value)
        {
            Value = newValue;
            OnChange?.Invoke(player, Value);
            OnChangeWithOption?.Invoke(player, this, Value);
        }
    }
}