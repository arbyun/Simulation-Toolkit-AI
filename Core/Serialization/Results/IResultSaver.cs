using SimArena.Core.Serialization.Results;

namespace SimArena.Core.Configuration
{
    public interface IResultSaver
    {
        void Save(ISimulationResult result, string path);
        void Save(ISimulationResult result);
    }
}