using System;
using System.Text.Json.Serialization;
using Plugins.RogueSharp.RogueSharp;
using SimArena.Core;
using SimArena.Entities;
using SimArena.Serialization.Configuration.Brains;

namespace SimArena.Serialization.Configuration
{
    [Serializable]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(RandomBrainConfiguration), "RandomBrain")]
    [JsonDerivedType(typeof(ChaserBrainConfiguration), "ChaserBrain")]
    public class BrainConfiguration
    {
        /// <summary>
        /// Name of the brain type
        /// </summary>
        [JsonPropertyName("BrainTypeName")]
        public string BrainTypeName { get; set; } = "RandomBrain";
        
        /// <summary>
        /// Awareness radius
        /// </summary>
        public int Awareness { get; set; } = 10;
        
        /// <summary>
        /// How often the brain ticks (in ms)
        /// </summary>
        public int TickIntervalMs { get; set; } = 500;
        
        /// <summary>
        /// Team the brain belongs to
        /// </summary>
        public int Team { get; set; } = 0;

        // For JSON deserialization
        public BrainConfiguration() { }
        
        public BrainConfiguration(string brainTypeName, int team = 0, int tickIntervalMs = 500, int awareness = 10)
        {
            BrainTypeName = brainTypeName;
            Team = team;
            TickIntervalMs = tickIntervalMs;
            Awareness = awareness;
        }
        
        /// <summary>
        /// Factory method to create an actual brain instance
        /// </summary>
        public virtual Brain CreateBrain(Agent agent, IMap map, Simulation simulation = null)
        {
            throw new NotImplementedException("This method must be overridden in derived classes.");
        }
    }
}