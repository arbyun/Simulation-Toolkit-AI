using System.Text.Json;
using System.Xml.Serialization;
using SimArena.Serialization.Configuration.Templates;

namespace SimArena.Serialization.Configuration
{
    [Serializable]
    public class GameConfiguration
    {
        #region Properties
        
        /// <summary>
        /// Name of the match
        /// </summary>
        public string Name { get; set; } = "Default Match";
    
        /// <summary>
        /// Map configuration for this game
        /// </summary>
        public MapConfiguration Map { get; set; }
        
        /// <summary>
        /// Map template reference (alternative to direct map configuration)
        /// </summary>
        public MapTemplateReference MapTemplate { get; set; }
    
        /// <summary>
        /// Whether to run the simulation in realtime mode
        /// </summary>
        public bool RealtimeMode { get; set; } = true;
    
        /// <summary>
        /// Maximum number of simulation steps (for offline mode)
        /// </summary>
        public ObjectiveConfiguration Objective { get; set; }
    
        /// <summary>
        /// List of agent configurations
        /// </summary>
        public List<AgentConfiguration> Agents { get; set; } = new List<AgentConfiguration>();
        
        /// <summary>
        /// List of agent template references (alternative to direct agent configurations)
        /// </summary>
        public List<TemplateReference> AgentTemplates { get; set; } = new List<TemplateReference>();
        
        /// <summary>
        /// List of weapon configurations
        /// </summary>
        public List<WeaponConfiguration> Weapons { get; set; } = new List<WeaponConfiguration>();
    
        #endregion
    
        #region Methods
    
        /// <summary>
        /// Loads a match configuration from a JSON file
        /// </summary>
        /// <param name="path">Path to the JSON file</param>
        /// <returns>The loaded match configuration</returns>
        public static GameConfiguration LoadFromJson(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file not found: {path}");
            
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<GameConfiguration>(json);
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static GameConfiguration LoadFromTextJson(string text)
        {
            return JsonSerializer.Deserialize<GameConfiguration>(text);
        }
    
        /// <summary>
        /// Loads a match configuration from an XML file
        /// </summary>
        /// <param name="path">Path to the XML file</param>
        /// <returns>The loaded match configuration</returns>
        public static GameConfiguration LoadFromXml(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file not found: {path}");
            
            using var stream = new FileStream(path, FileMode.Open);
            var serializer = new XmlSerializer(typeof(GameConfiguration));
            return (GameConfiguration)serializer.Deserialize(stream);
        }
    
        /// <summary>
        /// Loads a match configuration from a file (JSON or XML)
        /// </summary>
        /// <param name="path">Path to the configuration file</param>
        /// <returns>The loaded match configuration</returns>
        public static GameConfiguration LoadFromFile(string path)
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
            var serializer = new XmlSerializer(typeof(GameConfiguration));
            serializer.Serialize(stream, this);
        }

        /// <summary>
        /// Resolves all template references to actual configurations
        /// </summary>
        /// <param name="templateManager">Template manager to use for resolution</param>
        public void ResolveTemplates(ITemplateManager templateManager)
        {
            if (templateManager == null)
                throw new ArgumentNullException(nameof(templateManager));
                
            // Resolve map template if specified
            if (MapTemplate != null && Map == null)
            {
                Map = templateManager.ResolveMapTemplate(MapTemplate);
                MapTemplate = null; // Clear template reference after resolution
            }
                
            // Resolve agent templates and add them to the agents list
            foreach (var templateRef in AgentTemplates)
            {
                var agentConfig = templateManager.ResolveAgentTemplate(templateRef);
                Agents.Add(agentConfig);
            }
            
            // Clear template references after resolution
            AgentTemplates.Clear();
        }
        
        /// <summary>
        /// Validates the match configuration
        /// </summary>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if the configuration is valid, false otherwise</returns>
        public bool Validate(out string errorMessage)
        {
            // Check if we have either a map configuration or map template
            if (Map == null && MapTemplate == null)
            {
                errorMessage = "Either Map or MapTemplate must be specified";
                return false;
            }
            
            // Validate map configuration if specified
            if (Map != null && !Map.Validate(out errorMessage))
            {
                return false;
            }
            
            // Validate map template reference if specified
            if (MapTemplate != null && !MapTemplate.Validate(out errorMessage))
            {
                return false;
            }
        
            // Check if there are any agents or agent templates
            if (Agents.Count == 0 && AgentTemplates.Count == 0)
            {
                errorMessage = "No agents or agent templates defined in the configuration";
                return false;
            }
        
            errorMessage = null;
            return true;
        }
    
        #endregion
    }
}