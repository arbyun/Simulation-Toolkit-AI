using System;
using System.Text.Json.Serialization;
using SimArena.Core.Objectives;
using SimArena.Core.Objectives.Trackers;
using SimArena.Core.Objectives.Trackers.Interfaces;

namespace SimArena.Serialization.Configuration.Objectives
{
    [Serializable]
    [JsonDerivedType(typeof(DeathmatchObjective), nameof(SimulationObjective.TeamDeathmatch))]
    public class DeathmatchObjective : ObjectiveConfiguration, IJsonOnDeserialized
    {
        public int Teams { get; set; }
        public int PlayersPerTeam { get; set; }

        public DeathmatchObjective(SimulationObjective type, int teams, int playersPerTeam) : 
            base(type)
        {
            Teams = teams < 2 ? 2 : teams;
            PlayersPerTeam = playersPerTeam < 1 ? 1 : playersPerTeam;
        }
    
        public DeathmatchObjective() : base(SimulationObjective.TeamDeathmatch) { }
    
        public DeathmatchObjective(SimulationObjective type) : base(type) { }

        public void OnDeserialized()
        {
            Teams = Teams < 2 ? 2 : Teams;
            PlayersPerTeam = PlayersPerTeam < 1 ? 1 : PlayersPerTeam;
        }
        
        public override IObjectiveTracker CreateTracker() => new DeathmatchTracker(this);
    }
}