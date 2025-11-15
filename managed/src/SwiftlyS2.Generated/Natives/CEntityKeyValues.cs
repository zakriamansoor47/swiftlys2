#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeCEntityKeyValues
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<nint> _Allocate;

    public unsafe static nint Allocate()
    {
        var ret = _Allocate();
        return ret;
    }

    private unsafe static delegate* unmanaged<nint, void> _Deallocate;

    public unsafe static void Deallocate(nint keyvalues)
    {
        _Deallocate(keyvalues);
    }

    private unsafe static delegate* unmanaged<nint, byte*, byte> _GetBool;

    public unsafe static bool GetBool(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetBool(keyvalues, keyBufferPtr);
            return ret == 1;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, int> _GetInt;

    public unsafe static int GetInt(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetInt(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, uint> _GetUint;

    public unsafe static uint GetUint(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetUint(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, long> _GetInt64;

    public unsafe static long GetInt64(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetInt64(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, ulong> _GetUint64;

    public unsafe static ulong GetUint64(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetUint64(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, float> _GetFloat;

    public unsafe static float GetFloat(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetFloat(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, double> _GetDouble;

    public unsafe static double GetDouble(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetDouble(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint, byte*, int> _GetString;

    public unsafe static string GetString(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetString(null, keyvalues, keyBufferPtr);
            var retBuffer = new byte[ret + 1];
            fixed (byte* retBufferPtr = retBuffer)
            {
                ret = _GetString(retBufferPtr, keyvalues, keyBufferPtr);
                return Encoding.UTF8.GetString(retBufferPtr, ret);
            }
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, nint> _GetPtr;

    public unsafe static nint GetPtr(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetPtr(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, CUtlStringToken> _GetStringToken;

    public unsafe static CUtlStringToken GetStringToken(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetStringToken(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, Color> _GetColor;

    public unsafe static Color GetColor(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetColor(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, Vector> _GetVector;

    public unsafe static Vector GetVector(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetVector(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, Vector2D> _GetVector2D;

    public unsafe static Vector2D GetVector2D(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetVector2D(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, Vector4D> _GetVector4D;

    public unsafe static Vector4D GetVector4D(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetVector4D(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, QAngle> _GetQAngle;

    public unsafe static QAngle GetQAngle(nint keyvalues, string key)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            var ret = _GetQAngle(keyvalues, keyBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, byte, void> _SetBool;

    public unsafe static void SetBool(nint keyvalues, string key, bool value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetBool(keyvalues, keyBufferPtr, value ? (byte)1 : (byte)0);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, int, void> _SetInt;

    public unsafe static void SetInt(nint keyvalues, string key, int value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetInt(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, uint, void> _SetUint;

    public unsafe static void SetUint(nint keyvalues, string key, uint value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetUint(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, long, void> _SetInt64;

    public unsafe static void SetInt64(nint keyvalues, string key, long value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetInt64(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, ulong, void> _SetUint64;

    public unsafe static void SetUint64(nint keyvalues, string key, ulong value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetUint64(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, float, void> _SetFloat;

    public unsafe static void SetFloat(nint keyvalues, string key, float value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetFloat(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, double, void> _SetDouble;

    public unsafe static void SetDouble(nint keyvalues, string key, double value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetDouble(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, byte*, void> _SetString;

    public unsafe static void SetString(nint keyvalues, string key, string value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        byte[] valueBuffer = Encoding.UTF8.GetBytes(value + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            fixed (byte* valueBufferPtr = valueBuffer)
            {
                _SetString(keyvalues, keyBufferPtr, valueBufferPtr);
            }
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, nint, void> _SetPtr;

    public unsafe static void SetPtr(nint keyvalues, string key, nint value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetPtr(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, CUtlStringToken, void> _SetStringToken;

    public unsafe static void SetStringToken(nint keyvalues, string key, CUtlStringToken value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetStringToken(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, Color, void> _SetColor;

    public unsafe static void SetColor(nint keyvalues, string key, Color value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetColor(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, Vector, void> _SetVector;

    public unsafe static void SetVector(nint keyvalues, string key, Vector value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetVector(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, Vector2D, void> _SetVector2D;

    public unsafe static void SetVector2D(nint keyvalues, string key, Vector2D value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetVector2D(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, Vector4D, void> _SetVector4D;

    public unsafe static void SetVector4D(nint keyvalues, string key, Vector4D value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetVector4D(keyvalues, keyBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<nint, byte*, QAngle, void> _SetQAngle;

    public unsafe static void SetQAngle(nint keyvalues, string key, QAngle value)
    {
        byte[] keyBuffer = Encoding.UTF8.GetBytes(key + "\0");
        fixed (byte* keyBufferPtr = keyBuffer)
        {
            _SetQAngle(keyvalues, keyBufferPtr, value);
        }
    }
}