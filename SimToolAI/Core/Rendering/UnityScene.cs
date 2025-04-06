using System;
using System.Collections.Generic;
using System.Linq;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;

namespace SimToolAI.Examples.Unity
{
    /// <summary>
    /// Unity-specific implementation of the IScene interface
    /// </summary>
    public class UnityScene : IScene
    {
        #region Properties
        
        /// <summary>
        /// Gets the map associated with this scene
        /// </summary>
        public ISimMap Map { get; }
        
        /// <summary>
        /// List of entities in the scene
        /// </summary>
        private readonly List<Entity> _entities = new List<Entity>();
        
        /// <summary>
        /// Whether the scene needs to be rendered
        /// </summary>
        private bool _renderRequired = true;
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Creates a new Unity scene with the specified map
        /// </summary>
        /// <param name="map">Map for the scene</param>
        public UnityScene(ISimMap map)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
        }
        
        #endregion
        
        #region IScene Implementation
        
        /// <summary>
        /// Adds an entity to the scene
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public void AddEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
                _renderRequired = true;
            }
        }
        
        /// <summary>
        /// Removes an entity from the scene
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public void RemoveEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            if (_entities.Remove(entity))
            {
                _renderRequired = true;
            }
        }
        
        /// <summary>
        /// Updates the scene state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public void Update(float deltaTime)
        {
            // Create a copy of the entities list to avoid issues if entities are added/removed during update
            foreach (Entity entity in _entities.ToList())
            {
                entity.Update(deltaTime);
            }
        }
        
        /// <summary>
        /// Renders the scene
        /// </summary>
        public void Render()
        {
            // Unity handles rendering through the GameObject system
            // This method is kept for compatibility with the IScene interface
            _renderRequired = false;
        }
        
        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">ID of the entity to get</param>
        /// <returns>The entity with the specified ID, or null if not found</returns>
        public Entity GetEntity(Guid id)
        {
            return _entities.FirstOrDefault(e => e.Id == id);
        }
        
        /// <summary>
        /// Gets an entity at the specified position
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>The entity at the specified position, or null if not found</returns>
        public Entity GetEntityAt(int x, int y)
        {
            return _entities.FirstOrDefault(e => e.X == x && e.Y == y);
        }
        
        /// <summary>
        /// Gets all entities of the specified type
        /// </summary>
        /// <typeparam name="T">Type of entities to get</typeparam>
        /// <returns>All entities of the specified type</returns>
        public IEnumerable<T> GetEntities<T>() where T : Entity
        {
            return _entities.OfType<T>();
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
        public T QueryScene<T>(string query, params object[] parameters)
        {
            switch (query)
            {
                case "GetMap":
                    return (T)(object)Map;
                case "SetRenderRequired":
                    _renderRequired = (bool)parameters[0];
                    return default;
                case "IsRenderRequired":
                    return (T)(object)_renderRequired;
                case "GetPlayer":
                    return (T)(object)GetEntities<Player>().FirstOrDefault();
                case "GetEntitiesAt":
                    int x = (int)parameters[0];
                    int y = (int)parameters[1];
                    return (T)(object)_entities.Where(e => e.X == x && e.Y == y).ToList();
                default:
                    throw new ArgumentException($"Unknown query: {query}");
            }
        }
        
        #endregion
    }
}