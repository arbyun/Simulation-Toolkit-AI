namespace SimArena.Core.Analysis
{
    /// <summary>
    /// Represents an area with calculated data density
    /// Generic version that works with any type of positional data
    /// </summary>
    /// <typeparam name="TData">The type of data in this density area</typeparam>
    public class DensityArea<TData> where TData : IPositionalData
    {
        /// <summary>
        /// Center X coordinate of the density area
        /// </summary>
        public int X { get; }
        
        /// <summary>
        /// Center Y coordinate of the density area
        /// </summary>
        public int Y { get; }
        
        /// <summary>
        /// Number of data points in this area
        /// </summary>
        public int DataCount { get; }
        
        /// <summary>
        /// Calculated density score for this area
        /// The meaning depends on the analysis type (e.g., deaths per area, total damage, etc.)
        /// </summary>
        public double DensityScore { get; }
        
        /// <summary>
        /// Individual data points that make up this density area
        /// </summary>
        public IReadOnlyList<TData> DataPoints { get; }

        public DensityArea(int x, int y, int dataCount, double densityScore, List<TData> dataPoints)
        {
            X = x;
            Y = y;
            DataCount = dataCount;
            DensityScore = densityScore;
            DataPoints = dataPoints?.AsReadOnly() ?? throw new ArgumentNullException(nameof(dataPoints));
        }
    }
}