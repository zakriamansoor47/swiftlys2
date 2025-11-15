#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeConvars
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<int, byte*, void> _QueryClientConvar;

    public unsafe static void QueryClientConvar(int playerid, string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            _QueryClientConvar(playerid, cvarNameBufferPtr);
        }
    }

    private unsafe static delegate* unmanaged<nint, int> _AddQueryClientCvarCallback;

    /// <summary>
    /// the callback should receive the following: int32 playerid, string cvarName, string cvarValue
    /// </summary>
    public unsafe static int AddQueryClientCvarCallback(nint callback)
    {
        var ret = _AddQueryClientCvarCallback(callback);
        return ret;
    }

    private unsafe static delegate* unmanaged<int, void> _RemoveQueryClientCvarCallback;

    public unsafe static void RemoveQueryClientCvarCallback(int callbackID)
    {
        _RemoveQueryClientCvarCallback(callbackID);
    }

    private unsafe static delegate* unmanaged<nint, ulong> _AddGlobalChangeListener;

    /// <summary>
    /// the callback should receive the following: string convarName, int playerid, string newValue, string oldValue
    /// </summary>
    public unsafe static ulong AddGlobalChangeListener(nint callback)
    {
        var ret = _AddGlobalChangeListener(callback);
        return ret;
    }

    private unsafe static delegate* unmanaged<ulong, void> _RemoveGlobalChangeListener;

    public unsafe static void RemoveGlobalChangeListener(ulong callbackID)
    {
        _RemoveGlobalChangeListener(callbackID);
    }

    private unsafe static delegate* unmanaged<nint, ulong> _AddConvarCreatedListener;

    /// <summary>
    /// the callback should receive the following: string convarName
    /// </summary>
    public unsafe static ulong AddConvarCreatedListener(nint callback)
    {
        var ret = _AddConvarCreatedListener(callback);
        return ret;
    }

    private unsafe static delegate* unmanaged<ulong, void> _RemoveConvarCreatedListener;

    public unsafe static void RemoveConvarCreatedListener(ulong callbackID)
    {
        _RemoveConvarCreatedListener(callbackID);
    }

    private unsafe static delegate* unmanaged<nint, ulong> _AddConCommandCreatedListener;

    /// <summary>
    /// the callback should receive the following: string commandName
    /// </summary>
    public unsafe static ulong AddConCommandCreatedListener(nint callback)
    {
        var ret = _AddConCommandCreatedListener(callback);
        return ret;
    }

    private unsafe static delegate* unmanaged<ulong, void> _RemoveConCommandCreatedListener;

    public unsafe static void RemoveConCommandCreatedListener(ulong callbackID)
    {
        _RemoveConCommandCreatedListener(callbackID);
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, short, nint, nint, void> _CreateConvarInt16;

    public unsafe static void CreateConvarInt16(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, short defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarInt16(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, ushort, nint, nint, void> _CreateConvarUInt16;

    public unsafe static void CreateConvarUInt16(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, ushort defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarUInt16(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, int, nint, nint, void> _CreateConvarInt32;

    public unsafe static void CreateConvarInt32(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, int defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarInt32(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, uint, nint, nint, void> _CreateConvarUInt32;

    public unsafe static void CreateConvarUInt32(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, uint defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarUInt32(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, long, nint, nint, void> _CreateConvarInt64;

    public unsafe static void CreateConvarInt64(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, long defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarInt64(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, ulong, nint, nint, void> _CreateConvarUInt64;

    public unsafe static void CreateConvarUInt64(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, ulong defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarUInt64(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, byte, nint, nint, void> _CreateConvarBool;

    public unsafe static void CreateConvarBool(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, bool defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarBool(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue ? (byte)1 : (byte)0, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, float, nint, nint, void> _CreateConvarFloat;

    public unsafe static void CreateConvarFloat(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, float defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarFloat(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, double, nint, nint, void> _CreateConvarDouble;

    public unsafe static void CreateConvarDouble(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, double defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarDouble(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, Color, nint, nint, void> _CreateConvarColor;

    public unsafe static void CreateConvarColor(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, Color defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarColor(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, Vector2D, nint, nint, void> _CreateConvarVector2D;

    public unsafe static void CreateConvarVector2D(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, Vector2D defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarVector2D(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, Vector, nint, nint, void> _CreateConvarVector;

    public unsafe static void CreateConvarVector(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, Vector defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarVector(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, Vector4D, nint, nint, void> _CreateConvarVector4D;

    public unsafe static void CreateConvarVector4D(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, Vector4D defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarVector4D(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, QAngle, nint, nint, void> _CreateConvarQAngle;

    public unsafe static void CreateConvarQAngle(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, QAngle defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                _CreateConvarQAngle(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValue, minValue, maxValue);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, int, ulong, byte*, byte*, nint, nint, void> _CreateConvarString;

    public unsafe static void CreateConvarString(string cvarName, int cvarType, ulong cvarFlags, string helpMessage, string defaultValue, nint minValue, nint maxValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] helpMessageBuffer = Encoding.UTF8.GetBytes(helpMessage + "\0");
        byte[] defaultValueBuffer = Encoding.UTF8.GetBytes(defaultValue + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* helpMessageBufferPtr = helpMessageBuffer)
            {
                fixed (byte* defaultValueBufferPtr = defaultValueBuffer)
                {
                    _CreateConvarString(cvarNameBufferPtr, cvarType, cvarFlags, helpMessageBufferPtr, defaultValueBufferPtr, minValue, maxValue);
                }
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, void> _DeleteConvar;

    public unsafe static void DeleteConvar(string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            _DeleteConvar(cvarNameBufferPtr);
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte> _ExistsConvar;

    public unsafe static bool ExistsConvar(string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            var ret = _ExistsConvar(cvarNameBufferPtr);
            return ret == 1;
        }
    }

    private unsafe static delegate* unmanaged<byte*, int> _GetConvarType;

    public unsafe static int GetConvarType(string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            var ret = _GetConvarType(cvarNameBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<int, byte*, byte*, void> _SetClientConvarValueString;

    public unsafe static void SetClientConvarValueString(int playerid, string cvarName, string defaultValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] defaultValueBuffer = Encoding.UTF8.GetBytes(defaultValue + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* defaultValueBufferPtr = defaultValueBuffer)
            {
                _SetClientConvarValueString(playerid, cvarNameBufferPtr, defaultValueBufferPtr);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, ulong> _GetFlags;

    public unsafe static ulong GetFlags(string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            var ret = _GetFlags(cvarNameBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, ulong, void> _SetFlags;

    public unsafe static void SetFlags(string cvarName, ulong flags)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            _SetFlags(cvarNameBufferPtr, flags);
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint> _GetMinValuePtrPtr;

    public unsafe static nint GetMinValuePtrPtr(string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            var ret = _GetMinValuePtrPtr(cvarNameBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint> _GetMaxValuePtrPtr;

    public unsafe static nint GetMaxValuePtrPtr(string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            var ret = _GetMaxValuePtrPtr(cvarNameBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte> _HasDefaultValue;

    public unsafe static bool HasDefaultValue(string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            var ret = _HasDefaultValue(cvarNameBufferPtr);
            return ret == 1;
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint> _GetDefaultValuePtr;

    public unsafe static nint GetDefaultValuePtr(string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            var ret = _GetDefaultValuePtr(cvarNameBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint, void> _SetDefaultValue;

    public unsafe static void SetDefaultValue(string cvarName, nint defaultValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            _SetDefaultValue(cvarNameBufferPtr, defaultValue);
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, void> _SetDefaultValueString;

    public unsafe static void SetDefaultValueString(string cvarName, string defaultValue)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        byte[] defaultValueBuffer = Encoding.UTF8.GetBytes(defaultValue + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            fixed (byte* defaultValueBufferPtr = defaultValueBuffer)
            {
                _SetDefaultValueString(cvarNameBufferPtr, defaultValueBufferPtr);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint> _GetValuePtr;

    public unsafe static nint GetValuePtr(string cvarName)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            var ret = _GetValuePtr(cvarNameBufferPtr);
            return ret;
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint, void> _SetValuePtr;

    public unsafe static void SetValuePtr(string cvarName, nint value)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            _SetValuePtr(cvarNameBufferPtr, value);
        }
    }

    private unsafe static delegate* unmanaged<byte*, nint, void> _SetValueInternalPtr;

    public unsafe static void SetValueInternalPtr(string cvarName, nint value)
    {
        byte[] cvarNameBuffer = Encoding.UTF8.GetBytes(cvarName + "\0");
        fixed (byte* cvarNameBufferPtr = cvarNameBuffer)
        {
            _SetValueInternalPtr(cvarNameBufferPtr, value);
        }
    }
}