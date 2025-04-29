using System.Numerics;
using SimArena.Core.SimulationElements.Map;

namespace SimArena.Core.Entities.Components.Collision
{
    public interface ICollider
    {
        /// <summary>
        /// The entity this collider belongs to
        /// </summary>
        Entity Owner { get; }
        
        /// <summary>
        /// Checks if this collider can move to a given position without colliding with anything.
        /// </summary>
        /// <param name="position">The position to move to</param>
        /// <returns>If the movement is possible</returns>
        public bool CanMoveTo(Vector3 position, IMap map);

        /// <summary>
        /// Checks if this collider can move to a given position without colliding with anything.
        /// </summary>
        /// <param name="x">The new X coordinate</param>
        /// <param name="y">The new Y coordinate</param>
        /// <returns>If the movement is possible</returns>
        public bool CanMoveTo(int x, int y, IMap map);
        
        /// <summary>
        /// Assigns an entity as the owner of this collider. This allows the collider to perform checks
        /// on its own position relative to other entities.
        /// </summary>
        /// <param name="character">The entity to assign as the owner</param>
        public void AssignOwner(Entity character);
        
        /// <summary>
        /// Checks if this collider intersects with another collider
        /// </summary>
        /// <param name="other">The other collider to check against</param>
        /// <returns>True if the colliders intersect, false otherwise</returns>
        bool Intersects(ICollider other);
        
        /// <summary>
        /// Checks if this collider would intersect with another collider if the owner moved to the specified position
        /// </summary>
        /// <param name="other">The other collider to check against</param>
        /// <param name="newX">New X position to check</param>
        /// <param name="newY">New Y position to check</param>
        /// <returns>True if the colliders would intersect, false otherwise</returns>
        bool WouldIntersect(ICollider other, int newX, int newY);
        
        /// <summary>
        /// Checks if this collider would intersect with any collider in the collection if the owner moved to the specified position
        /// </summary>
        /// <param name="colliders">Collection of colliders to check against</param>
        /// <param name="newX">New X position to check</param>
        /// <param name="newY">New Y position to check</param>
        /// <returns>True if the collider would intersect with any collider in the collection, false otherwise</returns>
        bool WouldIntersectAny(IEnumerable<ICollider> colliders, int newX, int newY);
        
        /// <summary>
        /// Checks if the specified position is valid for this collider (within map bounds and not colliding with obstacles)
        /// </summary>
        /// <param name="map">The map to check against</param>
        /// <param name="x">X position to check</param>
        /// <param name="y">Y position to check</param>
        /// <returns>True if the position is valid, false otherwise</returns>
        bool IsValidPosition(IMap map, int x, int y);

        /// <summary>
        /// Checks if the specified position is valid for this collider
        /// (within map bounds and not colliding with obstacles).
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool IsValidPositionFor(Entity entity, IMap map, int x, int y);
        
        /// <summary>
        /// Gets all cells occupied by this collider
        /// </summary>
        /// <returns>Collection of (x,y) coordinates occupied by this collider</returns>
        IEnumerable<(int x, int y)> GetOccupiedCells();
        
        /// <summary>
        /// Gets all cells that would be occupied by this collider if the owner moved to the specified position
        /// </summary>
        /// <param name="newX">New X position</param>
        /// <param name="newY">New Y position</param>
        /// <returns>Collection of (x,y) coordinates that would be occupied</returns>
        IEnumerable<(int x, int y)> GetFutureOccupiedCells(int newX, int newY);
    }
}