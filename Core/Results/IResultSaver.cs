using SimArena.Core.Results.Objective_Results;

namespace SimArena.Core.Results
{
    public interface IResultSaver
    {
        void Save(ISimulationResult result, string path);
        void Save(ISimulationResult result);
    }
}