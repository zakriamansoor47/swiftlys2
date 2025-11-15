#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeSignatures
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<byte*, byte> _Exists;

    public unsafe static bool Exists(string signatureName)
    {
        byte[] signatureNameBuffer = Encoding.UTF8.GetBytes(signatureName + "\0");
        fixed (byte* signatureNameBufferPtr = signatureNameBuffer)
        {
            var ret = _Exists(signatureNameBufferPtr);
            return ret == 1;
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint> _Fetch;

    public unsafe static nint Fetch(string signatureName)
    {
        byte[] signatureNameBuffer = Encoding.UTF8.GetBytes(signatureName + "\0");
        fixed (byte* signatureNameBufferPtr = signatureNameBuffer)
        {
            var ret = _Fetch(signatureNameBufferPtr);
            return ret;
        }
    }
}