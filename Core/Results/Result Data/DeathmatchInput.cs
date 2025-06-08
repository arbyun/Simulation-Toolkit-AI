using System;
using SimArena.Core.Results.Objective_Results;
using SimArena.Core.Results.Result_Builder;

namespace SimArena.Core.Results.Result_Data
{
    public record DeathmatchInput: IBuildsResult
    {
        public int WinnerTeam { get; set; }
        public int TotalTeams { get; set; }
        public DeathmatchSimulationResult.Team[] Teams { get; set; } = Array.Empty<DeathmatchSimulationResult.Team>();
        
        public ISimulationResultBuilder CreateBuilder()
        {
            return new DeathmatchSimulationResultBuilder();
        }
    }
}