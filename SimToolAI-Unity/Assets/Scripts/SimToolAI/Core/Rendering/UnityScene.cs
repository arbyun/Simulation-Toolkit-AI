using System;
using System.Linq;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering.RenderStrategies;

namespace SimToolAI.Core.Rendering
{
    /// <summary>
    /// Unity-specific implementation of the IScene interface
    /// </summary>
    public class UnityScene : Scene
    {
        #region Constructors
        
        /// <summary>
        /// Creates a new Unity scene with the specified map
        /// </summary>
        /// <param name="map">Map for the scene</param>
        public UnityScene(ISimMap map): base(map)
        {
        }
        
        #endregion
        
        #region IScene Implementation
        
        /// <summary>
        /// Updates the scene state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public override void Update(float deltaTime)
        {
            // Create a copy of the entities list to avoid issues if entities
            // are added/removed during update
            foreach (Entity entity in Entities.ToList())
            {
                entity.Update(deltaTime);
            }
        }
        
        /// <summary>
        /// Renders the scene
        /// </summary>
        public override void Render()
        {
            RenderRequired = false;

            foreach (var entity in Entities.Where(entity => entity.Avatar != null))
            {
                entity.Avatar.Render();
            }
        }

        public override void RemoveEntity(Entity entity)
        {
            base.RemoveEntity(entity);

            if (entity.Avatar is UnityEntityRenderable)
            {
                EntityRemoved?.Invoke(this, new EntityEventArgs(entity));
            }
        }

        #endregion
        
        #region Query System
        
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
                    return (T)(object)Map;
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
        
        #endregion
    }
}