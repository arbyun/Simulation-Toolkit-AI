using SimArena.Entities;

namespace SimArena.Core.Objectives.Trackers.Interfaces
{
    public interface IKillTracker: IObjectiveTracker
    {
        void OnAgentKilled(Entity killer, Entity victim);
    }
}