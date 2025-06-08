using System;
using System.Text.Json.Serialization;
using SimArena.Core.Objectives;
using SimArena.Core.Objectives.Trackers;
using SimArena.Core.Objectives.Trackers.Interfaces;

namespace SimArena.Serialization.Configuration.Objectives
{
    [Serializable]
    [JsonDerivedType(typeof(DefendObjective), nameof(SimulationObjective.DefendObjective))]
    public class DefendObjective : DeathmatchObjective, IJsonOnDeserialized
    {
        public float ObjectiveThreshold { get; set; }
        public float MaxMatchTime { get; set; }
    
        //Force max 2 teams
        public DefendObjective(SimulationObjective type, int teams, int playersPerTeam, float maxMatchTime = 60f) : 
            base(type, teams, playersPerTeam)
        {
            MaxMatchTime = maxMatchTime;
            Teams = 2;
            ObjectiveThreshold = 0;
        }
    
        public DefendObjective() : base(SimulationObjective.DefendObjective) { }

        public new void OnDeserialized()
        {
            Teams = 2; // Enforce rule
            PlayersPerTeam = PlayersPerTeam < 1 ? 1 : PlayersPerTeam;
        }
        
        public override IObjectiveTracker CreateTracker() => new DefendTracker(this);
    }
}