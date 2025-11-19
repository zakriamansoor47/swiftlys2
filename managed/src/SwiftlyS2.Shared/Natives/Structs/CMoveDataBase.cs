// Reference: https://github.com/KZGlobalTeam/cs2kz-metamod/blob/8d4038394173f1c10d763346d45cd3ccbc0091aa/src/sdk/datatypes.h#L165-L250

using System.Runtime.InteropServices;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace SwiftlyS2.Shared.Natives;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CMoveDataBase
{
    private byte _bitfield0; // Bitfield members

    public bool HasZeroFrametime {
        get => BitFieldHelper.GetBit(ref _bitfield0, 0);
        set => BitFieldHelper.SetBit(ref _bitfield0, 0, value);
    }

    public bool IsLateCommand {
        get => BitFieldHelper.GetBit(ref _bitfield0, 1);
        set => BitFieldHelper.SetBit(ref _bitfield0, 1, value);
    }

    public CHandle<CCSPlayerPawn> PlayerHandle;
    public QAngle AbsViewAngles;
    public QAngle ViewAngles;
    public Vector LastMovementImpulses;
    public float ForwardMove;
    public float SideMove; // Warning! Flipped compared to CS:GO, moving right gives negative value
    public float UpMove;
    public Vector Velocity;
    public QAngle Angles;
    public Vector Unknown; // Unused. Probably pulled from engine upstream.
    public CUtlVector<SubtickMove> SubtickMoves;
    public CUtlVector<SubtickMove> AttackSubtickMoves;
    public bool HasSubtickInputs;
    public float UnknownFloat; // Set to 1.0 during SetupMove, never change during gameplay. Is apparently used for weapon services stuff.
    public CUtlVector<TouchListT> TouchList;
    public Vector CollisionNormal;
    public Vector GroundNormal;
    public Vector AbsOrigin;
    public int TickCount;
    public int TargetTick;
    public float SubtickStartFraction;
    public float SubtickEndFraction;
}