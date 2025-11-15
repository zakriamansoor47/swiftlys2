#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeOffsets
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<byte*, byte> _Exists;

    public unsafe static bool Exists(string name)
    {
        byte[] nameBuffer = Encoding.UTF8.GetBytes(name + "\0");
        fixed (byte* nameBufferPtr = nameBuffer)
        {
            var ret = _Exists(nameBufferPtr);
            return ret == 1;
        }
    }

    private unsafe static delegate* unmanaged<byte*, int> _Fetch;

    public unsafe static int Fetch(string name)
    {
        byte[] nameBuffer = Encoding.UTF8.GetBytes(name + "\0");
        fixed (byte* nameBufferPtr = nameBuffer)
        {
            var ret = _Fetch(nameBufferPtr);
            return ret;
        }
    }
}