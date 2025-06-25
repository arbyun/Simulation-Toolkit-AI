namespace SimArena.Core.Analysis
{
    /// <summary>
    /// Base interface for all map analysis systems
    /// Each analyzer tracks and analyzes specific types of events on the map
    /// </summary>
    public interface IMapAnalysis
    {
        /// <summary>
        /// Whether this analyzer has been initialized with map dimensions
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// Initialize the analyzer with map dimensions
        /// </summary>
        /// <param name="mapWidth">Width of the map</param>
        /// <param name="mapHeight">Height of the map</param>
        void Initialize(int mapWidth, int mapHeight);
        
        /// <summary>
        /// Reset all tracked data
        /// </summary>
        void Reset();
        
        /// <summary>
        /// Get the type of analysis this analyzer provides
        /// </summary>
        string AnalysisType { get; }
    }
    
    /// <summary>
    /// Generic interface for map analysis systems that track specific data types
    /// </summary>
    /// <typeparam name="TData">The type of data being tracked</typeparam>
    public interface IMapAnalysis<TData> : IMapAnalysis
    {
        /// <summary>
        /// All tracked data points
        /// </summary>
        IReadOnlyList<TData> Data { get; }
        
        /// <summary>
        /// Record a new data point
        /// </summary>
        /// <param name="dataPoint">The data to record</param>
        void RecordData(TData dataPoint);
        
        /// <summary>
        /// Get data points within a radius of a coordinate
        /// </summary>
        /// <param name="centerX">Center X coordinate</param>
        /// <param name="centerY">Center Y coordinate</param>
        /// <param name="radius">Radius to search within</param>
        /// <returns>List of data points within the radius</returns>
        List<TData> GetDataInRadius(int centerX, int centerY, int radius);
        
        /// <summary>
        /// Get data points at a specific coordinate
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>List of data points at the coordinate</returns>
        List<TData> GetDataAtCoordinate(int x, int y);
        
        /// <summary>
        /// Get the total count of recorded data points
        /// </summary>
        /// <returns>Total data count</returns>
        int GetTotalDataCount();
    }
}