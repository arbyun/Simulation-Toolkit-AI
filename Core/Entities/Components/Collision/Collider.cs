using System.Numerics;
using SimArena.Core.SimulationElements.Map;

namespace SimArena.Core.Entities.Components.Collision
{
    public class Collider: ICollider
    {
        public (int x, int y) Position => (Owner.Position.X, Owner.Position.Y);
        
        /// <summary>
        /// The entity this collider belongs to
        /// </summary>
        public Entity Owner { get; private set; }

        /// <summary>
        /// Width of the collider's bounding box
        /// </summary>
        public float Width { get; }
        
        /// <summary>
        /// Height of the collider's bounding box
        /// </summary>
        public float Height { get; }
        
        /// <summary>
        /// Offset of the collider from the entity's position
        /// </summary>
        public Vector2 Offset { get; }

        /// <summary>
        /// Creates a new continuous collider with the specified parameters
        /// </summary>
        /// <param name="owner">Entity this collider belongs to</param>
        /// <param name="width">Width of the collider's bounding box</param>
        /// <param name="height">Height of the collider's bounding box</param>
        /// <param name="offsetX">X offset from the entity's position</param>
        /// <param name="offsetY">Y offset from the entity's position</param>
        public Collider(Entity owner, float width = 1.0f, float height = 1.0f, float offsetX = 0, float offsetY = 0)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Width = Math.Max(0.1f, width);   // Ensure minimum size
            Height = Math.Max(0.1f, height); // Ensure minimum size
            Offset = new Vector2(offsetX, offsetY);
        }
        
        /// <summary>
        /// Creates a new continuous collider with the specified parameters
        /// </summary>
        /// <param name="width">Width of the collider's bounding box</param>
        /// <param name="height">Height of the collider's bounding box</param>
        /// <param name="offsetX">X offset from the entity's position</param>
        /// <param name="offsetY">Y offset from the entity's position</param>
        public Collider(float width, float height, float offsetX = 0, float offsetY = 0)
        {
            Width = Math.Max(0.1f, width);   // Ensure minimum size
            Height = Math.Max(0.1f, height); // Ensure minimum size
            Offset = new Vector2(offsetX, offsetY);
        }
        
        /// <summary>
        /// Gets the bounds of this collider
        /// </summary>
        /// <param name="x">X position of the entity</param>
        /// <param name="y">Y position of the entity</param>
        /// <returns>Tuple containing (minX, minY, maxX, maxY)</returns>
        private (float minX, float minY, float maxX, float maxY) GetBounds(float x, float y)
        {
            float minX = x + Offset.X - Width / 2;
            float minY = y + Offset.Y - Height / 2;
            float maxX = minX + Width;
            float maxY = minY + Height;
            
            return (minX, minY, maxX, maxY);
        }
        
        /// <summary>
        /// Gets the bounds of this collider at the entity's current position
        /// </summary>
        /// <returns>Tuple containing (minX, minY, maxX, maxY)</returns>
        private (float minX, float minY, float maxX, float maxY) GetBounds()
        {
            return GetBounds(Owner.X, Owner.Y);
        }
        
        /// <summary>
        /// Checks if this collider intersects with another collider
        /// </summary>
        /// <param name="other">The other collider to check against</param>
        /// <returns>True if the colliders intersect, false otherwise</returns>
        public bool Intersects(ICollider other)
        {
            if (other is Collider continuousCollider)
            {
                // Get bounds for both colliders
                var thisBounds = GetBounds();
                var otherBounds = continuousCollider.GetBounds();
                
                // Check for intersection using AABB (Axis-Aligned Bounding Box) collision detection
                return thisBounds.minX < otherBounds.maxX &&
                       thisBounds.maxX > otherBounds.minX &&
                       thisBounds.minY < otherBounds.maxY &&
                       thisBounds.maxY > otherBounds.minY;
            }
            else if (other is GridCollider gridCollider)
            {
                // For grid colliders, we'll check if any of their cells intersect with our bounds
                var thisBounds = GetBounds();
                var gridCells = gridCollider.GetOccupiedCells();
                
                // Check if any grid cell is within our bounds
                return gridCells.Any(cell => 
                    cell.x >= Math.Floor(thisBounds.minX) && 
                    cell.x <= Math.Ceiling(thisBounds.maxX) &&
                    cell.y >= Math.Floor(thisBounds.minY) && 
                    cell.y <= Math.Ceiling(thisBounds.maxY));
            }
            
            return false;
        }

        /// <summary>
        /// Checks if this collider would intersect with another collider if the owner moved to the specified position
        /// </summary>
        /// <param name="other">The other collider to check against</param>
        /// <param name="newX">New X position to check</param>
        /// <param name="newY">New Y position to check</param>
        /// <returns>True if the colliders would intersect, false otherwise</returns>
        public bool WouldIntersect(ICollider other, int newX, int newY)
        {
            if (other is Collider continuousCollider)
            {
                // Get bounds for both colliders
                var thisBounds = GetBounds(newX, newY);
                var otherBounds = continuousCollider.GetBounds();
                
                // Check for intersection using AABB collision detection
                return thisBounds.minX < otherBounds.maxX &&
                       thisBounds.maxX > otherBounds.minX &&
                       thisBounds.minY < otherBounds.maxY &&
                       thisBounds.maxY > otherBounds.minY;
            }
            else if (other is GridCollider gridCollider)
            {
                // For grid colliders, we'll check if any of their cells intersect with our bounds
                var thisBounds = GetBounds(newX, newY);
                var gridCells = gridCollider.GetOccupiedCells();
                
                // Check if any grid cell is within our bounds
                return gridCells.Any(cell => 
                    cell.x >= Math.Floor(thisBounds.minX) && 
                    cell.x <= Math.Ceiling(thisBounds.maxX) &&
                    cell.y >= Math.Floor(thisBounds.minY) && 
                    cell.y <= Math.Ceiling(thisBounds.maxY));
            }
            
            return false;
        }

        /// <summary>
        /// Checks if this collider would intersect with any collider in the collection
        /// if the owner moved to the specified position
        /// </summary>
        /// <param name="colliders">Collection of colliders to check against</param>
        /// <param name="newX">New X position to check</param>
        /// <param name="newY">New Y position to check</param>
        /// <returns>True if the collider would intersect with any collider in the collection,
        /// false otherwise</returns>
        public bool WouldIntersectAny(IEnumerable<ICollider> colliders, int newX, int newY)
        {
            return colliders.Any(collider => 
                !collider.Owner.Equals(Owner) && // Skip self
                WouldIntersect(collider, newX, newY));
        }

        /// <summary>
        /// Checks if the specified position is valid for this collider
        /// (within map bounds and not colliding with obstacles)
        /// </summary>
        /// <param name="map">The map to check against</param>
        /// <param name="x">X position to check</param>
        /// <param name="y">Y position to check</param>
        /// <returns>True if the position is valid, false otherwise</returns>
        public bool IsValidPosition(IMap map, int x, int y)
        {
            // For continuous maps, we need to check if the bounding box is entirely within the map
            // and doesn't intersect with any non-walkable cells
            var bounds = GetBounds(x, y);
            
            // Check if the bounds are within the map
            if (bounds.minX < 0 || bounds.minY < 0 || 
                bounds.maxX >= map.Width || bounds.maxY >= map.Height)
            {
                return false;
            }
            
            // Get all grid cells that the bounding box overlaps
            var cells = GetFutureOccupiedCells(x, y);
            
            // Check if all cells are walkable
            return cells.All(cell => map.IsWalkable(cell.x, cell.y));
        }

        public bool IsValidPositionFor(Entity entity, IMap map, int x, int y)
        {
            // For continuous maps, we need to check if the bounding box is entirely within the map
            // and doesn't intersect with any non-walkable cells
            var bounds = GetBounds(x, y);
            
            // Check if the bounds are within the map
            if (bounds.minX < 0 || bounds.minY < 0 || 
                bounds.maxX >= map.Width || bounds.maxY >= map.Height)
            {
                return false;
            }
            
            // Get all grid cells that the bounding box overlaps
            var cells = GetFutureOccupiedCells(x, y);
            
            //If cells are occupied by us, then we can move there
            var validCells = cells.Where(cell => 
                map.IsOccupiedBy(cell.x, cell.y)!.Equals(entity) || 
                map.IsOccupiedBy(cell.x, cell.y) == null);
            
            return validCells.Contains((x, y));
        }

        /// <summary>
        /// Gets all cells occupied by this collider
        /// </summary>
        /// <returns>Collection of (x,y) coordinates occupied by this collider</returns>
        public IEnumerable<(int x, int y)> GetOccupiedCells()
        {
            return GetFutureOccupiedCells(Owner.X, Owner.Y);
        }

        /// <summary>
        /// Gets all cells that would be occupied by this collider if the owner moved to the specified position
        /// </summary>
        /// <param name="newX">New X position</param>
        /// <param name="newY">New Y position</param>
        /// <returns>Collection of (x,y) coordinates that would be occupied</returns>
        public IEnumerable<(int x, int y)> GetFutureOccupiedCells(int newX, int newY)
        {
            var cells = new HashSet<(int x, int y)>();
            var bounds = GetBounds(newX, newY);
            
            // Convert floating-point bounds to integer grid coordinates
            int minGridX = (int)Math.Floor(bounds.minX);
            int minGridY = (int)Math.Floor(bounds.minY);
            int maxGridX = (int)Math.Ceiling(bounds.maxX);
            int maxGridY = (int)Math.Ceiling(bounds.maxY);
            
            // Add all grid cells that the bounding box overlaps
            for (int x = minGridX; x < maxGridX; x++)
            {
                for (int y = minGridY; y < maxGridY; y++)
                {
                    cells.Add((x, y));
                }
            }
            
            return cells;
        }
        
        public bool CanMoveTo(Vector3 position, IMap map)
        {
            return IsValidPosition(map, (int)position.X, (int)position.Y);
        }

        public bool CanMoveTo(int x, int y, IMap map)
        {
            return IsValidPosition(map, x, y);
        }

        public void AssignOwner(Entity character)
        {
            Owner = character;
        }
    }
}