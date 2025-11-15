#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativePlayerManager
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<int, byte> _IsPlayerOnline;

    public unsafe static bool IsPlayerOnline(int playerid)
    {
        var ret = _IsPlayerOnline(playerid);
        return ret == 1;
    }

    private unsafe static delegate* unmanaged<int> _GetPlayerCount;

    public unsafe static int GetPlayerCount()
    {
        var ret = _GetPlayerCount();
        return ret;
    }

    private unsafe static delegate* unmanaged<int> _GetPlayerCap;

    public unsafe static int GetPlayerCap()
    {
        var ret = _GetPlayerCap();
        return ret;
    }

    private unsafe static delegate* unmanaged<int, byte*, int, void> _SendMessage;

    public unsafe static void SendMessage(int kind, string message, int duration)
    {
        byte[] messageBuffer = Encoding.UTF8.GetBytes(message + "\0");
        fixed (byte* messageBufferPtr = messageBuffer)
        {
            _SendMessage(kind, messageBufferPtr, duration);
        }
    }

    private unsafe static delegate* unmanaged<int, byte, void> _ShouldBlockTransmitEntity;

    public unsafe static void ShouldBlockTransmitEntity(int entityidx, bool shouldBlockTransmit)
    {
        _ShouldBlockTransmitEntity(entityidx, shouldBlockTransmit ? (byte)1 : (byte)0);
    }

    private unsafe static delegate* unmanaged<void> _ClearAllBlockedTransmitEntity;

    public unsafe static void ClearAllBlockedTransmitEntity()
    {
        _ClearAllBlockedTransmitEntity();
    }
}