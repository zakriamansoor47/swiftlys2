#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeDatabase
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<byte*, int> _GetDefaultConnection;

    public unsafe static string GetDefaultConnection()
    {
        var ret = _GetDefaultConnection(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetDefaultConnection(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }

    private unsafe static delegate* unmanaged<byte*, int> _GetDefaultConnectionCredentials;

    public unsafe static string GetDefaultConnectionCredentials()
    {
        var ret = _GetDefaultConnectionCredentials(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetDefaultConnectionCredentials(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, int> _GetCredentials;

    public unsafe static string GetCredentials(string connectionName)
    {
        byte[] connectionNameBuffer = Encoding.UTF8.GetBytes(connectionName + "\0");
        fixed (byte* connectionNameBufferPtr = connectionNameBuffer)
        {
            var ret = _GetCredentials(null, connectionNameBufferPtr);
            var retBuffer = new byte[ret + 1];
            fixed (byte* retBufferPtr = retBuffer)
            {
                ret = _GetCredentials(retBufferPtr, connectionNameBufferPtr);
                return Encoding.UTF8.GetString(retBufferPtr, ret);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte> _ConnectionExists;

    public unsafe static bool ConnectionExists(string connectionName)
    {
        byte[] connectionNameBuffer = Encoding.UTF8.GetBytes(connectionName + "\0");
        fixed (byte* connectionNameBufferPtr = connectionNameBuffer)
        {
            var ret = _ConnectionExists(connectionNameBufferPtr);
            return ret == 1;
        }
    }
}