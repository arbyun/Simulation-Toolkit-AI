namespace SimArena.Serialization.Configuration.Templates
{
    /// <summary>
    /// Interface for configuration templates that can be referenced and extended
    /// </summary>
    public interface IConfigurationTemplate
    {
        /// <summary>
        /// Unique identifier for this template
        /// </summary>
        string TemplateId { get; set; }
        
        /// <summary>
        /// Description of what this template provides
        /// </summary>
        string Description { get; set; }
        
        /// <summary>
        /// Version of the template for backwards compatibility
        /// </summary>
        string Version { get; set; }
    }
}