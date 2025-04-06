using SimToolAI.Utilities;

namespace SimToolAI.Core.Rendering.RenderStrategies
{
    /// <summary>
    /// Base class for renderable objects implementing the Strategy pattern
    /// </summary>
    public abstract class RenderableBase : IRenderable
    {
        /// <summary>
        /// Settings for the renderable
        /// </summary>
        protected readonly Data Settings;
        
        /// <summary>
        /// Whether this object needs to be rendered
        /// </summary>
        private bool _needsRendering = true;
        
        /// <summary>
        /// Gets whether this object needs to be rendered
        /// </summary>
        public bool NeedsRendering => _needsRendering;
        
        /// <summary>
        /// Gets the rendering priority (higher values are rendered later/on top)
        /// </summary>
        public virtual int RenderPriority => 0;
        
        /// <summary>
        /// Creates a new renderable with the specified settings
        /// </summary>
        /// <param name="settings">Settings for the renderable</param>
        protected RenderableBase(Data settings)
        {
            Settings = settings ?? new Data();
        }
        
        /// <summary>
        /// Creates a new renderable with default settings
        /// </summary>
        protected RenderableBase() : this(new Data())
        {
        }
        
        /// <summary>
        /// Renders the object to the current rendering context
        /// </summary>
        public abstract void Render();
        
        /// <summary>
        /// Marks this object as needing to be rendered
        /// </summary>
        public void MarkForRendering()
        {
            _needsRendering = true;
        }
        
        /// <summary>
        /// Marks this object as having been rendered
        /// </summary>
        protected void MarkAsRendered()
        {
            _needsRendering = false;
        }
    }
}