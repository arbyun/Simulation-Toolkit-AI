using System;
using System.Text.Json.Serialization;

namespace SimToolAI.Core.Configuration
{
    /// <summary>
    /// Configuration for an agent in a simulation
    /// </summary>
    [Serializable]
    public class AgentConfig
    {
        #region Properties
        
        /// <summary>
        /// Name of the agent
        /// </summary>
        public string Name { get; set; } = "Agent";
        
        /// <summary>
        /// Type of brain for the agent
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter<BrainType>))] 
        public BrainType BrainType { get; set; } = BrainType.AI;
        
        /// <summary>
        /// Starting X-coordinate
        /// </summary>
        public int StartX { get; set; } = 0;
        
        /// <summary>
        /// Starting Y-coordinate
        /// </summary>
        public int StartY { get; set; } = 0;
        
        /// <summary>
        /// Whether to use a random starting position
        /// </summary>
        public bool RandomStart { get; set; } = true;
        
        /// <summary>
        /// Awareness radius
        /// </summary>
        public int Awareness { get; set; } = 10;
        
        /// <summary>
        /// Maximum health
        /// </summary>
        public int MaxHealth { get; set; } = 100;
        
        /// <summary>
        /// Attack power
        /// </summary>
        public int AttackPower { get; set; } = 10;
        
        /// <summary>
        /// Defense
        /// </summary>
        public int Defense { get; set; } = 5;
        
        /// <summary>
        /// Movement speed
        /// </summary>
        public float Speed { get; set; } = 1.0f;
        
        #endregion
    }
}