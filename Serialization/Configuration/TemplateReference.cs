using System;
using System.Collections.Generic;

namespace SimArena.Serialization.Configuration
{
    /// <summary>
    /// Represents a reference to a template configuration with optional overrides
    /// </summary>
    [Serializable]
    public class TemplateReference
    {
        /// <summary>
        /// Path to the template file or template ID
        /// </summary>
        public string TemplatePath { get; set; }
        
        /// <summary>
        /// Optional overrides for specific properties from the template
        /// Key-value pairs where key is the property path (e.g., "Brain.Awareness")
        /// and value is the new value to override
        /// </summary>
        public Dictionary<string, object> Overrides { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Optional name override for the instantiated configuration
        /// </summary>
        public string Name { get; set; }
    }
}