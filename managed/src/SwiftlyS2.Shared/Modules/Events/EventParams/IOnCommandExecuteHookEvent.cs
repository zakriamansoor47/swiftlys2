using SwiftlyS2.Shared.Misc;

namespace SwiftlyS2.Shared.Events;

/// <summary>
/// Called when a command is executed.
/// </summary>
public interface IOnCommandExecuteHookEvent {

  /// <summary>
  /// The original command name.
  /// </summary>
  public string OriginalName { get; }

  /// <summary>
  /// The original command arguments.
  /// </summary>
  public string[] OriginalArgs { get; }

  /// <summary>
  /// The command arguments.
  /// </summary>
  public HookMode HookMode { get; }

  /// <summary>
  /// Intercept and modify the command name.
  /// This will modify the command name and stop the following hooks and original function.
  /// </summary>
  /// <param name="name">The name to modify.</param>
  public void SetCommandName(string name);
}