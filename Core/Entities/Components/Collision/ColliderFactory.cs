using SimArena.Core.SimulationElements.Map;

namespace SimArena.Core.Entities.Components.Collision
{
    /// <summary>
    /// Factory for creating colliders based on map type and entity properties
    /// </summary>
    public static class ColliderFactory
    {
        /// <summary>
        /// Creates an appropriate collider for the entity based on the map type
        /// </summary>
        /// <param name="entity">Entity to create a collider for</param>
        /// <param name="map">Map the entity exists in</param>
        /// <param name="width">Width of the collider (grid cells or units)</param>
        /// <param name="height">Height of the collider (grid cells or units)</param>
        /// <returns>An appropriate collider for the entity</returns>
        public static ICollider? CreateCollider(Entity entity, IMap map, int width = 1, int height = 1)
        {
            // Create a grid collider for grid maps
            if (map is GridMap)
            {
                return new GridCollider(entity, width, height);
            }
            
            // Create a continuous collider for continuous maps
            if (map is ContinuousMap)
            {
                return new Collider(entity, width, height);
            }
            
            // Default to a grid collider with size 1x1
            return new GridCollider(entity);
        }
    }
}