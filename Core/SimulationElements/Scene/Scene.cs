using SimArena.Core.Entities;
using SimArena.Core.SimulationElements.Map;

namespace SimArena.Core.SimulationElements.Scene
{
    public abstract class Scene: IScene
    {
        /// <summary>
        /// Event raised when an entity is removed from the scene
        /// </summary>
        public EventHandler<EntityEventArgs>? EntityRemoved;

        /// <summary>
        /// Gets the map associated with this scene
        /// </summary>
        public IMap Map { get; }

        /// <summary>
        /// List of entities in the scene
        /// </summary>
        protected readonly List<Entity> Entities = new();

        /// <summary>
        /// Whether the scene needs to be rendered
        /// </summary>
        protected bool RenderRequired = true;
        
        /// <summary>
        /// Creates a new scene with the specified map
        /// </summary>
        /// <param name="map">Map for the scene</param>
        protected Scene(IMap map)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
        }
        
        /// <summary>
        /// Adds an entity to the scene
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public virtual void AddEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!Entities.Contains(entity))
            {
                Entities.Add(entity);
                RenderRequired = true;
            }
        }

        /// <summary>
        /// Removes an entity from the scene
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public virtual void RemoveEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (Entities.Remove(entity))
            {
                RenderRequired = true;

                // Raise the EntityRemoved event
                EntityRemoved?.Invoke(this, new EntityEventArgs(entity));
            }
        }

        /// <summary>
        /// Updates the scene state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">ID of the entity to get</param>
        /// <returns>The entity with the specified ID, or null if not found</returns>
        public virtual Entity? GetEntity(Guid id)
        {
            return Entities.FirstOrDefault(e => e.Id == id);
        }

        /// <summary>
        /// Gets an entity at the specified position
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>The entity at the specified position, or null if not found</returns>
        public virtual Entity? GetEntityAt(int x, int y)
        {
            return Entities.FirstOrDefault(e => e.X == x && e.Y == y);
        }

        /// <summary>
        /// Gets all entities of the specified type
        /// </summary>
        /// <typeparam name="T">Type of entities to get</typeparam>
        /// <returns>All entities of the specified type</returns>
        public virtual IEnumerable<T> GetEntities<T>() where T : Entity?
        {
            return Entities.OfType<T>();
        }

    }
}