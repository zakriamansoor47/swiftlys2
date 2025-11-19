// Reference: https://github.com/KZGlobalTeam/cs2kz-metamod/blob/8d4038394173f1c10d763346d45cd3ccbc0091aa/src/sdk/datatypes.h#L141-L163

using System.Runtime.InteropServices;

namespace SwiftlyS2.Shared.Natives;

[StructLayout(LayoutKind.Explicit, Size = 0x20, Pack = 1)]
public struct SubtickMove
{
    [FieldOffset(0x00)]
    public float When;

    [FieldOffset(0x04)]
    private readonly int Padding0;

    [FieldOffset(0x08)]
    public ulong Button;

    // Union: pressed (bool) or analogMove struct
    [FieldOffset(0x10)]
    public bool Pressed;

    [FieldOffset(0x10)]
    public float AnalogForwardDelta;

    [FieldOffset(0x14)]
    public float AnalogLeftDelta;

    [FieldOffset(0x18)]
    public float PitchDelta;

    [FieldOffset(0x1C)]
    public float YawDelta;

    public readonly bool IsAnalogInput()
    {
        return Button == 0;
    }
}