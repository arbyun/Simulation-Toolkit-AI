using System;
using System.Collections.Generic;
using System.Linq;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering.RenderStrategies;

namespace SimToolAI.Core.Rendering
{
    /// <summary>
    /// Console-specific implementation of the Scene class
    /// </summary>
    public class ConsoleScene : Scene
    {
        /// <summary>
        /// Creates a new console scene with the specified map
        /// </summary>
        /// <param name="map">Map for the scene</param>
        public ConsoleScene(ISimMap map) : base(map)
        {
        }

        /// <summary>
        /// Updates all entities in the scene
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public override void Update(float deltaTime)
        {
            // Create a copy of the entities list to avoid issues if entities are added/removed during update
            foreach (Entity entity in Entities.ToList())
            {
                entity.Update(deltaTime);
            }
        }

        /// <summary>
        /// Renders the scene to the console
        /// </summary>
        public override void Render()
        {
            if (!RenderRequired)
                return;

            try
            {
                // Render the map first
                if (Map.Renderable != null)
                {
                    Map.Renderable.MarkForRendering();
                    Map.Renderable.Render();
                }

                // Render all entities sorted by render priority
                var renderables = new List<IRenderable>();

                foreach (var entity in Entities.Where(entity => entity.Avatar != null))
                {
                    entity.Avatar.MarkForRendering();
                    renderables.Add(entity.Avatar);
                }

                // Sort renderables by priority and render them
                foreach (var renderable in renderables.OrderBy(r => r.RenderPriority))
                {
                    renderable.Render();
                }

                // Reset the render flag
                RenderRequired = false;
            }
            catch (Exception ex)
            {
                // Ignore exceptions related to console buffer size changes
                if (ex is not (ArgumentOutOfRangeException or System.IO.IOException))
                    throw;
            }
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
                case "GetMap":
                    return (T)Map;
                case "SetRenderRequired":
                    RenderRequired = (bool)parameters[0];
                    return default;
                case "IsRenderRequired":
                    return (T)(object)RenderRequired;
                case "GetPlayer":
                    return (T)(object)GetEntities<Player>().FirstOrDefault();
                case "GetEntitiesAt":
                    int x = (int)parameters[0];
                    int y = (int)parameters[1];
                    return (T)(object)Entities.Where(e => e.X == x && e.Y == y).ToList();
                default:
                    throw new ArgumentException($"Unknown query: {query}");
            }
        }
    }
}