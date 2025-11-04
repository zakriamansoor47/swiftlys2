using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using SwiftlyS2.Core.Menu.Options;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace SwiftlyS2.Core.Menus;

internal partial class Menu : IMenu
{
    public string Title { get; set; } = "Menu";

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
    private ConcurrentDictionary<string, int> ScrollOffsets { get; set; } = new();
    private ConcurrentDictionary<string, int> ScrollCallCounts { get; set; } = new();
    internal ISwiftlyCore _Core { get; set; }
    public bool HasSound { get; set; } = true;
    public bool RenderOntick { get; set; } = false;
    private bool Initialized { get; set; } = false;
    public MenuVerticalScrollStyle VerticalScrollStyle { get; set; } = MenuVerticalScrollStyle.CenterFixed;
    public MenuHorizontalStyle? HorizontalStyle { get; set; } = null;
    public bool RequiresTickRendering => RenderOntick ||
        HorizontalStyle?.OverflowStyle == MenuHorizontalOverflowStyle.ScrollLeftFade ||
        HorizontalStyle?.OverflowStyle == MenuHorizontalOverflowStyle.ScrollRightFade ||
        HorizontalStyle?.OverflowStyle == MenuHorizontalOverflowStyle.ScrollLeftLoop ||
        HorizontalStyle?.OverflowStyle == MenuHorizontalOverflowStyle.ScrollRightLoop ||
        Options.Any(opt =>
            opt.OverflowStyle?.OverflowStyle == MenuHorizontalOverflowStyle.ScrollLeftFade ||
            opt.OverflowStyle?.OverflowStyle == MenuHorizontalOverflowStyle.ScrollRightFade ||
            opt.OverflowStyle?.OverflowStyle == MenuHorizontalOverflowStyle.ScrollLeftLoop ||
            opt.OverflowStyle?.OverflowStyle == MenuHorizontalOverflowStyle.ScrollRightLoop);

    public void Close(IPlayer player)
    {
        NativePlayer.ClearCenterMenuRender(player.PlayerID);
        OnClose?.Invoke(player);
        if (ShouldFreeze == true) SetFreezeState(player, false);

        PlayersWithMenuOpen.Remove(player);

        if (Initialized && PlayersWithMenuOpen.Count == 0 && RequiresTickRendering)
        {
            Initialized = false;
            _Core.Event.OnTick -= OnTickRender;
        }

        ScrollOffsets.Clear();
        ScrollCallCounts.Clear();
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

        if (!Initialized && RequiresTickRendering)
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

        ScrollOffsets.Clear();
        ScrollCallCounts.Clear();
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

    internal string ApplyHorizontalStyle(string text, MenuHorizontalStyle? overflowStyle = null)
    {
        var activeStyle = overflowStyle ?? HorizontalStyle;

        if (activeStyle == null || string.IsNullOrEmpty(text))
        {
            return text;
        }

        var plainText = StripHtmlTags(text);
        if (Helper.EstimateTextWidth(plainText) <= activeStyle.Value.MaxWidth)
        {
            return text;
        }

        return activeStyle.Value.OverflowStyle switch
        {
            MenuHorizontalOverflowStyle.TruncateEnd => TruncateTextEnd(text, activeStyle.Value.MaxWidth),
            MenuHorizontalOverflowStyle.TruncateBothEnds => TruncateTextBothEnds(text, activeStyle.Value.MaxWidth),
            MenuHorizontalOverflowStyle.ScrollLeftFade => ScrollTextWithFade(text, activeStyle.Value.MaxWidth, true, activeStyle),
            MenuHorizontalOverflowStyle.ScrollRightFade => ScrollTextWithFade(text, activeStyle.Value.MaxWidth, false, activeStyle),
            MenuHorizontalOverflowStyle.ScrollLeftLoop => ScrollTextWithLoop($"{text.TrimEnd()} ", activeStyle.Value.MaxWidth, true, activeStyle),
            MenuHorizontalOverflowStyle.ScrollRightLoop => ScrollTextWithLoop($" {text.TrimStart()}", activeStyle.Value.MaxWidth, false, activeStyle),
            _ => text
        };
    }

    private string ScrollTextWithFade(string text, float maxWidth, bool scrollLeft, MenuHorizontalStyle? style = null)
    {
        // Prepare scroll data and validate
        var (plainChars, segments, targetCharCount) = PrepareScrollData(text, maxWidth);
        if (plainChars is null)
        {
            return text;
        }
        if (targetCharCount == 0)
        {
            return string.Empty;
        }

        // Update scroll offset (allow scrolling beyond end for complete fade-out)
        var offset = UpdateScrollOffset(StripHtmlTags(text), scrollLeft, plainChars.Length + 1, style);

        // Calculate visible character range
        var (skipStart, skipEnd) = scrollLeft
            ? (offset, Math.Max(0, plainChars.Length - offset - targetCharCount))
            : (Math.Max(0, plainChars.Length - targetCharCount - offset), offset);

        // Build output with proper HTML tag tracking
        StringBuilder result = new();
        List<string> outputTags = [], activeTags = [];
        var (charIdx, started) = (0, false);

        foreach (var (content, isTag) in segments)
        {
            if (isTag)
            {
                // Track active opening and closing tags
                UpdateTagState(content, activeTags);

                // Output tags within visible window
                if (started)
                {
                    result.Append(content);
                    ProcessOpenTag(content, outputTags);
                }
            }
            else
            {
                // Process characters within scroll window
                foreach (var ch in content)
                {
                    if (charIdx >= skipStart && charIdx < plainChars.Length - skipEnd)
                    {
                        // Apply active tags at start of output
                        if (!started)
                        {
                            started = true;
                            activeTags.ForEach(tag => { result.Append(tag); ProcessOpenTag(tag, outputTags); });
                        }
                        result.Append(ch);
                    }
                    charIdx++;
                }
            }
        }

        CloseOpenTags(result, outputTags);
        return result.ToString();
    }

    private string ScrollTextWithLoop(string text, float maxWidth, bool scrollLeft, MenuHorizontalStyle? style = null)
    {
        // Prepare scroll data and validate
        var (plainChars, segments, targetCharCount) = PrepareScrollData(text, maxWidth);
        if (plainChars is null)
        {
            return text;
        }
        if (targetCharCount == 0)
        {
            return string.Empty;
        }

        // Update scroll offset for circular wrapping
        var offset = UpdateScrollOffset(StripHtmlTags(text), scrollLeft, plainChars.Length, style);

        // Build character-to-tags mapping for circular access
        Dictionary<int, List<string>> charToActiveTags = [];
        List<string> currentActiveTags = [];
        var currentCharIdx = 0;

        foreach (var (content, isTag) in segments)
        {
            if (isTag)
            {
                // Track active opening and closing tags
                UpdateTagState(content, currentActiveTags);
            }
            else
            {
                // Map each character to its active tags
                foreach (var ch in content)
                {
                    charToActiveTags[currentCharIdx] = [.. currentActiveTags];
                    currentCharIdx++;
                }
            }
        }

        // Build output in circular order with dynamic tag management
        StringBuilder result = new();
        List<string> outputTags = [];
        List<string>? previousTags = null;

        for (int i = 0; i < targetCharCount; i++)
        {
            // Calculate circular character index
            var charIndex = scrollLeft
                ? (offset + i) % plainChars.Length
                : (plainChars.Length - offset + i) % plainChars.Length;
            var currentTags = charToActiveTags.GetValueOrDefault(charIndex, []);

            // Close tags that are no longer active
            if (previousTags is not null)
            {
                for (int j = previousTags.Count - 1; j >= 0; j--)
                {
                    if (!currentTags.Contains(previousTags[j]))
                    {
                        var prevTagName = previousTags[j][1..^1].Split(' ')[0];
                        result.Append($"</{prevTagName}>");
                        var idx = outputTags.FindLastIndex(t => t.Equals(prevTagName, StringComparison.OrdinalIgnoreCase));
                        if (idx >= 0)
                        {
                            outputTags.RemoveAt(idx);
                        }
                    }
                }
            }

            // Open new tags that are now active
            foreach (var tag in currentTags)
            {
                if (previousTags is null || !previousTags.Contains(tag))
                {
                    result.Append(tag);
                    var tagName = tag[1..^1].Split(' ')[0];
                    outputTags.Add(tagName);
                }
            }

            result.Append(plainChars[charIndex]);
            previousTags = currentTags;
        }

        CloseOpenTags(result, outputTags);
        return result.ToString();
    }

    private static string TruncateTextEnd(string text, float maxWidth, string suffix = "...")
    {
        // Reserve space for suffix
        var targetWidth = maxWidth - Helper.EstimateTextWidth(suffix);
        if (targetWidth <= 0)
        {
            return suffix;
        }

        var segments = ParseHtmlSegments(text);
        StringBuilder result = new();
        List<string> openTags = [];
        var (currentWidth, reachedLimit) = (0f, false);

        foreach (var (content, isTag) in segments)
        {
            switch (isTag, reachedLimit)
            {
                // Preserve HTML tags before reaching limit
                case (true, false):
                    result.Append(content);
                    ProcessOpenTag(content, openTags);
                    break;

                // Process plain text characters until width limit
                case (false, false):
                    foreach (var ch in content)
                    {
                        var charWidth = Helper.GetCharWidth(ch);
                        if (currentWidth + charWidth > targetWidth)
                        {
                            reachedLimit = true;
                            break;
                        }
                        result.Append(ch);
                        currentWidth += charWidth;
                    }
                    break;
            }
        }

        if (reachedLimit)
        {
            result.Append(suffix);
        }

        CloseOpenTags(result, openTags);
        return result.ToString();
    }

    private static string TruncateTextBothEnds(string text, float maxWidth)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        // Check if text fits without truncation
        var plainText = StripHtmlTags(text);
        if (Helper.EstimateTextWidth(plainText) <= maxWidth)
        {
            return text;
        }

        // Extract all plain text characters from segments
        var segments = ParseHtmlSegments(text);
        var plainChars = segments
            .Where(s => !s.IsTag)
            .SelectMany(s => s.Content)
            .ToArray();

        if (plainChars.Length == 0)
        {
            return text;
        }

        // Calculate how many characters can fit
        var targetCharCount = CalculateTargetCharCount(plainChars, maxWidth);
        if (targetCharCount == 0)
        {
            return string.Empty;
        }

        // Calculate range to keep from middle
        var skipFromStart = Math.Max(0, (plainChars.Length - targetCharCount) / 2);
        var skipFromEnd = plainChars.Length - skipFromStart - targetCharCount;

        StringBuilder result = new();
        List<string> outputOpenTags = [];
        List<string> pendingOpenTags = [];
        var (plainCharIndex, hasStartedOutput) = (0, false);

        foreach (var (content, isTag) in segments)
        {
            switch (isTag, hasStartedOutput)
            {
                // Process tags after output has started
                case (true, true):
                    result.Append(content);
                    ProcessOpenTag(content, outputOpenTags);
                    break;

                // Queue opening tags before output starts
                case (true, false) when !content.StartsWith("</") && !content.StartsWith("<!") && !content.EndsWith("/>"):
                    pendingOpenTags.Add(content);
                    break;

                // Process plain text, keeping only middle portion
                case (false, _):
                    foreach (var ch in content)
                    {
                        if (plainCharIndex >= skipFromStart && plainCharIndex < plainChars.Length - skipFromEnd)
                        {
                            // Start output and apply pending tags
                            if (!hasStartedOutput)
                            {
                                hasStartedOutput = true;
                                pendingOpenTags.ForEach(tag =>
                                {
                                    result.Append(tag);
                                    ProcessOpenTag(tag, outputOpenTags);
                                });
                            }
                            result.Append(ch);
                        }
                        plainCharIndex++;
                    }
                    break;
            }
        }

        CloseOpenTags(result, outputOpenTags);
        return result.ToString();
    }
}

internal partial class Menu
{
    [GeneratedRegex("<.*?>")]
    private static partial Regex HtmlTagRegex();

    private static string StripHtmlTags(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return HtmlTagRegex().Replace(text, string.Empty);
    }

    private static List<(string Content, bool IsTag)> ParseHtmlSegments(string text)
    {
        var tagMatches = HtmlTagRegex().Matches(text);
        if (tagMatches.Count == 0)
        {
            return [(text, false)];
        }

        List<(string Content, bool IsTag)> segments = [];
        var currentIndex = 0;

        foreach (Match match in tagMatches)
        {
            if (match.Index > currentIndex)
            {
                segments.Add((text[currentIndex..match.Index], false));
            }
            segments.Add((match.Value, true));
            currentIndex = match.Index + match.Length;
        }

        if (currentIndex < text.Length)
        {
            segments.Add((text[currentIndex..], false));
        }

        return segments;
    }

    private static void ProcessOpenTag(string tag, List<string> openTags)
    {
        var tagName = tag switch
        {
            ['<', '/', .. var rest] => new string(rest).TrimEnd('>').Split(' ', 2)[0],
            ['<', '!', ..] => null,
            [.. var chars] when chars[^1] == '/' && chars[^2] == '>' => null,
            ['<', .. var rest] => new string(rest).TrimEnd('>').Split(' ', 2)[0],
            _ => null
        };

        if (tagName is null)
        {
            return;
        }

        if (tag.StartsWith("</"))
        {
            var index = openTags.FindLastIndex(t => t.Equals(tagName, StringComparison.OrdinalIgnoreCase));
            if (index >= 0) openTags.RemoveAt(index);
        }
        else
        {
            openTags.Add(tagName);
        }
    }

    private static void CloseOpenTags(StringBuilder result, List<string> openTags)
    {
        openTags.AsEnumerable().Reverse().ToList().ForEach(tag => result.Append($"</{tag}>"));
    }

    private static int CalculateTargetCharCount(ReadOnlySpan<char> plainChars, float maxWidth)
    {
        var currentWidth = 0f;
        var count = 0;

        foreach (var ch in plainChars)
        {
            var charWidth = Helper.GetCharWidth(ch);
            if (currentWidth + charWidth > maxWidth) break;
            currentWidth += charWidth;
            count++;
        }

        return count;
    }

    private int UpdateScrollOffset(string plainText, bool scrollLeft, int wrapLength, MenuHorizontalStyle? style)
    {
        var key = $"{plainText}_{scrollLeft}";
        ScrollOffsets.TryAdd(key, 0);
        ScrollCallCounts.TryAdd(key, 0);

        var ticksPerScroll = style?.TicksPerScroll ?? HorizontalStyle?.TicksPerScroll ?? 16;
        if (++ScrollCallCounts[key] >= ticksPerScroll)
        {
            ScrollCallCounts[key] = 0;
            ScrollOffsets[key] = (ScrollOffsets[key] + 1) % wrapLength;
        }

        return ScrollOffsets[key];
    }

    private static void UpdateTagState(string content, List<string> activeTags)
    {
        if (!content.StartsWith("</") && !content.StartsWith("<!") && !content.EndsWith("/>"))
        {
            activeTags.Add(content);
        }
        else if (content.StartsWith("</"))
        {
            var tagName = content[2..^1].Split(' ')[0];
            var index = activeTags.FindLastIndex(t => t[1..^1].Split(' ')[0].Equals(tagName, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                activeTags.RemoveAt(index);
            }
        }
    }

    private static (char[]? PlainChars, List<(string Content, bool IsTag)> Segments, int TargetCharCount) PrepareScrollData(string text, float maxWidth)
    {
        var plainText = StripHtmlTags(text);
        if (Helper.EstimateTextWidth(plainText) <= maxWidth)
        {
            return (null, [], 0);
        }

        var segments = ParseHtmlSegments(text);
        var plainChars = segments.Where(s => !s.IsTag).SelectMany(s => s.Content).ToArray();

        if (plainChars.Length == 0)
        {
            return (null, segments, 0);
        }

        var targetCharCount = CalculateTargetCharCount(plainChars, maxWidth);
        return (plainChars, segments, targetCharCount);
    }
}