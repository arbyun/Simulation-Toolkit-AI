using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SimArena.Core.SimulationElements.Map;

namespace SimArena.Core.Entities.Components.Collision
{
    public class GridCollider: ICollider
    {
        public (int x, int y) Position => (Owner.Position.X, Owner.Position.Y);
        
        /// <summary>
        /// Width of the collider in grid cells
        /// </summary>
        public int Width { get; }
        
        /// <summary>
        /// Height of the collider in grid cells
        /// </summary>
        public int Height { get; }
        
        /// <summary>
        /// Offset of the collider from the entity's position (in grid cells)
        /// </summary>
        public (int x, int y) Offset { get; }
        
        public Entity Owner { get; private set; }

        /// <summary>
        /// Creates a new grid collider with the specified parameters
        /// </summary>
        /// <param name="owner">Entity this collider belongs to</param>
        /// <param name="width">Width of the collider in grid cells</param>
        /// <param name="height">Height of the collider in grid cells</param>
        /// <param name="offsetX">X offset from the entity's position</param>
        /// <param name="offsetY">Y offset from the entity's position</param>
        public GridCollider(Entity owner, int width = 1, int height = 1, int offsetX = 0, int offsetY = 0)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Width = Math.Max(1, width);  // Ensure at least 1 cell width
            Height = Math.Max(1, height); // Ensure at least 1 cell height
            Offset = (offsetX, offsetY);
        }
        
        /// <summary>
        /// Creates a new grid collider with the specified parameters
        /// </summary>
        /// <param name="width">Width of the collider in grid cells</param>
        /// <param name="height">Height of the collider in grid cells</param>
        /// <param name="offsetX">X offset from the entity's position</param>
        /// <param name="offsetY">Y offset from the entity's position</param>
        public GridCollider(int width, int height, int offsetX = 0, int offsetY = 0)
        {
            Width = Math.Max(1, width);  // Ensure at least 1 cell width
            Height = Math.Max(1, height); // Ensure at least 1 cell height
            Offset = (offsetX, offsetY);
        }
        
        /// <summary>
        /// Checks if this collider can move to the specified position on the given map
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool CanMoveTo(int x, int y, IMap map)
        {
            return IsValidPosition(map, x, y);
        }

        /// <summary>
        /// Assigns an owner to this collider
        /// </summary>
        /// <param name="entity"></param>
        public void AssignOwner(Entity entity)
        {
            Owner = entity;
        }

        /// <summary>
        /// Checks if this collider intersects with another collider
        /// </summary>
        /// <param name="other">The other collider to check against</param>
        /// <returns>True if the colliders intersect, false otherwise</returns>
        public bool Intersects(ICollider other)
        {
            if (other is GridCollider gridCollider)
            {
                // Get occupied cells for both colliders
                var thisCells = GetOccupiedCells();
                var otherCells = gridCollider.GetOccupiedCells();
                
                // Check if any cells overlap
                return thisCells.Any(cell => otherCells.Contains(cell));
            }
            
            return false;
        }

        /// <summary>
        /// Checks if this collider is adjacent to another collider
        /// </summary>
        /// <param name="other"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool AdjacentTo(ICollider other, Map map)
        {
            if (other is GridCollider gridOther)
            {
                IEnumerable<Cell> adjacentCells = map.GetAdjacentCells(Position.x, Position.y);

                if (adjacentCells.Any(cell => cell.X == gridOther.Position.x && cell.Y == gridOther.Position.y))
                {
                    return true;
                }
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
            if (other is GridCollider gridCollider)
            {
                // Get occupied cells for both colliders
                var thisCells = GetFutureOccupiedCells(newX, newY);
                var otherCells = gridCollider.GetOccupiedCells();
                
                // Check if any cells overlap
                return thisCells.Any(cell => otherCells.Contains(cell));
            }
            
            return false;
        }
        
        /// <summary>
        /// Checks if this collider would intersect with any collider in the collection if the owner moved to the specified position
        /// </summary>
        /// <param name="colliders">Collection of colliders to check against</param>
        /// <param name="newX">New X position to check</param>
        /// <param name="newY">New Y position to check</param>
        /// <returns>True if the collider would intersect with any collider in the collection, false otherwise</returns>
        public bool WouldIntersectAny(IEnumerable<ICollider> colliders, int newX, int newY)
        {
            return colliders.Any(collider => 
                !collider.Owner.Equals(Owner) && // Skip self
                WouldIntersect(collider, newX, newY));
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
            var cells = new List<(int x, int y)>();
            
            // Calculate the base position with offset
            int baseX = newX + Offset.x;
            int baseY = newY + Offset.y;
            
            // Add all cells covered by the collider
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    cells.Add((baseX + x, baseY + y));
                }
            }
            
            return cells;
        }

        /// <summary>
        /// Checks if the specified position is valid for this collider (within map bounds
        /// and not colliding with obstacles)
        /// </summary>
        /// <param name="map">The map to check against</param>
        /// <param name="x">X position to check</param>
        /// <param name="y">Y position to check</param>
        /// <returns>True if the position is valid, false otherwise</returns>
        public bool IsValidPosition(IMap map, int x, int y)
        {
            // Get all cells that would be occupied at the new position
            var cells = GetFutureOccupiedCells(x, y);
            
            // Check if all cells are within bounds and walkable
            return cells.All(cell => map.IsInBounds(cell.x, cell.y) && map.IsWalkable(cell.x, cell.y));
        }

        /// <summary>
        /// Checks if the specified position is valid for this collider (within map bounds)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsValidPositionFor(Entity entity, IMap map, int x, int y)
        {
            var cells = GetFutureOccupiedCells(x, y);
            
            //If cells are occupied by us, then we can move there
            var validCells = cells.Where(cell => 
                map.IsOccupiedBy(cell.x, cell.y)!.Equals(entity) || 
                map.IsOccupiedBy(cell.x, cell.y) == null);

            var valueTuples = validCells as (int x, int y)[] ?? validCells.ToArray();
            return valueTuples.All(cell => map.IsInBounds(cell.x, cell.y)) && valueTuples.Contains((x,y));
        }

        /// <summary>
        /// Checks if the specified position is valid for this collider (within map bounds)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool CanMoveTo(Vector3 position, IMap map)
        {
            return IsValidPosition(map, (int)position.X, (int)position.Y);
        }
    }
}