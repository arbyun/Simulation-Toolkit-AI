using System;
using System.Text.Json.Serialization;
using SimArena.Core.Objectives;
using SimArena.Core.Objectives.Trackers;
using SimArena.Core.Objectives.Trackers.Interfaces;

namespace SimArena.Serialization.Configuration.Objectives
{
    [Serializable]
    [JsonDerivedType(typeof(StepsObjective), nameof(SimulationObjective.Steps))]
    public class StepsObjective : ObjectiveConfiguration
    {
        public int MaxSteps { get; set; }

        public StepsObjective(SimulationObjective type, int maxSteps = 10000) : 
            base(type)
        {
            MaxSteps = maxSteps;
        }
    
        public StepsObjective() : base(SimulationObjective.Steps) { }
        
        public override IObjectiveTracker CreateTracker() => new StepsTracker(MaxSteps);
    }
}