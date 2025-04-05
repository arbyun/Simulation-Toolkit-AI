using SimToolAI.Utilities;

namespace SimToolAI.Core.Rendering
{
    public interface IRenderable
    {
        /// <summary>
        /// Initialize the renderer
        /// </summary>
        //void Initialize(Data arguments);

        /// <summary>
        /// Present the rendered frame
        /// </summary>
        void Render();
    }
}