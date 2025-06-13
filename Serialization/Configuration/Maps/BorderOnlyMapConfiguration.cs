using System.Text.Json.Serialization;
using RogueSharp;
using RogueSharp.MapCreation;

namespace SimArena.Serialization.Configuration.Maps
{
    /// <summary>
    /// Configuration for BorderOnlyMapCreationStrategy
    /// </summary>
    [Serializable]
    [JsonDerivedType(typeof(BorderOnlyMapConfiguration), "BorderOnly")]
    public class BorderOnlyMapConfiguration : ProcGenMapConfiguration
    {
        public BorderOnlyMapConfiguration() { }
        
        public BorderOnlyMapConfiguration(int width, int height) : base(width, height)
        {
        }
        
        protected override IMapCreationStrategy<Map> CreateMapCreationStrategy()
        {
            return new BorderOnlyMapCreationStrategy<Map>(Width, Height);
        }
    }
}