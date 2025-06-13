using SimArena.Core.Objectives.Trackers.Interfaces;
using SimArena.Core.Results;
using SimArena.Core.Results.Result_Data;
using SimArena.Serialization.Configuration.Objectives;

namespace SimArena.Core.Objectives.Trackers
{
    /// <summary>
    /// Tracker for defend objectives
    /// </summary>
    public class DefendTracker : IObjectiveTracker, ITimeElapsedTracker
    {
        private readonly DefendObjective _objective;
        private float _objectiveHealth = 100;
        
        public float TimeElapsed { get; set; }
        public bool ShouldStop { get; private set; }
        
        public DefendTracker(DefendObjective objective)
        {
            _objective = objective;
        }
        
        public void Update(float deltaTime)
        {
            // Increment the time elapsed
            TimeElapsed += deltaTime;
            
            // In a real implementation, we would check if the objective is being attacked
            // and update the objective health accordingly
            
            // For testing, we'll just randomly decrease the objective health
            if (new Random().Next(0, 10) == 0)
            {
                _objectiveHealth -= 5;
                Console.WriteLine($"Objective health decreased to {_objectiveHealth}%");
            }
            
            // Check if the objective is complete
            if (_objectiveHealth <= _objective.ObjectiveThreshold || TimeElapsed >= _objective.MaxMatchTime)
            {
                ShouldStop = true;
            }
        }
        
        public IBuildsResult GetInput()
        {
            // In a real implementation, we would return more detailed information about the defend progress
            return new StepsInput
            {
                Steps = (int)TimeElapsed,
                MaxSteps = (int)_objective.MaxMatchTime
            };
        }
    }
}