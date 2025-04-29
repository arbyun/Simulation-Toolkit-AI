using SimArena.Core.Entities;
using SimArena.Core.SimulationElements.Map;

namespace SimArena.Core.SimulationElements.Scene
{
    public interface IScene
    {
        /// <summary>
        /// Gets the map associated with this scene
        /// </summary>
        IMap Map { get; }
        
        /// <summary>
        /// Adds an entity to the scene
        /// </summary>
        /// <param name="entity">Entity to add</param>
        void AddEntity(Entity entity);
        
        /// <summary>
        /// Removes an entity from the scene
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        void RemoveEntity(Entity entity);
        
        /// <summary>
        /// Updates the scene state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">ID of the entity to get</param>
        /// <returns>The entity with the specified ID, or null if not found</returns>
        Entity? GetEntity(System.Guid id);
        
        /// <summary>
        /// Gets an entity at the specified position
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>The entity at the specified position, or null if not found</returns>
        Entity? GetEntityAt(int x, int y);
        
        /// <summary>
        /// Gets all entities of the specified type
        /// </summary>
        /// <typeparam name="T">Type of entities to get</typeparam>
        /// <returns>All entities of the specified type</returns>
        IEnumerable<T> GetEntities<T>() where T : Entity?;
    }
}