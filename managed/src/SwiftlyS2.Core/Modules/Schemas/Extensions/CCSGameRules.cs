using SwiftlyS2.Shared.Schemas;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Shared.SchemaDefinitions;

public partial interface CCSGameRules
{
    /// <summary>
    /// Find the player that the controller is targetting
    /// </summary>
    /// <typeparam name="T">Entity Class</typeparam>
    /// <param name="controller">Player Controller</param>
    public T? FindPickerEntity<T>( CBasePlayerController controller ) where T : ISchemaClass<T>;

    /// <summary>
    /// Ends the current round with the specified reason after an optional delay
    /// </summary>
    /// <param name="reason">The reason for ending the round</param>
    /// <param name="delay">The delay before ending the round</param>
    public void TerminateRound( RoundEndReason reason, float delay );

    /// <summary>
    /// Ends the current round with the specified reason after an optional delay
    /// </summary>
    /// <param name="reason">The reason for ending the round</param>
    /// <param name="delay">The delay before ending the round</param>
    /// <param name="teamId">The team id to end the round for</param>
    /// <param name="unk01">Unknown parameter</param>
    public void TerminateRound( RoundEndReason reason, float delay, uint teamId, uint unk01 = 0 );

    /// <summary>
    /// Add wins to the Terrorist team
    /// </summary>
    /// <param name="wins">The number of wins to add</param>
    /// <remarks>This only updates the score and does not end the round.</remarks>
    public void AddTerroristWins( short wins );

    /// <summary>
    /// Add wins to the Terrorist team and end the round with the specified reason
    /// </summary>
    /// <param name="wins">The number of wins to add</param>
    /// <param name="delay">The delay before ending the round</param>
    public void AddTerroristWins( short wins, float delay );

    /// <summary>
    /// Add wins to the Counter-Terrorist team
    /// </summary>
    /// <param name="wins">The number of wins to add</param>
    /// <remarks>This only updates the score and does not end the round.</remarks>
    public void AddCTWins( short wins );

    /// <summary>
    /// Add wins to the Counter-Terrorist team and end the round with the specified reason
    /// </summary>
    /// <param name="wins">The number of wins to add</param>
    /// <param name="delay">The delay before ending the round</param>
    public void AddCTWins( short wins, float delay );
}