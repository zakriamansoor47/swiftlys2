using SwiftlyS2.Shared;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared.Services;
using SwiftlyS2.Core.Extensions;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Services;

internal class EngineService : IEngineService
{
    private readonly CommandTrackerManager _commandTrackedManager;

    public EngineService(CommandTrackerManager commandTrackedManager)
    {
        this._commandTrackedManager = commandTrackedManager;
    }

    public string ServerIP => NativeEngineHelpers.GetServerIP();

    public ref CGlobalVars GlobalVars => ref NativeEngineHelpers.GetGlobalVars().AsRef<CGlobalVars>();

    public string Map => GlobalVars.MapName.ToString() ?? string.Empty;

    public int MaxPlayers => GlobalVars.MaxClients;

    public float CurrentTime => GlobalVars.CurrentTime;

    public int TickCount => GlobalVars.TickCount;

    public void ExecuteCommand(string command)
    {
        NativeEngineHelpers.ExecuteCommand(command);
    }

    public void ExecuteCommandWithBuffer(string command, Action<string> bufferCallback)
    {
        _commandTrackedManager.EnqueueCommand(bufferCallback);
        NativeEngineHelpers.ExecuteCommand($"^wb^{command}");
    }

    public bool IsMapValid(string map)
    {
        return NativeEngineHelpers.IsMapValid(map);
    }

    public nint? FindGameSystemByName(string name)
    {
        var handle = NativeEngineHelpers.FindGameSystemByName(name);
        return handle == nint.Zero ? null : handle;
    }
}