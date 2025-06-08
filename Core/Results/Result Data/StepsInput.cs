using SimArena.Core.Results.Result_Builder;

namespace SimArena.Core.Results.Result_Data
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
}