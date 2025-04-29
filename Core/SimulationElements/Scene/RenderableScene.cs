using SimArena.Core.SimulationElements.Map;

namespace SimArena.Core.SimulationElements.Scene
{
    public abstract class RenderableScene: Scene
    {
        public RenderableScene(IMap map) : base(map)
        {
        }

        public abstract override void Update(float deltaTime);
        public abstract void Render();
    }
}