using System;
using SimArena.Core.Entities;

namespace SimArena.Core.SimulationElements.Map
{
    public class GridMap: Map, IMap
    {
        #region Properties

        /// <summary>
        /// Gets the width of the map
        /// </summary>
        int IMap.Width => base.Width;

        /// <summary>
        /// Gets the height of the map
        /// </summary>
        int IMap.Height => base.Height;

        /// <summary>
        /// Field of view calculator
        /// </summary>
        private FieldOfView? _fieldOfView;

        /// <summary>
        /// Entity currently used for field of view calculations
        /// </summary>
        private Entity _currentFovEntity;

        /// <summary>
        /// Random number generator
        /// </summary>
        private readonly Random _random = new Random();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new grid map with the specified dimensions and renderable
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        public GridMap(int width, int height) : base(width, height)
        {
            _fieldOfView = new FieldOfView(this);
        }

        /// <summary>
        /// Creates a new grid map with default settings
        /// </summary>
        public GridMap()
        {
            _fieldOfView = new FieldOfView(this);
        }

        #endregion

        #region IMap Implementation

        /// <summary>
        /// Checks if a position is within the map bounds
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>True if the position is within the map bounds, false otherwise</returns>
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// Toggles field of view computation for an entity
        /// </summary>
        /// <param name="entity">Entity to toggle field of view for</param>
        /// <param name="enabled">Whether field of view should be enabled</param>
        public void ToggleFieldOfView(Character entity, bool enabled = true)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_fieldOfView == null)
            {
                _fieldOfView = new FieldOfView(this);
            }

            _currentFovEntity = enabled ? entity : null;

            if (enabled)
            {
                ComputeFov(entity);
            }
        }

        /// <summary>
        /// Sets the walkable property of a cell
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="isWalkable">Whether the cell should be walkable</param>
        public void SetWalkable(int x, int y, bool isWalkable, Entity entity = null)
        {
            if (!IsInBounds(x, y))
                return;

            GetCell(x, y).IsWalkable = isWalkable;
            GetCell(x,y).Entity = entity;
        }

        /// <summary>
        /// Sets the transparent property of a cell
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="isTransparent">Whether the cell should be transparent</param>
        public void SetTransparent(int x, int y, bool isTransparent)
        {
            if (!IsInBounds(x, y))
                return;

            GetCell(x, y).IsTransparent = isTransparent;
        }

        /// <summary>
        /// Computes the field of view from a given entity's position
        /// </summary>
        /// <param name="entity">Entity to compute field of view for</param>
        /// <param name="lightWalls">Whether walls should be visible at the edge of the field of view</param>
        public void ComputeFov(Character entity, bool lightWalls = true)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_fieldOfView == null)
            {
                _fieldOfView = new FieldOfView(this);
            }

            _fieldOfView.ComputeFov(entity.X, entity.Y, entity.Brain.Awareness, lightWalls);
        }

        /// <summary>
        /// Checks if a position is in the current field of view
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>True if the position is in the field of view, false otherwise</returns>
        public bool IsInFov(int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            return _fieldOfView != null && _fieldOfView.IsInFov(x, y);
        }

        /// <summary>
        /// Gets a random walkable location in a specified area
        /// </summary>
        /// <param name="minX">Minimum X-coordinate</param>
        /// <param name="maxX">Maximum X-coordinate</param>
        /// <param name="minY">Minimum Y-coordinate</param>
        /// <param name="maxY">Maximum Y-coordinate</param>
        /// <returns>A random walkable location, or null if none was found</returns>
        public (int x, int y)? GetRandomWalkableLocation(int minX, int maxX, int minY, int maxY)
        {
            // Ensure bounds are within map limits
            minX = Math.Clamp(minX, 0, Width - 1);
            maxX = Math.Clamp(maxX, 0, Width - 1);
            minY = Math.Clamp(minY, 0, Height - 1);
            maxY = Math.Clamp(maxY, 0, Height - 1);

            // Check if there's any walkable space in the area
            bool hasWalkableSpace = false;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (IsWalkable(x, y))
                    {
                        hasWalkableSpace = true;
                        break;
                    }
                }
                if (hasWalkableSpace) break;
            }

            if (!hasWalkableSpace)
                return null;

            // Try to find a random walkable location
            for (int i = 0; i < 100; i++)
            {
                int x = _random.Next(minX, maxX + 1);
                int y = _random.Next(minY, maxY + 1);

                if (IsWalkable(x, y))
                    return (x, y);
            }

            return null;
        }

        /// <summary>
        /// Gets a random walkable location anywhere on the map
        /// </summary>
        /// <returns>A random walkable location, or null if none was found</returns>
        public (int x, int y)? GetRandomWalkableLocation()
        {
            return GetRandomWalkableLocation(0, Width - 1, 0, Height - 1);
        }

        /// <summary>
        /// Attempts to set an entity's position on the map
        /// </summary>
        /// <param name="entity">Entity to move</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>True if the entity was moved, false otherwise</returns>
        public bool SetEntityPosition(Entity entity, int x, int y)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!IsInBounds(x, y) || !IsWalkable(x, y))
                return false;

            // Make the entity's current position walkable again
            if (IsInBounds(entity.X, entity.Y))
            {
                SetWalkable(entity.X, entity.Y, true);
            }

            // Update entity position
            entity.X = x;
            entity.Y = y;

            // Make the entity's new position not walkable if the entity blocks movement
            if (entity.BlocksMovement)
            {
                SetWalkable(x, y, false, entity);
            }

            // Compute FOV with the entity as the center if it's the current FOV entity
            if (_currentFovEntity != null && _currentFovEntity.Equals(entity) && entity is Character character)
            {
                ComputeFov(character);
            }

            return true;
        }
        
        /// <summary>
        /// Gets the entity that occupies a specific position on the map
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Entity? IsOccupiedBy(int x, int y)
        {
            if (IsWalkable(x, y))
            {
                return null;
            }

            return GetCell(x, y).Entity;
        }

        /// <summary>
        /// Gets the distance between two positions
        /// </summary>
        /// <param name="x1">X-coordinate of the first position</param>
        /// <param name="y1">Y-coordinate of the first position</param>
        /// <param name="x2">X-coordinate of the second position</param>
        /// <param name="y2">Y-coordinate of the second position</param>
        /// <returns>The distance between the two positions</returns>
        public float GetDistance(int x1, int y1, int x2, int y2)
        {
            int dx = x1 - x2;
            int dy = y1 - y2;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Checks if there is a clear line of sight between two positions
        /// </summary>
        /// <param name="ownerX">X-coordinate of the starting position</param>
        /// <param name="ownerY">Y-coordinate of the starting position</param>
        /// <param name="argX">X-coordinate of the target position</param>
        /// <param name="argY">Y-coordinate of the target position</param>
        /// <returns>True if there is a clear line of sight, false otherwise</returns>
        public bool IsInLineOfSight(int ownerX, int ownerY, int argX, int argY)
        {
            // Check if both positions are in bounds
            if (!IsInBounds(ownerX, ownerY) || !IsInBounds(argX, argY))
                return false;
                
            // Use Bresenham's line algorithm to check for obstacles
            int dx = Math.Abs(argX - ownerX);
            int dy = Math.Abs(argY - ownerY);
            int sx = ownerX < argX ? 1 : -1;
            int sy = ownerY < argY ? 1 : -1;
            int err = dx - dy;
            
            int x = ownerX;
            int y = ownerY;
            
            while (x != argX || y != argY)
            {
                // Skip the starting position
                if (x != ownerX || y != ownerY)
                {
                    // If we hit a non-transparent cell, there's no line of sight
                    if (!GetCell(x, y).IsTransparent)
                        return false;
                }
                
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }
            
            // If we reached the target without hitting obstacles, there's a clear line of sight
            return true;
        }

        #endregion
    }
}