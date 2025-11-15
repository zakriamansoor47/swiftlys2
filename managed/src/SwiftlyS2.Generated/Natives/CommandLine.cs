#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeCommandLine
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<byte*, byte> _HasParameter;

    public unsafe static bool HasParameter(string parameter)
    {
        byte[] parameterBuffer = Encoding.UTF8.GetBytes(parameter + "\0");
        fixed (byte* parameterBufferPtr = parameterBuffer)
        {
            var ret = _HasParameter(parameterBufferPtr);
            return ret == 1;
        }
    }

    private unsafe static delegate* unmanaged<int> _GetParameterCount;

    public unsafe static int GetParameterCount()
    {
        var ret = _GetParameterCount();
        return ret;
    }

    private unsafe static delegate* unmanaged<byte*, byte*, byte*, int> _GetParameterValueString;

    public unsafe static string GetParameterValueString(string parameter, string defaultValue)
    {
        byte[] parameterBuffer = Encoding.UTF8.GetBytes(parameter + "\0");
        byte[] defaultValueBuffer = Encoding.UTF8.GetBytes(defaultValue + "\0");
        fixed (byte* parameterBufferPtr = parameterBuffer)
        {
            fixed (byte* defaultValueBufferPtr = defaultValueBuffer)
            {
                var ret = _GetParameterValueString(null, parameterBufferPtr, defaultValueBufferPtr);
                var retBuffer = new byte[ret + 1];
                fixed (byte* retBufferPtr = retBuffer)
                {
                    ret = _GetParameterValueString(retBufferPtr, parameterBufferPtr, defaultValueBufferPtr);
                    return Encoding.UTF8.GetString(retBufferPtr, ret);
                }
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, int> _GetParameterValueInt;

    public unsafe static int GetParameterValueInt(string parameter, int defaultValue)
    {
        byte[] parameterBuffer = Encoding.UTF8.GetBytes(parameter + "\0");
        fixed (byte* parameterBufferPtr = parameterBuffer)
        {
            var ret = _GetParameterValueInt(parameterBufferPtr, defaultValue);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, float, float> _GetParameterValueFloat;

    public unsafe static float GetParameterValueFloat(string parameter, float defaultValue)
    {
        byte[] parameterBuffer = Encoding.UTF8.GetBytes(parameter + "\0");
        fixed (byte* parameterBufferPtr = parameterBuffer)
        {
            var ret = _GetParameterValueFloat(parameterBufferPtr, defaultValue);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, int> _GetCommandLine;

    public unsafe static string GetCommandLine()
    {
        var ret = _GetCommandLine(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetCommandLine(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }

    private unsafe static delegate* unmanaged<byte> _HasParameters;

    public unsafe static bool HasParameters()
    {
        var ret = _HasParameters();
        return ret == 1;
    }
}