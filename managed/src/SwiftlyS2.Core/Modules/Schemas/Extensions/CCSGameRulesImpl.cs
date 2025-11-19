using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared.Schemas;
using SwiftlyS2.Shared.SchemaDefinitions;
using EndReason = SwiftlyS2.Shared.Natives.RoundEndReason;

namespace SwiftlyS2.Core.SchemaDefinitions;

internal partial class CCSGameRulesImpl : CCSGameRules
{
    public T? FindPickerEntity<T>( CBasePlayerController controller ) where T : ISchemaClass<T>
    {
        return ((CBaseEntity)new CBaseEntityImpl(GameFunctions.FindPickerEntity(Address, controller.Address))).As<T>();
    }

    public void TerminateRound( EndReason reason, float delay )
    {
        GameFunctions.TerminateRound(Address, (uint)reason, delay, 0, 0);
    }

    public void TerminateRound( EndReason reason, float delay, uint teamId, uint unk01 )
    {
        GameFunctions.TerminateRound(Address, (uint)reason, delay, teamId, unk01);
    }

    public void AddTerroristWins( short wins )
    {
        GameFunctions.AddTerroristWins(Address, wins);
    }

    public void AddTerroristWins( short wins, float delay )
    {
        GameFunctions.AddTerroristWins(Address, wins);
        TerminateRound(EndReason.TerroristsWin, delay, 2, 1);
    }

    public void AddCTWins( short wins )
    {
        GameFunctions.AddCTWins(Address, wins);
    }

    public void AddCTWins( short wins, float delay )
    {
        GameFunctions.AddCTWins(Address, wins);
        TerminateRound(EndReason.CTsWin, delay, 3, 1);
    }
}