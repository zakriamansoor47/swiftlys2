using System.Runtime.InteropServices;
using SwiftlyS2.Shared.Misc;

namespace SwiftlyS2.Shared.Natives;

[StructLayout(LayoutKind.Sequential)]
public struct CCSMatch
{
    public short TotalScore;
    public short ActualRoundsPlayed;
    public short NOvertimePlaying;
    public short CTScoreFirstHalf;
    public short CTScoreSecondHalf;
    public short CTScoreOvertime;
    public short CTScoreTotal;
    public short TerroristScoreFirstHalf;
    public short TerroristScoreSecondHalf;
    public short TerroristScoreOvertime;
    public short TerroristScoreTotal;
    private readonly short _padding;
    public GamePhase Phase;

    /// <summary>
    /// Returns a formatted string representation of the match data.
    /// </summary>
    public override readonly string ToString()
    {
        return $"Match [Round {ActualRoundsPlayed}] T: {TerroristScoreTotal} ({TerroristScoreFirstHalf}/{TerroristScoreSecondHalf}/{TerroristScoreOvertime}) vs CT: {CTScoreTotal} ({CTScoreFirstHalf}/{CTScoreSecondHalf}/{CTScoreOvertime}) | OT: {NOvertimePlaying} | Phase: {Phase}";
    }
}