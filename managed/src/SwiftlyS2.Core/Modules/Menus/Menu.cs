using System.Collections.Concurrent;
using System.Text;
using SwiftlyS2.Core.Menu.Options;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace SwiftlyS2.Core.Menus;

internal class Menu : IMenu
{
    public string Title { get; set; } = "";

    public List<IOption> Options { get; set; } = new();

    public IMenu? Parent { get; set; }
    public ConcurrentDictionary<IPlayer, CancellationTokenSource?> AutoCloseCancelTokens { get; set; } = new();
    public IMenuButtonOverrides? ButtonOverrides { get; set; } = new MenuButtonOverrides();
    public int MaxVisibleOptions { get; set; }
    public bool? ShouldFreeze { get; set; } = false;
    public bool? CloseOnSelect { get; set; } = false;
    public Color RenderColor { get; set; } = new(255, 255, 255, 255);

    public IMenuManager MenuManager { get; set; }

    public float AutoCloseAfter { get; set; } = 0.0f;
    public IMenuBuilder Builder => new MenuBuilder().SetMenu(this);

    public event Action<IPlayer>? OnOpen;
    public event Action<IPlayer>? OnClose;
    public event Action<IPlayer>? OnMove;
    public event Action<IPlayer, IOption>? OnItemSelected;
    public event Action<IPlayer, IOption>? OnItemHovered;
    public event Action<IPlayer>? BeforeRender;
    public event Action<IPlayer>? AfterRender;

    private ConcurrentDictionary<IPlayer, string> RenderedText { get; set; } = new();
    private ConcurrentDictionary<IPlayer, int> SelectedIndex { get; set; } = new();
    private List<IPlayer> PlayersWithMenuOpen { get; set; } = new();
    internal ISwiftlyCore _Core { get; set; }
    public bool HasSound { get; set; } = true;
    public bool RenderOntick { get; set; } = false;
    private bool Initialized { get; set; } = false;
    public MenuVerticalScrollStyle VerticalScrollStyle { get; set; } = MenuVerticalScrollStyle.CenterFixed;
    public MenuHorizontalStyle? HorizontalStyle { get; set; } = null;

    public void Close(IPlayer player)
    {
        NativePlayer.ClearCenterMenuRender(player.PlayerID);
        OnClose?.Invoke(player);
        if (ShouldFreeze == true) SetFreezeState(player, false);

        PlayersWithMenuOpen.Remove(player);

        if (Initialized && PlayersWithMenuOpen.Count == 0 && RenderOntick)
        {
            Initialized = false;
            _Core.Event.OnTick -= OnTickRender;
        }
    }

    public void MoveSelection(IPlayer player, int offset)
    {
        if (!SelectedIndex.ContainsKey(player))
        {
            SelectedIndex[player] = 0;
        }

        SelectedIndex[player] += offset;
        if (SelectedIndex[player] < 0) SelectedIndex[player] = -SelectedIndex[player] % Options.Count;
        if (SelectedIndex[player] >= Options.Count) SelectedIndex[player] %= Options.Count;

        while (!Options[SelectedIndex[player]].CanInteract(player))
        {
            SelectedIndex[player]++;
            if (SelectedIndex[player] < 0) SelectedIndex[player] = -SelectedIndex[player] % Options.Count;
            if (SelectedIndex[player] >= Options.Count) SelectedIndex[player] %= Options.Count;
        }

        if (SelectedIndex[player] < 0) SelectedIndex[player] = -SelectedIndex[player] % Options.Count;
        if (SelectedIndex[player] >= Options.Count) SelectedIndex[player] %= Options.Count;

        OnMove?.Invoke(player);
        OnItemHovered?.Invoke(player, Options[SelectedIndex[player]]);

        Rerender(player);
    }

    public void Rerender(IPlayer player)
    {
        BeforeRender?.Invoke(player);

        var visibleOptions = Options.Where(option => option.ShouldShow(player)).ToList();

        var maxVisibleOptions = MaxVisibleOptions;
        var totalOptions = visibleOptions.Count;

        if (SelectedIndex[player] < 0 && totalOptions > 0)
        {
            for (int i = 0; i < totalOptions; i++)
            {
                if (IsOptionSelectable(visibleOptions[i]))
                {
                    SelectedIndex[player] = i;
                    break;
                }
            }
        }

        var html = new StringBuilder();

        html.Append($"<font class='fontSize-m' color='{RenderColor.ToHex(true)}'>{Title}</font>");

        if (totalOptions > maxVisibleOptions)
        {
            html.Append($"<font class='fontSize-s' color='#FFFFFF'> [{SelectedIndex[player] + 1}/{totalOptions}]</font>");
        }

        html.Append("<font color='#FFFFFF' class='fontSize-sm'><br>");

        if (totalOptions > 0)
        {
            if (totalOptions > maxVisibleOptions)
            {
                var selectedIdx = SelectedIndex[player];
                var halfVisible = maxVisibleOptions / 2;

                var (startIndex, arrowPosition) = VerticalScrollStyle switch
                {
                    MenuVerticalScrollStyle.WaitingCenter when selectedIdx < halfVisible
                        => (0, selectedIdx),                                                                        // WaitingCenter: (Near top) start from 0, arrow at selected
                    MenuVerticalScrollStyle.WaitingCenter when selectedIdx >= totalOptions - halfVisible
                        => (totalOptions - maxVisibleOptions, maxVisibleOptions - (totalOptions - selectedIdx)),    // WaitingCenter: (Near bottom) start from end-visible, arrow at bottom area
                    MenuVerticalScrollStyle.WaitingCenter
                        => (selectedIdx - halfVisible, halfVisible),                                                // WaitingCenter: (Middle) start from selected-half, arrow at center

                    MenuVerticalScrollStyle.LinearScroll when maxVisibleOptions == 1
                        => (selectedIdx, 0),                                                                        // LinearScroll: single visible, start from selected, arrow at top

                    MenuVerticalScrollStyle.LinearScroll when selectedIdx < maxVisibleOptions - 1
                        => (0, selectedIdx),                                                                        // LinearScroll: (Near top) start from 0, arrow at selected
                    MenuVerticalScrollStyle.LinearScroll when selectedIdx >= totalOptions - (maxVisibleOptions - 1)
                        => (totalOptions - maxVisibleOptions, maxVisibleOptions - (totalOptions - selectedIdx)),    // LinearScroll: (Near bottom) start from end-visible, arrow at bottom area
                    MenuVerticalScrollStyle.LinearScroll
                        => (selectedIdx - (maxVisibleOptions - 1), maxVisibleOptions - 1),                          // LinearScroll: (Middle) start from selected-visible+1, arrow at bottom

                    _
                        => (-1, halfVisible)    // CenterFixed: no scroll, arrow at middle
                };

                for (int i = 0; i < maxVisibleOptions; i++)
                {
                    var (actualIndex, isSelected) = VerticalScrollStyle == MenuVerticalScrollStyle.CenterFixed
                        ? ((selectedIdx + i - halfVisible + totalOptions) % totalOptions, i == halfVisible) // CenterFixed: circular wrap, arrow at center
                        : (startIndex + i, i == arrowPosition);                                             // WaitingCenter / LinearScroll: linear offset, arrow at position

                    var option = visibleOptions[actualIndex];
                    var arrowSizeClass = MenuSizeHelper.GetSizeClass(option.GetTextSize());

                    if (isSelected)
                    {
                        html.Append($"<font color='{RenderColor.ToHex(true)}' class='{arrowSizeClass}'>{MenuManager.Settings.NavigationPrefix} </font>");
                    }
                    else
                    {
                        html.Append("\u00A0\u00A0\u00A0 ");
                    }

                    html.Append(option.GetDisplayText(player));

                    html.Append("<br>");
                }
            }
            else
            {
                for (int i = 0; i < totalOptions; i++)
                {
                    var option = visibleOptions[i];
                    var isSelected = i == SelectedIndex[player];
                    var arrowSizeClass = MenuSizeHelper.GetSizeClass(option.GetTextSize());

                    if (isSelected)
                    {
                        html.Append($"<font color='{RenderColor.ToHex(true)}' class='{arrowSizeClass}'>{MenuManager.Settings.NavigationPrefix} </font>");
                    }
                    else
                    {
                        html.Append("\u00A0\u00A0\u00A0 ");
                    }

                    html.Append(option.GetDisplayText(player));

                    html.Append("<br>");
                }
            }
        }

        html.Append("<br>");

        html.Append(BuildFooter());

        html.Append("</font>");

        RenderedText[player] = html.ToString();

        NativePlayer.SetCenterMenuRender(player.PlayerID, RenderedText[player]);

        AfterRender?.Invoke(player);
    }

    private string BuildFooter()
    {
        var footer = new StringBuilder("<font color='#ffffff' class='fontSize-s'>");

        var isWASD = MenuManager.Settings.InputMode == "wasd";
        var selectDisplay = isWASD ? "D" : MenuManager.Settings.ButtonsUse.ToUpper();
        var scrollDisplay = isWASD ? "W/S" : MenuManager.Settings.ButtonsScroll.ToUpper();
        var exitDisplay = isWASD ? "A" : MenuManager.Settings.ButtonsExit.ToUpper();

        footer.Append($"Move: <font color='{RenderColor.ToHex(true)}'>{scrollDisplay}</font>");

        footer.Append($" | Use: <font color='{RenderColor.ToHex(true)}'>{selectDisplay}</font>");

        footer.Append($" | Exit: <font color='{RenderColor.ToHex(true)}'>{exitDisplay}</font>");

        footer.Append("</font><br>");
        return footer.ToString();
    }

    private void OnTickRender()
    {
        foreach (var p in PlayersWithMenuOpen)
        {
            Rerender(p);
        }
    }

    public void Show(IPlayer player)
    {
        if (!SelectedIndex.TryAdd(player, 0))
        {
            SelectedIndex[player] = 0;
        }

        if (!PlayersWithMenuOpen.Contains(player))
        {
            PlayersWithMenuOpen.Add(player);
        }

        if (!Initialized && RenderOntick)
        {
            Initialized = true;
            _Core.Event.OnTick += OnTickRender;
        }

        Rerender(player);
        OnOpen?.Invoke(player);
        if (ShouldFreeze == true) SetFreezeState(player, true);

        if (AutoCloseAfter != 0f)
        {
            AutoCloseCancelTokens[player] = _Core.Scheduler.DelayBySeconds(AutoCloseAfter, () =>
            {
                Close(player);
            });
        }
    }

    public void UseSelection(IPlayer player)
    {
        var selectedOption = Options[SelectedIndex[player]];
        OnItemSelected?.Invoke(player, selectedOption);

        switch (selectedOption)
        {
            case ButtonMenuOption buttonOption:
                {
                    if (buttonOption.ValidationCheck != null && !buttonOption.ValidationCheck(player))
                    {
                        buttonOption.OnValidationFailed?.Invoke(player);
                    }
                    buttonOption.OnClick?.Invoke(player);
                    buttonOption.OnClickWithOption?.Invoke(player, buttonOption);
                    if (buttonOption.CloseOnSelect)
                    {
                        MenuManager.CloseMenuForPlayer(player);
                    }
                    break;
                }
            case AsyncButtonMenuOption asyncButton:
                {
                    if (asyncButton.ValidationCheck != null && !asyncButton.ValidationCheck(player))
                    {
                        asyncButton.OnValidationFailed?.Invoke(player);
                    }
                    asyncButton.IsLoading = true;
                    asyncButton.SetLoadingText("Processing...");
                    Rerender(player);
                    var closeAfter = asyncButton.CloseOnSelect;
                    Task.Run(async () =>
                    {
                        try
                        {
                            await asyncButton.ExecuteAsync(player, "Processing...");
                        }
                        finally
                        {
                            asyncButton.IsLoading = false;
                            Rerender(player);

                            if (closeAfter && player.IsValid)
                            {
                                MenuManager.CloseMenuForPlayer(player);
                            }
                        }
                    });
                    break;
                }
            case ToggleMenuOption toggle:
                {
                    toggle.Toggle(player);
                    if (toggle.CloseOnSelect)
                    {
                        MenuManager.CloseMenuForPlayer(player);
                    }
                    else
                    {
                        Rerender(player);
                    }
                    break;
                }

            case SubmenuMenuOption submenu:
                var subMenu = submenu.GetSubmenu();
                if (subMenu != null)
                {
                    subMenu.Parent = this;
                    MenuManager.OpenMenu(player, subMenu);
                }
                break;
        }
    }

    public void UseSlideOption(IPlayer player, bool isRight)
    {
        var selectedOption = Options[SelectedIndex[player]];

        switch (selectedOption)
        {
            case SliderMenuButton slider:
                if (isRight) slider.Increase(player);
                else slider.Decrease(player);
                break;

            case ChoiceMenuOption choice:
                if (isRight) choice.Next(player);
                else choice.Previous(player);
                break;
        }

        Rerender(player);
    }

    public bool IsOptionSlider(IPlayer player)
    {
        var option = Options[SelectedIndex[player]];
        return option is SliderMenuButton || option is ChoiceMenuOption;
    }

    public bool IsCurrentOptionSelectable(IPlayer player)
    {
        var option = Options[SelectedIndex[player]];
        return IsOptionSelectable(option);
    }

    public bool IsOptionSelectable(IOption option)
    {
        return option is ButtonMenuOption ||
               option is ToggleMenuOption ||
               option is SliderMenuButton ||
               option is SubmenuMenuOption ||
               option is AsyncButtonMenuOption ||
               (option is DynamicMenuOption dynamic && dynamic.CanInteract(null!));
    }

    public void SetFreezeState(IPlayer player, bool freeze)
    {
        if (!player.IsValid || player.IsFakeClient) return;

        var pawn = player.PlayerPawn;
        if (pawn == null || !pawn.IsValid) return;

        var moveType = freeze ? MoveType_t.MOVETYPE_NONE : MoveType_t.MOVETYPE_WALK;
        pawn.MoveType = moveType;
        pawn.ActualMoveType = moveType;
        pawn.MoveTypeUpdated();
    }
}