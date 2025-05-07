using System.Text.Json.Serialization;
using SimArena.Core.Configuration;
using SimArena.Core.Serialization.Objectives;

namespace SimArena.Core.Serialization.Configuration
{
    [Serializable]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(DeathmatchObjective), nameof(SimulationObjective.TeamDeathmatch))]
    [JsonDerivedType(typeof(StepsObjective), nameof(SimulationObjective.Steps))]
    [JsonDerivedType(typeof(CapturePointObjective), nameof(SimulationObjective.CapturePoint))]
    [JsonDerivedType(typeof(DefendObjective), nameof(SimulationObjective.DefendObjective))]
    public class ObjectiveConfig
    {
        [JsonPropertyName("TypeEnum")]
        [JsonConverter(typeof(JsonStringEnumConverter<SimulationObjective>))] 
        public SimulationObjective TypeEnum { get; set; }

        public ObjectiveConfig(SimulationObjective type)
        {
            TypeEnum = type;
        }
        
        // This would work so much better if we could make this class (and method) abstract,
        // but JsonSerializer doesn't play nice with that.
        public virtual IObjectiveTracker CreateTracker()
        {
            throw new NotImplementedException("Not ideal, but this needs to overriden in a subclass.");
        }
    }

    [Serializable]
    [JsonDerivedType(typeof(StepsObjective), nameof(SimulationObjective.Steps))]
    public class StepsObjective : ObjectiveConfig
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

    [Serializable]
    [JsonDerivedType(typeof(DeathmatchObjective), nameof(SimulationObjective.TeamDeathmatch))]
    public class DeathmatchObjective : ObjectiveConfig, IJsonOnDeserialized
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