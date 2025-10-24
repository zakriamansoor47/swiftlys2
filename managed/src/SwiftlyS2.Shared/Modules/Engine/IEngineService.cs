using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Shared.Services;

public interface IEngineService
{
    /// <summary>
    /// The IP address of the server.
    /// </summary>
    string ServerIP { get; }

    /// <summary>
    /// Gets the map that the server is running
    /// </summary>
    [Obsolete("Use GlobalVars.MapName instead.")]
    string Map { get; }

    /// <summary>
    /// Gets a reference to the global variables structure.
    /// </summary>
    ref CGlobalVars GlobalVars { get; }

    /// <summary>
    /// Determines whether the specified map string represents a valid map in server files.
    /// </summary>
    /// <param name="map">The map string to validate. It also supports Workshop ID.</param>
    /// <returns>true if the map is valid; otherwise, false.</returns>
    bool IsMapValid(string map);

    /// <summary>
    /// Gets the maximum number of players allowed in the game.
    /// </summary>
    [Obsolete("Use GlobalVars.MaxClients instead.")]
    int MaxPlayers { get; }

    /// <summary>
    /// Executes the specified command string in the current context.
    /// </summary>
    /// <param name="command">The command to execute. Cannot be null or empty.</param>
    void ExecuteCommand(string command);

    /// <summary>
    /// Executes the specified command string in the current context.
    /// </summary>
    /// <param name="command">The command to execute. Cannot be null or empty.</param>
    /// <param name="bufferCallback">The callback to receive the output of the command.</param>
    void ExecuteCommandWithBuffer(string command, Action<string> bufferCallback);

    /// <summary>
    /// The time since the server started.
    /// </summary>
    [Obsolete("Use GlobalVars.CurrentTime instead.")]
    float CurrentTime { get; }

    /// <summary>
    /// The number of simulation ticks that have occurred since the server started.
    /// </summary>
    [Obsolete("Use GlobalVars.TickCount instead.")]
    int TickCount { get; }

    /// <summary>
    /// Find a game system by name.
    /// </summary>
    /// <param name="name">The name of the game system.</param>
    /// <returns>The game system handle. Null if not found.</returns>
    nint? FindGameSystemByName(string name);
}