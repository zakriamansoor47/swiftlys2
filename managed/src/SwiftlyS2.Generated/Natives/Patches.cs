#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativePatches
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<byte*, void> _Apply;

    public unsafe static void Apply(string patchName)
    {
        byte[] patchNameBuffer = Encoding.UTF8.GetBytes(patchName + "\0");
        fixed (byte* patchNameBufferPtr = patchNameBuffer)
        {
            _Apply(patchNameBufferPtr);
        }
    }

    private unsafe static delegate* unmanaged<byte*, void> _Revert;

    public unsafe static void Revert(string patchName)
    {
        byte[] patchNameBuffer = Encoding.UTF8.GetBytes(patchName + "\0");
        fixed (byte* patchNameBufferPtr = patchNameBuffer)
        {
            _Revert(patchNameBufferPtr);
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte> _Exists;

    public unsafe static bool Exists(string patchName)
    {
        byte[] patchNameBuffer = Encoding.UTF8.GetBytes(patchName + "\0");
        fixed (byte* patchNameBufferPtr = patchNameBuffer)
        {
            var ret = _Exists(patchNameBufferPtr);
            return ret == 1;
        }
    }
}