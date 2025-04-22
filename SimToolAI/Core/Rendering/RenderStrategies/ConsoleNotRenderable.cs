namespace SimToolAI.Core.Rendering.RenderStrategies
{
    /// <summary>
    /// Simple render strategy that does nothing.
    /// </summary>
    public class ConsoleNotRenderable: RenderableBase
    {
        public override void Render()
        {
            return;
        }
    }
}