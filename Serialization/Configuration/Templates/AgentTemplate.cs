using System;

namespace SimArena.Serialization.Configuration
{
    /// <summary>
    /// Template for agent configurations that can be reused and extended
    /// </summary>
    [Serializable]
    public class AgentTemplate : AgentConfiguration, IConfigurationTemplate
    {
        #region IConfigurationTemplate Implementation
        
        /// <summary>
        /// Unique identifier for this template
        /// </summary>
        public string TemplateId { get; set; }
        
        /// <summary>
        /// Description of what this template provides
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Version of the template for backwards compatibility
        /// </summary>
        public string Version { get; set; } = "1.0";
        
        #endregion
        
        /// <summary>
        /// Tags to categorize this template (e.g., "human", "aggressive", "defensive")
        /// </summary>
        public string[] Tags { get; set; } = Array.Empty<string>();
        
        /// <summary>
        /// Author of this template
        /// </summary>
        public string Author { get; set; }
        
        /// <summary>
        /// Date when this template was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Creates a copy of this template for instantiation
        /// </summary>
        /// <returns>A new AgentConfiguration based on this template</returns>
        public AgentConfiguration CreateInstance()
        {
            return new AgentConfiguration
            {
                Name = this.Name,
                Brain = this.Brain,
                OwnedWeaponIds = (string[])this.OwnedWeaponIds.Clone(),
                RandomStart = this.RandomStart,
                StartX = this.StartX,
                StartY = this.StartY,
                MaxHealth = this.MaxHealth,
                AttackPower = this.AttackPower,
                Defense = this.Defense,
                Speed = this.Speed
            };
        }
    }
}