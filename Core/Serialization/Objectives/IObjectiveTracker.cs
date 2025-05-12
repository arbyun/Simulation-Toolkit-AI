using System;
using SimArena.Core.Configuration;
using SimArena.Core.Entities;
using SimArena.Core.Serialization.Results;

namespace SimArena.Core.Serialization.Objectives
{
    public interface IObjectiveTracker
    {
        IBuildsResult GetInput();
        void Update(float deltaTime);
        bool ShouldStop { get; }
    }
    
    public interface IStepTracker: IObjectiveTracker
    {
        event Action<int> StepCompleted;
        public int CurrentStep { get; }
    }
    
    public interface ITimeElapsedTracker : IObjectiveTracker
    {
        float TimeElapsed { get; set; }
    }

    public interface IKillTracker: IObjectiveTracker
    {
        void OnAgentKilled(Entity killer, Entity victim);
    }

    public interface ICompletionTracker: IObjectiveTracker
    {
        bool IsComplete { get; }
    }
    
    public class StepsTracker: IStepTracker, IEventInteractor, ICompletionTracker
    {
        public event Action<int>? StepCompleted;
        public int CurrentStep => _currentStep;
        public bool ShouldStop { get; private set; }
        
        private int _currentStep;
        private int _maxSteps;

        public StepsTracker(int maxSteps)
        {
            _maxSteps = maxSteps;
        } 
        
        public void InitializeEvents(SimulationEvents events)
        {
            StepCompleted += (step) => events.RaiseStepCompleted(this, step);
        }
        
        public void OnStep(float deltaTime)
        {
            _currentStep++;
        }

        public bool IsComplete => _currentStep >= _maxSteps;

        public IBuildsResult GetInput()
        {
            return new StepsInput
            {
                MaxSteps = _maxSteps,
                Steps = CurrentStep
            };
        }

        public void Update(float deltaTime)
        {
            OnStep(deltaTime);
            StepCompleted?.Invoke(CurrentStep);

            if (IsComplete)
            {
                ShouldStop = true;
            }
        }
    }
}


