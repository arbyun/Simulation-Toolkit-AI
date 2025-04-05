using SimToolAI.Core.Entities;
using SimToolAI.Core.Rendering;

namespace SimToolAI.Core.Map
{
    /// <summary>
    /// Interface for all maps in the game.
    /// </summary>
    public interface ISimMap
    {
        public int Width { get; }
        public int Height { get; }
        public IRenderable Renderable { get; }
    
        /// <summary>
        /// Checks if a position is walkable
        /// </summary>
        public bool IsWalkable(int x, int y);
    
        /// <summary>
        /// Checks if a position is transparent (for line of sight)
        /// </summary>
        public bool IsTransparent(int x, int y);
    
        /// <summary>
        /// Sets the walkable property of a cell
        /// </summary>
        public void SetWalkable(int x, int y, bool isWalkable);
    
        /// <summary>
        /// Sets the transparent property of a cell
        /// </summary>
        public void SetTransparent(int x, int y, bool isTransparent);

        /// <summary>
        /// Computes the field of view from a given position
        /// </summary>
        public void ComputeFov(Entity entity, bool lightWalls = true);
    
        /// <summary>
        /// Checks if a position is in the current field of view
        /// </summary>
        public bool IsInFov(int x, int y);
    
        /// <summary>
        /// Attempts to set an entity's position on the map
        /// </summary>
        public bool SetEntityPosition(Entity entity, int x, int y);
    
        /// <summary>
        /// Gets a random walkable location in a specified area
        /// </summary>
        public (int x, int y)? GetRandomWalkableLocation(int minX, int maxX, int minY, int maxY);
    }
}