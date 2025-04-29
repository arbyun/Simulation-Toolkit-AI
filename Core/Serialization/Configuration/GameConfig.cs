using System.Text.Json;
using System.Xml.Serialization;
using SimArena.Core.Serialization.Configuration;

namespace SimArena.Core.Configuration
{
    [Serializable]
    public class GameConfig
    {
        #region Properties
        
        /// <summary>
        /// Name of the match
        /// </summary>
        public string Name { get; set; } = "Default Match";
    
        /// <summary>
        /// Path to the map file
        /// </summary>
        public string MapPath { get; set; }
    
        /// <summary>
        /// Whether to run the simulation in realtime mode
        /// </summary>
        public bool RealtimeMode { get; set; } = true;
    
        /// <summary>
        /// Maximum number of simulation steps (for offline mode)
        /// </summary>
        public ObjectiveConfig? Objective { get; set; }
    
        /// <summary>
        /// List of agent configurations
        /// </summary>
        public List<AgentConfig> Agents { get; set; } = new List<AgentConfig>();
    
        #endregion
    
        #region Methods
    
        /// <summary>
        /// Loads a match configuration from a JSON file
        /// </summary>
        /// <param name="path">Path to the JSON file</param>
        /// <returns>The loaded match configuration</returns>
        public static GameConfig? LoadFromJson(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file not found: {path}");
            
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<GameConfig>(json);
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static GameConfig? LoadFromTextJson(string text)
        {
            return JsonSerializer.Deserialize<GameConfig>(text);
        }
    
        /// <summary>
        /// Loads a match configuration from an XML file
        /// </summary>
        /// <param name="path">Path to the XML file</param>
        /// <returns>The loaded match configuration</returns>
        public static GameConfig? LoadFromXml(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file not found: {path}");
            
            using var stream = new FileStream(path, FileMode.Open);
            var serializer = new XmlSerializer(typeof(GameConfig));
            return (GameConfig?)serializer.Deserialize(stream);
        }
    
        /// <summary>
        /// Loads a match configuration from a file (JSON or XML)
        /// </summary>
        /// <param name="path">Path to the configuration file</param>
        /// <returns>The loaded match configuration</returns>
        public static GameConfig? LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file not found: {path}");
            
            string extension = Path.GetExtension(path).ToLower();
        
            return extension switch
            {
                ".json" => LoadFromJson(path),
                ".xml" => LoadFromXml(path),
                _ => throw new NotSupportedException($"Unsupported file extension: {extension}")
            };
        }
    
        /// <summary>
        /// Saves the match configuration to a JSON file
        /// </summary>
        /// <param name="path">Path to save the JSON file</param>
        public void SaveToJson(string path)
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    
        /// <summary>
        /// Saves the match configuration to an XML file
        /// </summary>
        /// <param name="path">Path to save the XML file</param>
        public void SaveToXml(string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            var serializer = new XmlSerializer(typeof(GameConfig));
            serializer.Serialize(stream, this);
        }

        /// <summary>
        /// Validates the match configuration
        /// </summary>
        /// <param name="consoleMode">Whether the configuration is being validated for console mode</param>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if the configuration is valid, false otherwise</returns>
        public bool Validate(bool consoleMode, out string? errorMessage)
        {
            // Check if the map file exists
            if (string.IsNullOrEmpty(MapPath) || !File.Exists(MapPath))
            {
                errorMessage = $"Map file not found: {MapPath}";
                return false;
            }
        
            // Check if there are any agents
            if (Agents.Count == 0)
            {
                errorMessage = "No agents defined in the configuration";
                return false;
            }
        
            // In console mode, check if there are any human-controlled agents
            if (consoleMode)
            {
                if (Agents.Any(agent => agent.BrainType == BrainType.Human))
                {
                    errorMessage = "Human-controlled agents are not supported in console mode";
                    return false;
                }

                // In console mode, the simulation must be in offline mode
                if (RealtimeMode)
                {
                    errorMessage = "Realtime mode is not supported in console mode";
                    return false;
                }
            }
        
            errorMessage = null;
            return true;
        }
    
        #endregion
    }
}