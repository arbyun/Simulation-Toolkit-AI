using SimArena.Core.Results;

namespace SimArena.Core.Objectives.Trackers.Interfaces
{
    public interface IObjectiveTracker
    {
        IBuildsResult GetInput();
        void Update(float deltaTime);
        bool ShouldStop { get; }
    }
}


