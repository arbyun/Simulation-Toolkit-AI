using System;
using System.Text.Json.Serialization;
using RogueSharp;
using SimArena.Brains;
using SimArena.Core;
using SimArena.Entities;

namespace SimArena.Serialization.Configuration.Brains
{
    /// <summary>
    /// Configuration for ChaserBrain
    /// </summary>
    [Serializable]
    public class ChaserBrainConfiguration : BrainConfiguration, IJsonOnDeserialized
    {
        /// <summary>
        /// Whether the chaser is passive (runs away) or aggressive (chases enemies)
        /// </summary>
        public bool IsPassive { get; set; }
        
        public ChaserBrainConfiguration(int team = 0, bool isPassive = false, int tickIntervalMs = 500, int awareness = 10) 
            : base("ChaserBrain", team, tickIntervalMs, awareness)
        {
            IsPassive = isPassive;
        }
        
        // For JSON deserialization
        public ChaserBrainConfiguration() : base("ChaserBrain") { }
        
        public void OnDeserialized()
        {
            // Any validation or post-deserialization logic can go here
        }
        
        public override Brain CreateBrain(Agent agent, IMap map, Simulation simulation = null)
        {
            if (simulation == null)
            {
                throw new ArgumentNullException(nameof(simulation), 
                    "Simulation instance is required for creating a ChaserBrain.");
            }
            
            var brain = new ChaserBrain(map, simulation, Team, IsPassive, TickIntervalMs);
            if (agent != null)
            {
                brain.SetAgent(agent);
            }
            return brain;
        }
    }
}