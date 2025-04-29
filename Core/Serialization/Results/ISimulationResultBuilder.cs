using SimArena.Core.Configuration;

namespace SimArena.Core.Serialization.Results
{
    public interface ISimulationResultBuilder<in TInput>: ISimulationResultBuilder
    {
        ISimulationResult Build(TInput input);
    }
    
    public interface ISimulationResultBuilder
    {
        ISimulationResult Build(object input);
    }
    
    public interface IBuildsResult
    {
        ISimulationResultBuilder CreateBuilder();
    }
    
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