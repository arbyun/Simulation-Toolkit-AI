using System;
using SimArena.Core.Configuration;

namespace SimArena.Core.Serialization.Results
{
    public record StepsInput: IBuildsResult
    {
        public int Steps { get; set; }
        public int MaxSteps { get; set; }
        
        public ISimulationResultBuilder CreateBuilder()
        {
            return new StepsSimulationResultBuilder();
        }
    }

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