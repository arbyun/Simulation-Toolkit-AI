using System;
using System.Text.Json.Serialization;
using RogueSharp;
using RogueSharp.MapCreation;
using SimArena.Serialization.Configuration.Maps;

namespace SimArena.Serialization.Configuration
{
    /// <summary>
    /// Base class for map configurations with polymorphic JSON serialization
    /// </summary>
    [Serializable]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(SimpleMapConfiguration), "Simple")]
    [JsonDerivedType(typeof(ProcGenMapConfiguration), "ProcGen")]
    [JsonDerivedType(typeof(FileMapConfiguration), "File")]
    public abstract class MapConfiguration
    {
        /// <summary>
        /// Width of the map
        /// </summary>
        public int Width { get; set; }
        
        /// <summary>
        /// Height of the map
        /// </summary>
        public int Height { get; set; }

        protected MapConfiguration() { }
        
        protected MapConfiguration(int width, int height)
        {
            Width = width;
            Height = height;
        }
        
        /// <summary>
        /// Factory method to create a map from this configuration
        /// </summary>
        /// <returns>The created map</returns>
        public abstract Map CreateMap();
        
        /// <summary>
        /// Validates the map configuration
        /// </summary>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if the configuration is valid, false otherwise</returns>
        public virtual bool Validate(out string errorMessage)
        {
            if (Width <= 0)
            {
                errorMessage = "Map width must be greater than 0";
                return false;
            }
            
            if (Height <= 0)
            {
                errorMessage = "Map height must be greater than 0";
                return false;
            }
            
            errorMessage = null;
            return true;
        }
    }
}