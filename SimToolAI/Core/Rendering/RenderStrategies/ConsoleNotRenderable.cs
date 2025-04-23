using SimToolAI.Core.Entities;

namespace SimToolAI.Core.Rendering.RenderStrategies
{
    /// <summary>
    /// Simple render strategy that does nothing.
    /// </summary>
    public class ConsoleNotRenderable: RenderableBase
    {
        public override void Connect(Entity entity)
        {
            return;
        }

        public override void Render()
        {
            return;
        }
    }
}