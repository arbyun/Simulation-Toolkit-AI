using SimArena.Core.Results.Result_Builder;

namespace SimArena.Core.Results
{
    public interface IBuildsResult
    {
        ISimulationResultBuilder CreateBuilder();
    }
}