using System;
using SimArena.Core.Objectives.Trackers.Interfaces;
using SimArena.Core.Results;
using SimArena.Core.Results.Result_Data;

namespace SimArena.Core.Objectives.Trackers
{
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