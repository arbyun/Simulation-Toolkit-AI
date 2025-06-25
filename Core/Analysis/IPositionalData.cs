namespace SimArena.Core.Analysis
{
    /// <summary>
    /// Interface for data that has a position on the map
    /// </summary>
    public interface IPositionalData
    {
        /// <summary>
        /// X coordinate on the map
        /// </summary>
        int X { get; }
        
        /// <summary>
        /// Y coordinate on the map
        /// </summary>
        int Y { get; }
        
        /// <summary>
        /// When this data point was recorded
        /// </summary>
        DateTime Timestamp { get; }
        
        /// <summary>
        /// The simulation step when this occurred
        /// </summary>
        int Step { get; }
    }
}