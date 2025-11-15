#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeConsoleOutput
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<nint, ulong> _AddConsoleListener;

    /// <summary>
    /// callback should receive: string message
    /// </summary>
    public unsafe static ulong AddConsoleListener(nint callback)
    {
        var ret = _AddConsoleListener(callback);
        return ret;
    }

    private unsafe static delegate* unmanaged<ulong, void> _RemoveConsoleListener;

    public unsafe static void RemoveConsoleListener(ulong listenerId)
    {
        _RemoveConsoleListener(listenerId);
    }

    private unsafe static delegate* unmanaged<byte> _IsEnabled;

    /// <summary>
    /// returns whether console filtering is enabled
    /// </summary>
    public unsafe static bool IsEnabled()
    {
        var ret = _IsEnabled();
        return ret == 1;
    }

    private unsafe static delegate* unmanaged<void> _ToggleFilter;

    /// <summary>
    /// toggles the console filter on/off
    /// </summary>
    public unsafe static void ToggleFilter()
    {
        _ToggleFilter();
    }

    private unsafe static delegate* unmanaged<void> _ReloadFilterConfiguration;

    /// <summary>
    /// reloads the filter configuration from file
    /// </summary>
    public unsafe static void ReloadFilterConfiguration()
    {
        _ReloadFilterConfiguration();
    }

    private unsafe static delegate* unmanaged<byte*, byte> _NeedsFiltering;

    /// <summary>
    /// checks if a message needs filtering
    /// </summary>
    public unsafe static bool NeedsFiltering(string text)
    {
        byte[] textBuffer = Encoding.UTF8.GetBytes(text + "\0");
        fixed (byte* textBufferPtr = textBuffer)
        {
            var ret = _NeedsFiltering(textBufferPtr);
            return ret == 1;
        }
    }

    private unsafe static delegate* unmanaged<byte*, int> _GetCounterText;

    /// <summary>
    /// gets the counter text showing how many messages were filtered
    /// </summary>
    public unsafe static string GetCounterText()
    {
        var ret = _GetCounterText(null);
        var retBuffer = new byte[ret + 1];
        fixed (byte* retBufferPtr = retBuffer)
        {
            ret = _GetCounterText(retBufferPtr);
            return Encoding.UTF8.GetString(retBufferPtr, ret);
        }
    }
}