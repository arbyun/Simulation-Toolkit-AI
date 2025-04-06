using SimToolAI.Core.Entities;
using SimToolAI.Core.Rendering;

namespace SimToolAI.Core.Map
{
    /// <summary>
    /// Interface for all maps in the simulation
    /// </summary>
    public interface ISimMap
    {
        /// <summary>
        /// Gets the width of the map
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the map
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the renderable for the map
        /// </summary>
        IRenderable Renderable { get; }

        /// <summary>
        /// Initializes the map with a renderable
        /// </summary>
        /// <param name="renderable">Renderable for the map</param>
        void Initialize(IRenderable renderable);

        /// <summary>
        /// Checks if a position is within the map bounds
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>True if the position is within the map bounds, false otherwise</returns>
        bool IsInBounds(int x, int y);

        /// <summary>
        /// Checks if a position is walkable
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>True if the position is walkable, false otherwise</returns>
        bool IsWalkable(int x, int y);

        /// <summary>
        /// Checks if a position is transparent (for line of sight)
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>True if the position is transparent, false otherwise</returns>
        bool IsTransparent(int x, int y);

        /// <summary>
        /// Sets the walkable property of a cell
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="isWalkable">Whether the cell should be walkable</param>
        void SetWalkable(int x, int y, bool isWalkable);

        /// <summary>
        /// Sets the transparent property of a cell
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="isTransparent">Whether the cell should be transparent</param>
        void SetTransparent(int x, int y, bool isTransparent);

        /// <summary>
        /// Computes the field of view from a given entity's position
        /// </summary>
        /// <param name="entity">Entity to compute field of view for</param>
        /// <param name="lightWalls">Whether walls should be visible at the edge of the field of view</param>
        void ComputeFov(Entity entity, bool lightWalls = true);

        /// <summary>
        /// Toggles field of view computation for an entity
        /// </summary>
        /// <param name="entity">Entity to toggle field of view for</param>
        /// <param name="enabled">Whether field of view should be enabled</param>
        void ToggleFieldOfView(Entity entity, bool enabled = true);

        /// <summary>
        /// Checks if a position is in the current field of view
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>True if the position is in the field of view, false otherwise</returns>
        bool IsInFov(int x, int y);

        /// <summary>
        /// Attempts to set an entity's position on the map
        /// </summary>
        /// <param name="entity">Entity to move</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>True if the entity was moved, false otherwise</returns>
        bool SetEntityPosition(Entity entity, int x, int y);

        /// <summary>
        /// Gets a random walkable location in a specified area
        /// </summary>
        /// <param name="minX">Minimum X-coordinate</param>
        /// <param name="maxX">Maximum X-coordinate</param>
        /// <param name="minY">Minimum Y-coordinate</param>
        /// <param name="maxY">Maximum Y-coordinate</param>
        /// <returns>A random walkable location, or null if none was found</returns>
        (int x, int y)? GetRandomWalkableLocation(int minX, int maxX, int minY, int maxY);

        /// <summary>
        /// Gets a random walkable location anywhere on the map
        /// </summary>
        /// <returns>A random walkable location, or null if none was found</returns>
        (int x, int y)? GetRandomWalkableLocation();

        /// <summary>
        /// Gets the distance between two positions
        /// </summary>
        /// <param name="x1">X-coordinate of the first position</param>
        /// <param name="y1">Y-coordinate of the first position</param>
        /// <param name="x2">X-coordinate of the second position</param>
        /// <param name="y2">Y-coordinate of the second position</param>
        /// <returns>The distance between the two positions</returns>
        float GetDistance(int x1, int y1, int x2, int y2);
    }
}