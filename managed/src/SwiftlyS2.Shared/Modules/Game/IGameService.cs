using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Shared.Services;

public interface IGameService
{
    /// <summary>
    /// Gets the current match data.
    /// </summary>
    CCSMatch MatchData { get; }

    /// <summary>
    /// Resets all match data to initial state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Sets the current game phase.
    /// </summary>
    /// <param name="phase">The game phase to set.</param>
    void SetPhase( GamePhase phase );

    /// <summary>
    /// Adds wins to the Terrorist team.
    /// </summary>
    /// <param name="numWins">Number of wins to add.</param>
    void AddTerroristWins( int numWins );

    /// <summary>
    /// Adds wins to the Counter-Terrorist team.
    /// </summary>
    /// <param name="numWins">Number of wins to add.</param>
    void AddCTWins( int numWins );

    /// <summary>
    /// Increments the round count.
    /// </summary>
    /// <param name="numRounds">Number of rounds to increment.</param>
    void IncrementRound( int numRounds = 1 );

    /// <summary>
    /// Adds bonus points to the Terrorist team.
    /// </summary>
    /// <param name="points">Bonus points to add.</param>
    void AddTerroristBonusPoints( int points );

    /// <summary>
    /// Adds bonus points to the Counter-Terrorist team.
    /// </summary>
    /// <param name="points">Bonus points to add.</param>
    void AddCTBonusPoints( int points );

    /// <summary>
    /// Adds score to the Terrorist team.
    /// </summary>
    /// <param name="score">Score to add.</param>
    void AddTerroristScore( int score );

    /// <summary>
    /// Adds score to the Counter-Terrorist team.
    /// </summary>
    /// <param name="score">Score to add.</param>
    void AddCTScore( int score );

    /// <summary>
    /// Enters overtime mode.
    /// </summary>
    /// <param name="numOvertimesToAdd">Number of overtime periods to add.</param>
    void GoToOvertime( int numOvertimesToAdd = 1 );

    /// <summary>
    /// Swaps the team scores between Terrorist and Counter-Terrorist.
    /// </summary>
    void SwapTeamScores();

    // /// <summary>
    // /// Updates the team score entities based on current match data.
    // /// </summary>
    // void UpdateTeamScores();

    /// <summary>
    /// Gets the winning team ID.
    /// </summary>
    /// <returns>Team ID of the winner, or 0 if tie.</returns>
    int GetWinningTeam();
}