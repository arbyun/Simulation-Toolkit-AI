using System;
using System.Collections.Generic;

namespace SimArena.Serialization.Configuration
{
    /// <summary>
    /// Template for map configurations that can be reused across different game configurations
    /// </summary>
    [Serializable]
    public class MapTemplate : IConfigurationTemplate
    {
        /// <summary>
        /// Unique identifier for this template
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Human-readable name for this template
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unique identifier for this template
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Description of this map template
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Version of the template for backwards compatibility
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The map configuration for this template
        /// </summary>
        public MapConfiguration MapConfiguration { get; set; }
        
        /// <summary>
        /// Tags for categorizing and searching templates
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        public MapTemplate() { }
        
        public MapTemplate(string id, string name, MapConfiguration mapConfiguration, string description = null)
        {
            Id = id;
            Name = name;
            MapConfiguration = mapConfiguration;
            Description = description;
        }
        
        /// <summary>
        /// Creates a map configuration from this template with optional overrides
        /// </summary>
        /// <param name="overrides">Property overrides to apply</param>
        /// <returns>The configured map configuration</returns>
        public MapConfiguration CreateMapConfiguration(Dictionary<string, object> overrides = null)
        {
            // For now, we'll return a copy of the template's configuration
            // In a more advanced implementation, we could apply overrides here
            return MapConfiguration;
        }
        
        /// <summary>
        /// Validates the map template
        /// </summary>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if the template is valid, false otherwise</returns>
        public bool Validate(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                errorMessage = "Map template Id cannot be null or empty";
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Map template Name cannot be null or empty";
                return false;
            }
            
            if (MapConfiguration == null)
            {
                errorMessage = "Map template must have a MapConfiguration";
                return false;
            }
            
            if (!MapConfiguration.Validate(out errorMessage))
            {
                return false;
            }
            
            errorMessage = null;
            return true;
        }
    }
}