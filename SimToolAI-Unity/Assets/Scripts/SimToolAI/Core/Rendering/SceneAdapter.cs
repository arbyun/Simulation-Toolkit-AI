using System;
using System.Collections.Generic;
using System.Linq;
using SimToolAI.Core.Entities;

namespace SimToolAI.Core.Rendering
{
    /// <summary>
    /// Adapter class that wraps an IScene implementation and provides a Scene-compatible interface
    /// </summary>
    public class SceneAdapter : Scene
    {
        private readonly IScene _wrappedScene;

        /// <summary>
        /// Creates a new scene adapter that wraps the specified scene
        /// </summary>
        /// <param name="scene">Scene to wrap</param>
        public SceneAdapter(IScene scene) 
            : base(scene.Map)
        {
            _wrappedScene = scene;
        }

        /// <summary>
        /// Adds an entity to the scene
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public override void AddEntity(Entity entity)
        {
            _wrappedScene.AddEntity(entity);
        }

        /// <summary>
        /// Removes an entity from the scene
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public override void RemoveEntity(Entity entity)
        {
            _wrappedScene.RemoveEntity(entity);
        }

        /// <summary>
        /// Updates the scene state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public override void Update(float deltaTime)
        {
            _wrappedScene.Update(deltaTime);
        }

        /// <summary>
        /// Renders the scene
        /// </summary>
        public override void Render()
        {
            _wrappedScene.Render();
        }

        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">ID of the entity to get</param>
        /// <returns>The entity with the specified ID, or null if not found</returns>
        public override Entity GetEntity(Guid id)
        {
            return _wrappedScene.GetEntity(id);
        }

        /// <summary>
        /// Gets an entity at the specified position
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>The entity at the specified position, or null if not found</returns>
        public override Entity GetEntityAt(int x, int y)
        {
            return _wrappedScene.GetEntityAt(x, y);
        }

        /// <summary>
        /// Gets all entities of the specified type
        /// </summary>
        /// <typeparam name="T">Type of entities to get</typeparam>
        /// <returns>All entities of the specified type</returns>
        public override IEnumerable<T> GetEntities<T>()
        {
            return _wrappedScene.GetEntities<T>();
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
            // Handle common queries
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
            }

            // For other queries, try to forward to the wrapped scene if it supports QueryScene
            if (_wrappedScene is Scene sceneImpl)
            {
                return sceneImpl.QueryScene<T>(query, parameters);
            }

            throw new NotSupportedException($"Query '{query}' is not supported by this scene adapter");
        }
    }
}