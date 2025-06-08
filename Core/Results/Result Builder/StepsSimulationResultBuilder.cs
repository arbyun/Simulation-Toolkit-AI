using SimArena.Core.Results.Objective_Results;
using SimArena.Core.Results.Result_Data;

namespace SimArena.Core.Results.Result_Builder
{
    public class StepsSimulationResultBuilder : ISimulationResultBuilder<StepsInput>
    {
        public ISimulationResult Build(StepsInput input)
        {
            return new StepsSimulationResult(input.Steps, input.MaxSteps);
        }

        public ISimulationResult Build(object input)
        {
            return Build((StepsInput)input);
        }
    }
}