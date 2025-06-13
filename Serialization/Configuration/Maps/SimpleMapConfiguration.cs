using System.Text.Json.Serialization;
using RogueSharp;

namespace SimArena.Serialization.Configuration.Maps
{
    /// <summary>
    /// Configuration for a simple empty map with just width and height
    /// </summary>
    [Serializable]
    [JsonDerivedType(typeof(SimpleMapConfiguration), "Simple")]
    public class SimpleMapConfiguration : MapConfiguration
    {
        public SimpleMapConfiguration() { }
        
        public SimpleMapConfiguration(int width, int height) : base(width, height)
        {
        }
        
        /// <summary>
        /// Creates a simple empty map with all cells walkable and transparent
        /// </summary>
        /// <returns>The created map</returns>
        public override Map CreateMap()
        {
            var map = new Map(Width, Height);
            return map;
        }
    }
}