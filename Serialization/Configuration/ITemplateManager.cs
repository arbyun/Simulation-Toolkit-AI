using System.Collections.Generic;

namespace SimArena.Serialization.Configuration
{
    /// <summary>
    /// Interface for template management operations
    /// Follows the Interface Segregation Principle (ISP)
    /// </summary>
    public interface ITemplateManager
    {
        /// <summary>
        /// Loads all templates from configured search paths
        /// </summary>
        void LoadTemplates();
        
        /// <summary>
        /// Resolves a template reference to an actual agent configuration
        /// </summary>
        /// <param name="templateRef">Template reference to resolve</param>
        /// <returns>Resolved agent configuration</returns>
        AgentConfiguration ResolveAgentTemplate(TemplateReference templateRef);
        
        /// <summary>
        /// Gets all available agent templates
        /// </summary>
        /// <returns>Collection of available templates</returns>
        IEnumerable<AgentTemplate> GetAvailableAgentTemplates();
        
        /// <summary>
        /// Gets templates filtered by tag
        /// </summary>
        /// <param name="tag">Tag to filter by</param>
        /// <returns>Templates with the specified tag</returns>
        IEnumerable<AgentTemplate> GetTemplatesByTag(string tag);
    }
}