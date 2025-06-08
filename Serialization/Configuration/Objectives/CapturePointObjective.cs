using System;
using System.Text.Json.Serialization;
using SimArena.Core.Objectives;
using SimArena.Core.Objectives.Trackers;
using SimArena.Core.Objectives.Trackers.Interfaces;

namespace SimArena.Serialization.Configuration.Objectives
{
    [Serializable]
    [JsonDerivedType(typeof(CapturePointObjective), nameof(SimulationObjective.CapturePoint))]
    public class CapturePointObjective : DeathmatchObjective
    {
        public float CaptureRadius { get; set; }
        public float CaptureTime { get; set; }
    
        public CapturePointObjective() : base(SimulationObjective.CapturePoint) { }
    
        public CapturePointObjective(SimulationObjective type, int teams, int playersPerTeam, float captureRadius = 5f, 
            float captureTime = 4f) : base(type, teams, playersPerTeam)
        {
            CaptureRadius = captureRadius;
            CaptureTime = captureTime;
        }
        
        public override IObjectiveTracker CreateTracker() => new CapturePointTracker(this);
    }
}