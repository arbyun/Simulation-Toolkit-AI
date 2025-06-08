using System;

namespace SimArena.Serialization.Configuration
{
    [Serializable]
    public class AgentConfiguration
    {
        #region Properties
        
        /// <summary>
        /// Name of the agent
        /// </summary>
        public string Name { get; set; } = "Agent";
        
        /// <summary>
        /// Brain for the agent
        /// </summary>
        public BrainConfiguration Brain { get; set; }
        
        /// <summary>
        /// IDs of the weapons owned by this agent
        /// </summary>
        public string[] OwnedWeaponIds { get; set; } = Array.Empty<string>();
        
        /// <summary>
        /// Whether to use a random starting position
        /// </summary>
        public bool RandomStart { get; set; } = true;
        
        /// <summary>
        /// Starting X-coordinate
        /// </summary>
        public int StartX { get; set; } = 0;
        
        /// <summary>
        /// Starting Y-coordinate
        /// </summary>
        public int StartY { get; set; } = 0;
        
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