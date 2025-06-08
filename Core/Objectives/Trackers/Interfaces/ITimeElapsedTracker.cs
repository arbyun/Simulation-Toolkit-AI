namespace SimArena.Core.Objectives.Trackers.Interfaces
{
    public interface ITimeElapsedTracker : IObjectiveTracker
    {
        float TimeElapsed { get; set; }
    }
}