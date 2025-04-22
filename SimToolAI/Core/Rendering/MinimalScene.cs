using SimToolAI.Core.Map;

namespace SimToolAI.Core.Rendering
{
    /// <summary>
    /// Minimal scene implementation for offline simulations
    /// </summary>
    public class MinimalScene : Scene
    {
        /// <summary>
        /// Creates a new minimal scene with the specified map
        /// </summary>
        /// <param name="map">Map for the scene</param>
        public MinimalScene(ISimMap map) : base(map)
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

        /// <summary>
        /// Renders the scene (does nothing in minimal scene)
        /// </summary>
        public override void Render()
        {
            // No rendering in minimal scene
        }

        /// <summary>
        /// Queries the scene for data
        /// </summary>
        /// <typeparam name="T">Type of data to return</typeparam>
        /// <param name="query">Query string</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>The result of the query</returns>
        public override T QueryScene<T>(string query, params object[] parameters)
        {
            switch (query)
            {
                case "IsRenderRequired":
                    return (T)(object)RenderRequired;
                    
                case "SetRenderRequired":
                    if (parameters.Length > 0 && parameters[0] is bool renderRequired)
                    {
                        RenderRequired = renderRequired;
                    }
                    return (T)(object)RenderRequired;
                    
                case "GetMap":
                    return (T)Map;
                    
                default:
                    return default;
            }
        }
    }
}