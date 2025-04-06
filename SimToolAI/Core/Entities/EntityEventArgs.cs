using System;

namespace SimToolAI.Core.Entities
{
    /// <summary>
    /// Event arguments for entity-related events.
    /// </summary>
    public class EntityEventArgs : EventArgs
    {
        /// <summary>
        /// The entity associated with the event
        /// </summary>
        public Entity Entity { get; }

        /// <summary>
        /// Creates a new instance of EntityEventArgs
        /// </summary>
        /// <param name="entity">The entity associated with the event</param>
        public EntityEventArgs(Entity entity)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        }
    }
}