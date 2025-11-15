#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

public static class NativeBenchmark
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<void> _VoidToVoid;

    public unsafe static void VoidToVoid()
    {
        _VoidToVoid();
    }

    private unsafe static delegate* unmanaged<byte> _GetBool;

    public unsafe static bool GetBool()
    {
        var ret = _GetBool();
        return ret == 1;
    }

    private unsafe static delegate* unmanaged<int> _GetInt32;

    public unsafe static int GetInt32()
    {
        var ret = _GetInt32();
        return ret;
    }

    private unsafe static delegate* unmanaged<uint> _GetUInt32;

    public unsafe static uint GetUInt32()
    {
        var ret = _GetUInt32();
        return ret;
    }

    private unsafe static delegate* unmanaged<long> _GetInt64;

    public unsafe static long GetInt64()
    {
        var ret = _GetInt64();
        return ret;
    }

    private unsafe static delegate* unmanaged<ulong> _GetUInt64;

    public unsafe static ulong GetUInt64()
    {
        var ret = _GetUInt64();
        return ret;
    }

    private unsafe static delegate* unmanaged<float> _GetFloat;

    public unsafe static float GetFloat()
    {
        var ret = _GetFloat();
        return ret;
    }

    private unsafe static delegate* unmanaged<double> _GetDouble;

    public unsafe static double GetDouble()
    {
        var ret = _GetDouble();
        return ret;
    }

    private unsafe static delegate* unmanaged<nint> _GetPtr;

    public unsafe static nint GetPtr()
    {
        var ret = _GetPtr();
        return ret;
    }

    private unsafe static delegate* unmanaged<byte, byte> _BoolToBool;

    public unsafe static bool BoolToBool(bool value)
    {
        var ret = _BoolToBool(value ? (byte)1 : (byte)0);
        return ret == 1;
    }

    private unsafe static delegate* unmanaged<int, int> _Int32ToInt32;

    public unsafe static int Int32ToInt32(int value)
    {
        var ret = _Int32ToInt32(value);
        return ret;
    }

    private unsafe static delegate* unmanaged<uint, uint> _UInt32ToUInt32;

    public unsafe static uint UInt32ToUInt32(uint value)
    {
        var ret = _UInt32ToUInt32(value);
        return ret;
    }

    private unsafe static delegate* unmanaged<long, long> _Int64ToInt64;

    public unsafe static long Int64ToInt64(long value)
    {
        var ret = _Int64ToInt64(value);
        return ret;
    }

    private unsafe static delegate* unmanaged<ulong, ulong> _UInt64ToUInt64;

    public unsafe static ulong UInt64ToUInt64(ulong value)
    {
        var ret = _UInt64ToUInt64(value);
        return ret;
    }

    private unsafe static delegate* unmanaged<float, float> _FloatToFloat;

    public unsafe static float FloatToFloat(float value)
    {
        var ret = _FloatToFloat(value);
        return ret;
    }

    private unsafe static delegate* unmanaged<double, double> _DoubleToDouble;

    public unsafe static double DoubleToDouble(double value)
    {
        var ret = _DoubleToDouble(value);
        return ret;
    }

    private unsafe static delegate* unmanaged<nint, nint> _PtrToPtr;

    public unsafe static nint PtrToPtr(nint value)
    {
        var ret = _PtrToPtr(value);
        return ret;
    }

    private unsafe static delegate* unmanaged<byte*, byte*, int> _StringToString;

    public unsafe static string StringToString(string value)
    {
        byte[] valueBuffer = Encoding.UTF8.GetBytes(value + "\0");
        fixed (byte* valueBufferPtr = valueBuffer)
        {
            var ret = _StringToString(null, valueBufferPtr);
            var retBuffer = new byte[ret + 1];
            fixed (byte* retBufferPtr = retBuffer)
            {
                ret = _StringToString(retBufferPtr, valueBufferPtr);
                return Encoding.UTF8.GetString(retBufferPtr, ret);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint> _StringToPtr;

    public unsafe static nint StringToPtr(string value)
    {
        byte[] valueBuffer = Encoding.UTF8.GetBytes(value + "\0");
        fixed (byte* valueBufferPtr = valueBuffer)
        {
            var ret = _StringToPtr(valueBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, int, float, byte, ulong, int> _MultiPrimitives;

    public unsafe static int MultiPrimitives(nint p1, int i1, float f1, bool b1, ulong u1)
    {
        var ret = _MultiPrimitives(p1, i1, f1, b1 ? (byte)1 : (byte)0, u1);
        return ret;
    }

    private unsafe static delegate* unmanaged<nint, byte*, nint, int, float, int> _MultiWithOneString;

    public unsafe static int MultiWithOneString(nint p1, string s1, nint p2, int i1, float f1)
    {
        byte[] s1Buffer = Encoding.UTF8.GetBytes(s1 + "\0");
        fixed (byte* s1BufferPtr = s1Buffer)
        {
            var ret = _MultiWithOneString(p1, s1BufferPtr, p2, i1, f1);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, nint, byte*, int, void> _MultiWithTwoStrings;

    public unsafe static void MultiWithTwoStrings(nint p1, string s1, nint p2, string s2, int i1)
    {
        byte[] s1Buffer = Encoding.UTF8.GetBytes(s1 + "\0");
        byte[] s2Buffer = Encoding.UTF8.GetBytes(s2 + "\0");
        fixed (byte* s1BufferPtr = s1Buffer)
        {
            fixed (byte* s2BufferPtr = s2Buffer)
            {
                _MultiWithTwoStrings(p1, s1BufferPtr, p2, s2BufferPtr, i1);
            }
        }
    }

    private unsafe static delegate* unmanaged<nint, Vector, void> _VectorToVector;

    public unsafe static void VectorToVector(nint result, Vector value)
    {
        _VectorToVector(result, value);
    }

    private unsafe static delegate* unmanaged<nint, QAngle, void> _QAngleToQAngle;

    public unsafe static void QAngleToQAngle(nint result, QAngle value)
    {
        _QAngleToQAngle(result, value);
    }

    private unsafe static delegate* unmanaged<nint, Vector, byte*, QAngle, void> _ComplexWithString;

    public unsafe static void ComplexWithString(nint entity, Vector pos, string name, QAngle angle)
    {
        byte[] nameBuffer = Encoding.UTF8.GetBytes(name + "\0");
        fixed (byte* nameBufferPtr = nameBuffer)
        {
            _ComplexWithString(entity, pos, nameBufferPtr, angle);
        }
    }
}