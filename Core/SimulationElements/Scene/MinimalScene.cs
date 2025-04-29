using SimArena.Core.SimulationElements.Map;

namespace SimArena.Core.SimulationElements.Scene
{
    public class MinimalScene: Scene
    {
        /// <summary>
        /// Creates a new minimal scene with the specified map
        /// </summary>
        /// <param name="map">Map for the scene</param>
        public MinimalScene(IMap map) : base(map)
        {
        }

        /// <summary>
        /// Updates the scene state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public override void Update(float deltaTime)
        {
            // Update all entities
            foreach (var entity in Entities.ToArray())
            {
                entity.Update(deltaTime);
            }
            
            // Reset the render flag
            RenderRequired = false;
        }
    }
}