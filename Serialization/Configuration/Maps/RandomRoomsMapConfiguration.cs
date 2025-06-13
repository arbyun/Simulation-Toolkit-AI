using System.Text.Json.Serialization;
using RogueSharp;
using RogueSharp.MapCreation;

namespace SimArena.Serialization.Configuration.Maps
{
    /// <summary>
    /// Configuration for RandomRoomsMapCreationStrategy
    /// </summary>
    [Serializable]
    [JsonDerivedType(typeof(RandomRoomsMapConfiguration), "RandomRooms")]
    public class RandomRoomsMapConfiguration : ProcGenMapConfiguration
    {
        /// <summary>
        /// Maximum number of rooms to create
        /// </summary>
        public int MaxRooms { get; set; } = 10;
        
        /// <summary>
        /// Maximum width of each room
        /// </summary>
        public int RoomMaxWidth { get; set; } = 6;
        
        /// <summary>
        /// Maximum height of each room
        /// </summary>
        public int RoomMaxHeight { get; set; } = 6;
        
        public RandomRoomsMapConfiguration() { }
        
        public RandomRoomsMapConfiguration(int width, int height, int maxRooms = 10, int roomMaxWidth = 6, int roomMaxHeight = 6) 
            : base(width, height)
        {
            MaxRooms = maxRooms;
            RoomMaxWidth = roomMaxWidth;
            RoomMaxHeight = roomMaxHeight;
        }
        
        protected override IMapCreationStrategy<Map> CreateMapCreationStrategy()
        {
            return new RandomRoomsMapCreationStrategy<Map>(Width, Height, MaxRooms, RoomMaxWidth, RoomMaxHeight);
        }
        
        public override bool Validate(out string errorMessage)
        {
            if (!base.Validate(out errorMessage))
                return false;
                
            if (MaxRooms <= 0)
            {
                errorMessage = "MaxRooms must be greater than 0";
                return false;
            }
            
            if (RoomMaxWidth <= 0)
            {
                errorMessage = "RoomMaxWidth must be greater than 0";
                return false;
            }
            
            if (RoomMaxHeight <= 0)
            {
                errorMessage = "RoomMaxHeight must be greater than 0";
                return false;
            }
            
            return true;
        }
    }
}