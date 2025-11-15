#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeMemoryHelpers
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<byte*, nint> _FetchInterfaceByName;

    /// <summary>
    /// supports both internal interface system, but also valve interface system
    /// </summary>
    public unsafe static nint FetchInterfaceByName(string ifaceName)
    {
        byte[] ifaceNameBuffer = Encoding.UTF8.GetBytes(ifaceName + "\0");
        fixed (byte* ifaceNameBufferPtr = ifaceNameBuffer)
        {
            var ret = _FetchInterfaceByName(ifaceNameBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, nint> _GetVirtualTableAddress;

    public unsafe static nint GetVirtualTableAddress(string library, string vtableName)
    {
        byte[] libraryBuffer = Encoding.UTF8.GetBytes(library + "\0");
        byte[] vtableNameBuffer = Encoding.UTF8.GetBytes(vtableName + "\0");
        fixed (byte* libraryBufferPtr = libraryBuffer)
        {
            fixed (byte* vtableNameBufferPtr = vtableNameBuffer)
            {
                var ret = _GetVirtualTableAddress(libraryBufferPtr, vtableNameBufferPtr);
                return ret;
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, int, byte, nint> _GetAddressBySignature;

    public unsafe static nint GetAddressBySignature(string library, string sig, int len, bool rawBytes)
    {
        byte[] libraryBuffer = Encoding.UTF8.GetBytes(library + "\0");
        byte[] sigBuffer = Encoding.UTF8.GetBytes(sig + "\0");
        fixed (byte* libraryBufferPtr = libraryBuffer)
        {
            fixed (byte* sigBufferPtr = sigBuffer)
            {
                var ret = _GetAddressBySignature(libraryBufferPtr, sigBufferPtr, len, rawBytes ? (byte)1 : (byte)0);
                return ret;
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint, int> _GetObjectPtrVtableName;

    public unsafe static string GetObjectPtrVtableName(nint objptr)
    {
        var ret = _GetObjectPtrVtableName(null, objptr);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetObjectPtrVtableName(retBufferPtr, objptr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte> _ObjectPtrHasVtable;

    public unsafe static bool ObjectPtrHasVtable(nint objptr)
    {
        var ret = _ObjectPtrHasVtable(objptr);
        return ret == 1;
    }

    private unsafe static delegate* unmanaged<nint, byte*, byte> _ObjectPtrHasBaseClass;

    public unsafe static bool ObjectPtrHasBaseClass(nint objptr, string baseClassName)
    {
        byte[] baseClassNameBuffer = Encoding.UTF8.GetBytes(baseClassName + "\0");
        fixed (byte* baseClassNameBufferPtr = baseClassNameBuffer)
        {
            var ret = _ObjectPtrHasBaseClass(objptr, baseClassNameBufferPtr);
            return ret == 1;
        }
    }
}