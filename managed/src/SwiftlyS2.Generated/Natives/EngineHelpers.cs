#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeEngineHelpers
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<byte*, int> _GetIP;

    public unsafe static string GetIP()
    {
        var ret = _GetIP(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetIP(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte> _IsMapValid;

    /// <summary>
    /// it can be map name, or workshop id
    /// </summary>
    public unsafe static bool IsMapValid(string map_name)
    {
        byte[] map_nameBuffer = Encoding.UTF8.GetBytes(map_name + "\0");
        fixed (byte* map_nameBufferPtr = map_nameBuffer)
        {
            var ret = _IsMapValid(map_nameBufferPtr);
            return ret == 1;
        }
    }

    private unsafe static delegate* unmanaged<byte*, void> _ExecuteCommand;

    public unsafe static void ExecuteCommand(string command)
    {
        byte[] commandBuffer = Encoding.UTF8.GetBytes(command + "\0");
        fixed (byte* commandBufferPtr = commandBuffer)
        {
            _ExecuteCommand(commandBufferPtr);
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint> _FindGameSystemByName;

    public unsafe static nint FindGameSystemByName(string name)
    {
        byte[] nameBuffer = Encoding.UTF8.GetBytes(name + "\0");
        fixed (byte* nameBufferPtr = nameBuffer)
        {
            var ret = _FindGameSystemByName(nameBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, void> _SendMessageToConsole;

    public unsafe static void SendMessageToConsole(string msg)
    {
        byte[] msgBuffer = Encoding.UTF8.GetBytes(msg + "\0");
        fixed (byte* msgBufferPtr = msgBuffer)
        {
            _SendMessageToConsole(msgBufferPtr);
        }
    }

    private unsafe static delegate* unmanaged<nint> _GetTraceManager;

    public unsafe static nint GetTraceManager()
    {
        var ret = _GetTraceManager();
        return ret;
    }

    private unsafe static delegate* unmanaged<byte*, int> _GetCurrentGame;

    public unsafe static string GetCurrentGame()
    {
        var ret = _GetCurrentGame(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetCurrentGame(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }

    private unsafe static delegate* unmanaged<byte*, int> _GetNativeVersion;

    public unsafe static string GetNativeVersion()
    {
        var ret = _GetNativeVersion(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetNativeVersion(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }

    private unsafe static delegate* unmanaged<byte*, int> _GetMenuSettings;

    public unsafe static string GetMenuSettings()
    {
        var ret = _GetMenuSettings(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetMenuSettings(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }

    private unsafe static delegate* unmanaged<nint> _GetGlobalVars;

    public unsafe static nint GetGlobalVars()
    {
        var ret = _GetGlobalVars();
        return ret;
    }

    private unsafe static delegate* unmanaged<byte*, int> _GetCSGODirectoryPath;

    public unsafe static string GetCSGODirectoryPath()
    {
        var ret = _GetCSGODirectoryPath(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetCSGODirectoryPath(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }

    private unsafe static delegate* unmanaged<byte*, int> _GetGameDirectoryPath;

    public unsafe static string GetGameDirectoryPath()
    {
        var ret = _GetGameDirectoryPath(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetGameDirectoryPath(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }
}