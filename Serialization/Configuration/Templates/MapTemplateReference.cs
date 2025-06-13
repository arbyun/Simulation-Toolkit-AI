using System;
using System.Collections.Generic;

namespace SimArena.Serialization.Configuration
{
    /// <summary>
    /// Reference to a map template with optional property overrides
    /// </summary>
    [Serializable]
    public class MapTemplateReference
    {
        /// <summary>
        /// Path or identifier of the template to reference
        /// </summary>
        public string TemplatePath { get; set; }
        
        /// <summary>
        /// Optional property overrides to apply to the template
        /// Format: "PropertyName": value or "NestedObject.PropertyName": value
        /// </summary>
        public Dictionary<string, object> Overrides { get; set; } = new Dictionary<string, object>();

        public MapTemplateReference() { }
        
        public MapTemplateReference(string templatePath, Dictionary<string, object> overrides = null)
        {
            TemplatePath = templatePath;
            Overrides = overrides ?? new Dictionary<string, object>();
        }
        
        /// <summary>
        /// Validates the map template reference
        /// </summary>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if the reference is valid, false otherwise</returns>
        public bool Validate(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(TemplatePath))
            {
                errorMessage = "Map template reference TemplatePath cannot be null or empty";
                return false;
            }
            
            errorMessage = null;
            return true;
        }
    }
}