using SimArena.Core.Results.Objective_Results;

namespace SimArena.Core.Results.Result_Builder
{
    public interface ISimulationResultBuilder<in TInput>: ISimulationResultBuilder
    {
        ISimulationResult Build(TInput input);
    }
    
    public interface ISimulationResultBuilder
    {
        ISimulationResult Build(object input);
    }
}