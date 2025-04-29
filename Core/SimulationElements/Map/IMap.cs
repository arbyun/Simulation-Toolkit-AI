using SimArena.Core.Entities;

namespace SimArena.Core.SimulationElements.Map
{
    public interface IMap
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
        /// Checks who is occupying a specific position on the map
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>The entity that occupies the position, or null if no entity does</returns>
        Entity? IsOccupiedBy(int x, int y);

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
        void SetWalkable(int x, int y, bool isWalkable, Entity entity = null);

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
        void ComputeFov(Character entity, bool lightWalls = true);

        /// <summary>
        /// Toggles field of view computation for an entity
        /// </summary>
        /// <param name="entity">Entity to toggle field of view for</param>
        /// <param name="enabled">Whether field of view should be enabled</param>
        void ToggleFieldOfView(Character entity, bool enabled = true);

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

        /// <summary>
        /// Checks if there is a direct line of sight between two points.
        /// </summary>
        /// <param name="ownerX">X-coordinate of the owner</param>
        /// <param name="ownerY">Y-coordinate of the owner</param>
        /// <param name="argX">X-coordinate of the target point</param>
        /// <param name="argY">Y-coordinate of the target point</param>
        /// <returns></returns>
        bool IsInLineOfSight(int ownerX, int ownerY, int argX, int argY);
    }
    
    public interface IMap<TCell> where TCell : ICell
    {
        TCell this[int x, int y] { get; set; }

        int Width { get; }

        int Height { get; }

        void Initialize(int width, int height);

        bool IsTransparent(int x, int y);

        bool IsWalkable(int x, int y);

        void SetCellProperties(int x, int y, bool isTransparent, bool isWalkable);

        void Clear();

        void Clear(bool isTransparent, bool isWalkable);

        TMap Clone<TMap>() where TMap : IMap<TCell>, new();

        void Copy(IMap<TCell> sourceMap);

        void Copy(IMap<TCell> sourceMap, int left, int top);

        IEnumerable<TCell> GetAllCells();

        IEnumerable<TCell> GetCellsAlongLine(
            int xOrigin,
            int yOrigin,
            int xDestination,
            int yDestination);

        IEnumerable<TCell> GetCellsInCircle(int xCenter, int yCenter, int radius);

        IEnumerable<TCell> GetCellsInDiamond(int xCenter, int yCenter, int distance);

        IEnumerable<TCell> GetCellsInSquare(int xCenter, int yCenter, int distance);

        IEnumerable<TCell> GetCellsInRectangle(int top, int left, int width, int height);

        IEnumerable<TCell> GetBorderCellsInCircle(int xCenter, int yCenter, int radius);

        IEnumerable<TCell> GetBorderCellsInDiamond(int xCenter, int yCenter, int distance);

        IEnumerable<TCell> GetBorderCellsInSquare(int xCenter, int yCenter, int distance);

        IEnumerable<TCell> GetCellsInRows(params int[] rowNumbers);

        IEnumerable<TCell> GetCellsInColumns(params int[] columnNumbers);

        IEnumerable<TCell> GetAdjacentCells(int xCenter, int yCenter);

        IEnumerable<TCell> GetAdjacentCells(int xCenter, int yCenter, bool includeDiagonals);

        TCell GetCell(int x, int y);

        string ToString();

        MapState Save();

        void Restore(MapState state);

        TCell CellFor(int index);

        int IndexFor(int x, int y);

        int IndexFor(TCell cell);
    }
}