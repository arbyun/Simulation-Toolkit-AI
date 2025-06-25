using System.Text.Json.Serialization;
using SimArena.Core.Objectives;
using SimArena.Core.Objectives.Trackers.Interfaces;
using SimArena.Serialization.Configuration.Objectives;

namespace SimArena.Serialization.Configuration
{
    [Serializable]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(DeathmatchObjective), nameof(SimulationObjective.TeamDeathmatch))]
    [JsonDerivedType(typeof(StepsObjective), nameof(SimulationObjective.Steps))]
    [JsonDerivedType(typeof(CapturePointObjective), nameof(SimulationObjective.CapturePoint))]
    [JsonDerivedType(typeof(DefendObjective), nameof(SimulationObjective.DefendObjective))]
    public class ObjectiveConfiguration
    {
        [JsonPropertyName("TypeEnum")]
        [JsonConverter(typeof(JsonStringEnumConverter<SimulationObjective>))] 
        public SimulationObjective TypeEnum { get; set; }

        public ObjectiveConfiguration(SimulationObjective type)
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
}