// Reference: https://github.com/KZGlobalTeam/cs2kz-metamod/blob/8d4038394173f1c10d763346d45cd3ccbc0091aa/src/sdk/datatypes.h#L135-L139

using System.Runtime.InteropServices;

namespace SwiftlyS2.Shared.Natives;

[StructLayout(LayoutKind.Sequential)]
public struct TouchListT
{
    public Vector DeltaVelocity;
    public CGameTrace Trace;
}