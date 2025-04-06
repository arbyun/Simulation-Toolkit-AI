using System;
using SimToolAI.Utilities;

namespace SimToolAI.Core.Rendering.RenderStrategies
{
    /// <summary>
    /// Renderable strategy for Unity-based rendering (stub for future implementation)
    /// </summary>
    public class UnityEntityRenderable : RenderableBase
    {
        /// <summary>
        /// Creates a new Unity renderable with the specified settings
        /// </summary>
        /// <param name="settings">Settings for the renderable</param>
        public UnityEntityRenderable(Data settings) : base(settings)
        {
        }

        /// <summary>
        /// Creates a new Unity renderable with default settings
        /// </summary>
        public UnityEntityRenderable() : base()
        {
        }

        /// <summary>
        /// Renders the object using Unity (not implemented)
        /// </summary>
        public override void Render()
        {
            // This is a stub for future implementation
            // In a real implementation, this would use Unity's rendering system
            throw new NotImplementedException("Unity rendering is not implemented");
        }
    }
}