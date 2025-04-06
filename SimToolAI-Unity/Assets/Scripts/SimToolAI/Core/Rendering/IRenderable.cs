namespace SimToolAI.Core.Rendering
{
    /// <summary>
    /// Interface for objects that can be rendered
    /// Implements the Strategy pattern for rendering
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// Renders the object to the current rendering context
        /// </summary>
        void Render();

        /// <summary>
        /// Gets whether this object needs to be rendered
        /// </summary>
        bool NeedsRendering { get; }

        /// <summary>
        /// Marks this object as needing to be rendered
        /// </summary>
        void MarkForRendering();

        /// <summary>
        /// Gets the rendering priority (higher values are rendered later/on top)
        /// </summary>
        int RenderPriority { get; }
    }
}