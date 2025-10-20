using SwiftlyS2.Core.Menu.Options;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Core.Menus;

internal class MenuBuilder : IMenuBuilder
{
    private IMenu? _menu;
    private IMenu? _parent;

    public IMenuBuilder SetMenu(IMenu menu)
    {
        _menu = menu;
        return this;
    }

    public IMenuBuilder AddButton(string text, Action<IPlayer>? onClick = null, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new ButtonMenuOption(text, onClick, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }

    public IMenuBuilder AddButton(string text, Action<IPlayer>? onClick)
    {
        return AddButton(text, onClick, IMenuTextSize.Medium);
    }

    public IMenuBuilder AddToggle(string text, bool defaultValue = false, Action<IPlayer, bool>? onToggle = null, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new ToggleMenuOption(text, defaultValue, onToggle, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }

    public IMenuBuilder AddToggle(string text, bool defaultValue, Action<IPlayer, bool>? onToggle)
    {
        return AddToggle(text, defaultValue, onToggle, IMenuTextSize.Medium);
    }

    public IMenuBuilder AddSlider(string text, float min, float max, float defaultValue, float step = 1, Action<IPlayer, float>? onChange = null, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new SliderMenuButton(text, min, max, defaultValue, step, onChange, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }

    public IMenuBuilder AddSlider(string text, float min, float max, float defaultValue, float step, Action<IPlayer, float>? onChange)
    {
        return AddSlider(text, min, max, defaultValue, step, onChange, IMenuTextSize.Medium);
    }

    public IMenuBuilder AddAsyncButton(string text, Func<IPlayer, Task> onClickAsync, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new AsyncButtonMenuOption(text, onClickAsync, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }

    public IMenuBuilder AddAsyncButton(string text, Func<IPlayer, Task> onClickAsync)
    {
        return AddAsyncButton(text, onClickAsync, IMenuTextSize.Medium);
    }

    public IMenuBuilder AddText(string text, ITextAlign alignment = ITextAlign.Left, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new TextMenuOption(text, alignment, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }

    public IMenuBuilder AddDynamicText(Func<string> textProvider, TimeSpan updateInterval, Action<IPlayer>? onClick = null, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new DynamicMenuOption(textProvider, updateInterval, onClick, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }
    public IMenuBuilder AddSubmenu(string text, IMenu submenu, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new SubmenuMenuOption(text, submenu, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }

    public IMenuBuilder AddSubmenu(string text, IMenu submenu)
    {
        return AddSubmenu(text, submenu, IMenuTextSize.Medium);
    }

    public IMenuBuilder AddSubmenu(string text, Func<IMenu> submenuBuilder, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new SubmenuMenuOption(text, submenuBuilder, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }

    public IMenuBuilder AddSubmenu(string text, Func<IMenu> submenuBuilder)
    {
        return AddSubmenu(text, submenuBuilder, IMenuTextSize.Medium);
    }
    public IMenuBuilder AddChoice(string text, string[] choices, string? defaultChoice = null, Action<IPlayer, string>? onChange = null, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new ChoiceMenuOption(text, choices, defaultChoice, onChange, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }

    public IMenuBuilder AddChoice(string text, string[] choices, string? defaultChoice, Action<IPlayer, string>? onChange)
    {
        return AddChoice(text, choices, defaultChoice, onChange, IMenuTextSize.Medium);
    }
    public IMenuBuilder AddSeparator()
    {
        _menu!.Options.Add(new SeparatorMenuOption());
        _menu!.Options[^1].Menu = _menu;
        return this;
    }
    public IMenuBuilder AddProgressBar(string text, Func<float> progressProvider, int barWidth = 20, IMenuTextSize size = IMenuTextSize.Medium)
    {
        _menu!.Options.Add(new ProgressBarMenuOption(text, progressProvider, barWidth, size));
        _menu!.Options[^1].Menu = _menu;
        return this;
    }

    public IMenuBuilder AddProgressBar(string text, Func<float> progressProvider, int barWidth)
    {
        return AddProgressBar(text, progressProvider, barWidth, IMenuTextSize.Medium);
    }
    public IMenuBuilder WithParent(IMenu parent)
    {
        _parent = parent;
        _menu!.Parent = parent;
        return this;
    }

    public IMenuBuilder VisibleWhen(Func<IPlayer, bool> condition)
    {
        if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ButtonMenuOption button)
        {
            button.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ToggleMenuOption toggle)
        {
            toggle.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is SliderMenuButton slider)
        {
            slider.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ChoiceMenuOption choice)
        {
            choice.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is SubmenuMenuOption submenu)
        {
            submenu.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is TextMenuOption textItem)
        {
            textItem.VisibilityCheck = condition;
        }
        return this;
    }

    public IMenuBuilder EnabledWhen(Func<IPlayer, bool> condition)
    {
        if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ButtonMenuOption button)
        {
            button.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ToggleMenuOption toggle)
        {
            toggle.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is SliderMenuButton slider)
        {
            slider.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ChoiceMenuOption choice)
        {
            choice.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is SubmenuMenuOption submenu)
        {
            submenu.VisibilityCheck = condition;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is TextMenuOption textItem)
        {
            textItem.VisibilityCheck = condition;
        }
        return this;
    }

    public IMenuBuilder WithValidation(Func<IPlayer, bool> validation, Action<IPlayer>? onFailed = null)
    {
        if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ButtonMenuOption button)
        {
            button.ValidationCheck = validation;
            button.OnValidationFailed = onFailed;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ToggleMenuOption toggle)
        {
            toggle.ValidationCheck = validation;
            toggle.OnValidationFailed = onFailed;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is AsyncButtonMenuOption asyncButton)
        {
            asyncButton.ValidationCheck = validation;
            asyncButton.OnValidationFailed = onFailed;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is DynamicMenuOption dynamic)
        {
            dynamic.WithValidation(validation, onFailed);
        }
        return this;
    }

    public IMenuBuilder CloseOnSelect()
    {
        if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ButtonMenuOption button)
        {
            button.CloseOnSelect = true;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is ToggleMenuOption toggle)
        {
            toggle.CloseOnSelect = true;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is AsyncButtonMenuOption asyncButton)
        {
            asyncButton.CloseOnSelect = true;
        }
        else if (_menu!.Options.Count > 0 && _menu!.Options[^1] is DynamicMenuOption dynamic)
        {
            dynamic.WithCloseOnSelect(true);
        }
        return this;
    }

    public IMenuBuilder AutoClose(float seconds)
    {
        _menu!.AutoCloseAfter = seconds;
        return this;
    }

    public IMenuBuilder OverrideButtons(Action<IMenuButtonOverrides> configureOverrides)
    {
        configureOverrides(_menu!.ButtonOverrides!);
        return this;
    }

    public IMenuBuilder OverrideSelectButton(params string[] buttonNames)
    {
        _menu!.ButtonOverrides.Select = MenuButtonOverrides.ParseButtons(buttonNames);
        return this;
    }

    public IMenuBuilder OverrideMoveButton(params string[] buttonNames)
    {
        _menu!.ButtonOverrides.Move = MenuButtonOverrides.ParseButtons(buttonNames);
        return this;
    }


    public IMenuBuilder OverrideExitButton(params string[] buttonNames)
    {
        _menu!.ButtonOverrides.Exit = MenuButtonOverrides.ParseButtons(buttonNames);
        return this;
    }

    public IMenuBuilder MaxVisibleItems(int count)
    {
        _menu!.MaxVisibleOptions = Math.Max(1, count);
        return this;
    }

    public IMenuBuilder NoFreeze()
    {
        _menu!.ShouldFreeze = false;
        return this;
    }

    public IMenuBuilder ForceFreeze()
    {
        _menu!.ShouldFreeze = true;
        return this;
    }

    public IMenuBuilder HasSound(bool hasSound)
    {
        _menu!.HasSound = hasSound;
        return this;
    }

    public IMenuBuilder SetColor(Color color)
    {
        _menu!.RenderColor = color;
        return this;
    }
}