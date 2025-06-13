using System.Text.Json.Serialization;
using RogueSharp;
using SimArena.Utilities;

namespace SimArena.Serialization.Configuration.Maps
{
    /// <summary>
    /// Configuration for maps loaded from text files
    /// </summary>
    [Serializable]
    [JsonDerivedType(typeof(FileMapConfiguration), "File")]
    public class FileMapConfiguration : MapConfiguration
    {
        /// <summary>
        /// Path to the text file containing the map data
        /// </summary>
        public string FilePath { get; set; }

        public FileMapConfiguration() { }
        
        public FileMapConfiguration(string filePath, int width, int height) : base(width, height)
        {
            FilePath = filePath;
        }
        
        /// <summary>
        /// Creates a map by loading it from the specified file
        /// </summary>
        /// <returns>The created map</returns>
        public override Map CreateMap()
        {
            var strategy = new FileBasedMapCreationStrategy<Map>(FilePath);
            return strategy.CreateMap();
        }
        
        /// <summary>
        /// Validates the file map configuration
        /// </summary>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if the configuration is valid, false otherwise</returns>
        public override bool Validate(out string errorMessage)
        {
            if (!base.Validate(out errorMessage))
                return false;
                
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                errorMessage = "FilePath cannot be null or empty";
                return false;
            }
            
            if (!File.Exists(FilePath))
            {
                errorMessage = $"Map file not found: {FilePath}";
                return false;
            }
            
            return true;
        }
    }
}