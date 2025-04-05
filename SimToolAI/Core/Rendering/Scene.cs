using System.Collections.Generic;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;

namespace SimToolAI.Core.Rendering
{
    public abstract class Scene
    {
        protected ISimMap Map { get; set; }
        protected List<Entity> Entities { get; set; } = new List<Entity>();
    
        public Scene(ISimMap map)
        {
            Map = map;
        }
    
        /// <summary>
        /// Adds an entity to the scene
        /// </summary>
        public virtual void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }
    
        /// <summary>
        /// Removes an entity from the scene
        /// </summary>
        public virtual void RemoveEntity(Entity entity)
        {
            Entities.Remove(entity);

            // Trigger a render update when an entity is removed
            QueryScene<bool>("SetRenderRequired", true);
        }
    
        /// <summary>
        /// Updates the scene state
        /// </summary>
        public abstract void Update(float deltaTime);
    
        /// <summary>
        /// Renders the scene
        /// </summary>
        public abstract void Render();
    
        /// <summary>
        /// Queries the scene for data
        /// </summary>
        public abstract T QueryScene<T>(string query, params object[] parameters);

    }
}