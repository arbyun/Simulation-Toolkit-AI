using System;

namespace SimArena.Core.Objectives.Trackers.Interfaces
{
    public interface IStepTracker: IObjectiveTracker
    {
        event Action<int> StepCompleted;
        public int CurrentStep { get; }
    }
}