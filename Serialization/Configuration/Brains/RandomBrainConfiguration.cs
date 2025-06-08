using System;
using Plugins.RogueSharp.RogueSharp;
using SimArena.Brains;
using SimArena.Core;
using SimArena.Entities;

namespace SimArena.Serialization.Configuration.Brains
{
    /// <summary>
    /// Configuration for RandomBrain
    /// </summary>
    [Serializable]
    public class RandomBrainConfiguration : BrainConfiguration
    {
        public RandomBrainConfiguration(int team = 0, int tickIntervalMs = 500, int awareness = 10) 
            : base("RandomBrain", team, tickIntervalMs, awareness)
        {
        }
        
        // For JSON deserialization
        public RandomBrainConfiguration() : base("RandomBrain") { }
        
        public override Brain CreateBrain(Agent agent, IMap map, Simulation simulation = null)
        {
            var brain = new RandomBrain(map, Team, TickIntervalMs);
            if (agent != null)
            {
                brain.SetAgent(agent);
            }
            return brain;
        }
    }
}