using System.Text.Json.Serialization;
using RogueSharp;
using RogueSharp.MapCreation;

namespace SimArena.Serialization.Configuration.Maps
{
    /// <summary>
    /// Configuration for procedurally generated maps using IMapCreationStrategy
    /// </summary>
    [Serializable]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "StrategyType")]
    [JsonDerivedType(typeof(RandomRoomsMapConfiguration), "RandomRooms")]
    [JsonDerivedType(typeof(BorderOnlyMapConfiguration), "BorderOnly")]
    [JsonDerivedType(typeof(ProcGenMapConfiguration), "ProcGen")]
    public abstract class ProcGenMapConfiguration : MapConfiguration
    {
        protected ProcGenMapConfiguration() { }
        
        protected ProcGenMapConfiguration(int width, int height) : base(width, height)
        {
        }
        
        /// <summary>
        /// Creates the appropriate IMapCreationStrategy for this configuration
        /// </summary>
        /// <returns>The map creation strategy</returns>
        protected abstract IMapCreationStrategy<Map> CreateMapCreationStrategy();
        
        /// <summary>
        /// Creates a map using the configured strategy
        /// </summary>
        /// <returns>The created map</returns>
        public override Map CreateMap()
        {
            var strategy = CreateMapCreationStrategy();
            return strategy.CreateMap();
        }
    }
}