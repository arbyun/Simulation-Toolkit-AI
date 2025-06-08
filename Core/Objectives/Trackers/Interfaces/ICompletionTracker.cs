namespace SimArena.Core.Objectives.Trackers.Interfaces
{
    public interface ICompletionTracker: IObjectiveTracker
    {
        bool IsComplete { get; }
    }
}