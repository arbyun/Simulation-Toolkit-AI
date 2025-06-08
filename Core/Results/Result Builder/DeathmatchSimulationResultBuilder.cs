using SimArena.Core.Results.Objective_Results;
using SimArena.Core.Results.Result_Data;

namespace SimArena.Core.Results.Result_Builder
{
    public class DeathmatchSimulationResultBuilder : ISimulationResultBuilder<DeathmatchInput>
    {
        public ISimulationResult Build(DeathmatchInput input)
        {
            return new DeathmatchSimulationResult(input.WinnerTeam, input.TotalTeams, input.Teams);
        }
        
        public ISimulationResult Build(object input)
        {
            return Build((DeathmatchInput)input);
        }
    }
}