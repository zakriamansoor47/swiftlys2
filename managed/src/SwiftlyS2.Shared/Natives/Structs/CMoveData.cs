// Reference: https://github.com/KZGlobalTeam/cs2kz-metamod/blob/8d4038394173f1c10d763346d45cd3ccbc0091aa/src/sdk/datatypes.h#L252-L278

using System.Runtime.InteropServices;

namespace SwiftlyS2.Shared.Natives;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CMoveData
{
    public CMoveDataBase Base; // class CMoveData : public CMoveDataBase

    public Vector OutWishVel;
    public QAngle OldAngles;

    /// <summary>
    /// World space input vector. Used to compare against the movement services' previous rotation for ground movement stuff.
    /// </summary>
    public Vector InputRotated;

    /// <summary>
    /// Continuous acceleration in units per second squared (u/s²).
    /// </summary>
    public Vector ContinuousAcceleration;

    /// <summary>
    /// Immediate delta in u/s. Air acceleration bypasses per second acceleration,
    /// applies up to half of its impulse to the velocity and the rest goes straight into this.
    /// </summary>
    public Vector FrameVelocityDelta;

    public float MaxSpeed;
    public float ClientMaxSpeed;
    public float FrictionDecel;
    public bool InAir;
    public bool GameCodeMovedPlayer; // true if usercmd cmd number == (m_nGameCodeHasMovedPlayerAfterCommand + 1)
}