using System.Reflection;
using System.Text.Json;
using SimArena.Serialization.Configuration.Templates;

namespace SimArena.Serialization.Configuration
{
    /// <summary>
    /// Manages configuration templates and their resolution
    /// Implements the Template Method pattern for configuration loading
    /// </summary>
    public class TemplateManager : ITemplateManager
    {
        private readonly Dictionary<string, AgentTemplate> _agentTemplates;
        private readonly Dictionary<string, MapTemplate> _mapTemplates;
        private readonly List<string> _templateSearchPaths;
        
        public TemplateManager()
        {
            _agentTemplates = new Dictionary<string, AgentTemplate>();
            _mapTemplates = new Dictionary<string, MapTemplate>();
            _templateSearchPaths = new List<string>
            {
                "templates",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimArena", "templates")
            };
        }
        
        /// <summary>
        /// Loads all templates from the configured search paths
        /// </summary>
        public void LoadTemplates()
        {
            foreach (var searchPath in _templateSearchPaths)
            {
                if (Directory.Exists(searchPath))
                {
                    LoadTemplatesFromDirectory(searchPath);
                }
            }
        }
        
        /// <summary>
        /// Loads templates from a specific directory
        /// </summary>
        /// <param name="directory">Directory to search for template files</param>
        private void LoadTemplatesFromDirectory(string directory)
        {
            var templateFiles = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
            
            foreach (var file in templateFiles)
            {
                try
                {
                    LoadAgentTemplate(file);
                }
                catch (Exception)
                {
                    try
                    {
                        LoadMapTemplate(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Failed to load template from {file}: {ex.Message}");
                        throw;
                    }
                }
            }
        }
        
        /// <summary>
        /// Loads a specific map template from a file
        /// </summary>
        /// <param name="filePath">Path to the template file</param>
        private void LoadMapTemplate(string filePath)
            {
                var json = File.ReadAllText(filePath);
                var template = JsonSerializer.Deserialize<MapTemplate>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                                                     });
                
                    if (template != null && !string.IsNullOrEmpty(template.TemplateId))
                    {
                        _mapTemplates[template.TemplateId] = template;
                    }
            }
        
        /// <summary>
        /// Loads a specific agent template from a file
        /// </summary>
        /// <param name="filePath">Path to the template file</param>
        private void LoadAgentTemplate(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var template = JsonSerializer.Deserialize<AgentTemplate>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            });
            
            if (template != null && !string.IsNullOrEmpty(template.TemplateId))
            {
                _agentTemplates[template.TemplateId] = template;
            }
        }
        
        /// <summary>
        /// Resolves a template reference to an actual agent configuration
        /// </summary>
        /// <param name="templateRef">Template reference to resolve</param>
        /// <returns>Resolved agent configuration</returns>
        public AgentConfiguration ResolveAgentTemplate(TemplateReference templateRef)
        {
            if (templateRef == null)
                throw new ArgumentNullException(nameof(templateRef));
                
            AgentTemplate template = null;
            
            // Try to find template by ID first
            if (_agentTemplates.TryGetValue(templateRef.TemplatePath, out template))
            {
                // Found by ID
            }
            // Try to load from file path
            else if (File.Exists(templateRef.TemplatePath))
            {
                var json = File.ReadAllText(templateRef.TemplatePath);
                template = JsonSerializer.Deserialize<AgentTemplate>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                throw new FileNotFoundException($"Template not found: {templateRef.TemplatePath}");
            }
            
            // Create instance from template
            var agentConfig = template.CreateInstance();
            
            // Apply overrides
            ApplyOverrides(agentConfig, templateRef.Overrides);
            
            // Override name if specified
            if (!string.IsNullOrEmpty(templateRef.Name))
            {
                agentConfig.Name = templateRef.Name;
            }
            
            return agentConfig;
        }
        
        /// <summary>
        /// Applies property overrides to an agent configuration using reflection
        /// </summary>
        /// <param name="config">Configuration object to modify</param>
        /// <param name="overrides">Dictionary of property paths and values</param>
        private void ApplyOverrides(AgentConfiguration config, Dictionary<string, object> overrides)
        {
            if (overrides == null || overrides.Count == 0)
                return;
                
            foreach (var kvp in overrides)
            {
                ApplyOverride(config, kvp.Key, kvp.Value);
            }
        }
        
        /// <summary>
        /// Applies property overrides to a map configuration using reflection
        /// </summary>
        /// <param name="config">Configuration object to modify</param>
        /// <param name="overrides">Dictionary of property paths and values</param>
        private void ApplyOverrides(MapConfiguration config, Dictionary<string, object> overrides)
        {
            if (overrides == null || overrides.Count == 0)
                return;
                
            foreach (var kvp in overrides)
            {
                ApplyOverride(config, kvp.Key, kvp.Value);
            }
        }
        
        /// <summary>
        /// Applies a single override using property path navigation
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="propertyPath">Dot-separated property path (e.g., "Brain.Awareness")</param>
        /// <param name="value">Value to set</param>
        private void ApplyOverride(object target, string propertyPath, object value)
        {
            var pathParts = propertyPath.Split('.');
            var currentTarget = target;
            
            // Navigate to the parent object
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                var property = currentTarget.GetType().GetProperty(pathParts[i], 
                    BindingFlags.Public | BindingFlags.Instance);
                    
                if (property == null)
                    throw new ArgumentException($"Property '{pathParts[i]}' not found on {currentTarget.GetType().Name}");
                    
                currentTarget = property.GetValue(currentTarget);
                
                if (currentTarget == null)
                    throw new InvalidOperationException($"Property '{pathParts[i]}' is null, cannot navigate further");
            }
            
            // Set the final property
            var finalProperty = currentTarget.GetType().GetProperty(pathParts.Last(),
                BindingFlags.Public | BindingFlags.Instance);
                
            if (finalProperty == null)
                throw new ArgumentException($"Property '{pathParts.Last()}' not found on {currentTarget.GetType().Name}");
                
            // Convert value to appropriate type if needed
            var convertedValue = ConvertValue(value, finalProperty.PropertyType);
            finalProperty.SetValue(currentTarget, convertedValue);
        }
        
        /// <summary>
        /// Converts a value to the target type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Type to convert to</param>
        /// <returns>Converted value</returns>
        private object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return null;
                
            if (targetType.IsAssignableFrom(value.GetType()))
                return value;
                
            // Handle JsonElement from deserialization
            if (value is JsonElement jsonElement)
            {
                return JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType);
            }
            
            // Use Convert.ChangeType for basic type conversions
            return Convert.ChangeType(value, targetType);
        }
        
        /// <summary>
        /// Gets all available agent templates
        /// </summary>
        /// <returns>Collection of available templates</returns>
        public IEnumerable<AgentTemplate> GetAvailableAgentTemplates()
        {
            return _agentTemplates.Values;
        }
        
        /// <summary>
        /// Gets templates by tag
        /// </summary>
        /// <param name="tag">Tag to filter by</param>
        /// <returns>Templates with the specified tag</returns>
        public IEnumerable<AgentTemplate> GetTemplatesByTag(string tag)
        {
            return _agentTemplates.Values.Where(t => t.Tags != null && t.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Resolves a map template reference to an actual map configuration
        /// </summary>
        /// <param name="templateRef">Map template reference to resolve</param>
        /// <returns>Resolved map configuration</returns>
        public MapConfiguration ResolveMapTemplate(MapTemplateReference templateRef)
        {
            if (templateRef == null)
                throw new ArgumentNullException(nameof(templateRef));
                
            MapTemplate template = null;
            
            // Try to find template by ID first
            if (_mapTemplates.TryGetValue(templateRef.TemplatePath, out template))
            {
                // Found by ID
            }
            // Try to load from file path
            else if (File.Exists(templateRef.TemplatePath))
            {
                var json = File.ReadAllText(templateRef.TemplatePath);
                template = JsonSerializer.Deserialize<MapTemplate>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                throw new FileNotFoundException($"Map template not found: {templateRef.TemplatePath}");
            }
            
            // Create instance from template
            var mapConfig = template.CreateMapConfiguration();
            
            // Apply overrides if any
            if (templateRef.Overrides != null && templateRef.Overrides.Count > 0)
            {
                ApplyOverrides(mapConfig, templateRef.Overrides);
            }
            
            return mapConfig;
        }
        
        /// <summary>
        /// Gets all available map templates
        /// </summary>
        /// <returns>Collection of available map templates</returns>
        public IEnumerable<MapTemplate> GetAvailableMapTemplates()
        {
            return _mapTemplates.Values;
        }
        
        /// <summary>
        /// Gets map templates by tag
        /// </summary>
        /// <param name="tag">Tag to filter by</param>
        /// <returns>Map templates with the specified tag</returns>
        public IEnumerable<MapTemplate> GetMapTemplatesByTag(string tag)
        {
            return _mapTemplates.Values.Where(t => 
                t.Tags != null && t.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
        }
     }
 }