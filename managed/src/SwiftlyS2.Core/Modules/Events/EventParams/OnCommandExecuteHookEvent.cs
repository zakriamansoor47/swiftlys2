using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Events;

namespace SwiftlyS2.Core.Events;

internal class OnCommandExecuteHookEvent : IOnCommandExecuteHookEvent
{
  public required string OriginalName { get; init; }
  public required string[] OriginalArgs { get; init; }
  public string CommandName { get; set; } = string.Empty;

  public required HookMode HookMode { get; init; }

  public bool Intercepted { get; set; } = false;

  public void SetCommandName(string name) {
    if (HookMode == HookMode.Post) return;
    CommandName = name;
    Intercepted = true;
  }
}