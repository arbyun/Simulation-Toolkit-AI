namespace SimArena.Core.Analysis
{
    /// <summary>
    /// Base implementation for map analysis systems
    /// Provides common functionality for tracking positional data
    /// </summary>
    /// <typeparam name="TData">The type of data being tracked</typeparam>
    public abstract class BaseMapAnalysis<TData> : IMapAnalysis<TData> where TData : IPositionalData
    {
        private readonly List<TData> _data;
        
        protected int _mapWidth;
        protected int _mapHeight;
        private bool _isInitialized;

        public IReadOnlyList<TData> Data => _data.AsReadOnly();
        public bool IsInitialized => _isInitialized;
        public abstract string AnalysisType { get; }

        protected BaseMapAnalysis()
        {
            _data = new List<TData>();
            _isInitialized = false;
        }

        public virtual void Initialize(int mapWidth, int mapHeight)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _data.Clear();
            _isInitialized = true;
        }

        public virtual void Reset()
        {
            _data.Clear();
            _isInitialized = false;
        }

        public virtual void RecordData(TData dataPoint)
        {
            if (dataPoint == null)
                throw new ArgumentNullException(nameof(dataPoint));

            if (!_isInitialized)
                return; 

            _data.Add(dataPoint);
        }

        public virtual List<TData> GetDataInRadius(int centerX, int centerY, int radius)
        {
            if (!_isInitialized)
                return new List<TData>();

            return _data
                .Where(data => GetDistance(centerX, centerY, data.X, data.Y) <= radius)
                .ToList();
        }

        public virtual List<TData> GetDataAtCoordinate(int x, int y)
        {
            if (!_isInitialized)
                return new List<TData>();

            return _data
                .Where(data => data.X == x && data.Y == y)
                .ToList();
        }

        public virtual int GetTotalDataCount()
        {
            if (!_isInitialized)
                return 0;

            return _data.Count;
        }

        /// <summary>
        /// Calculate Euclidean distance between two points
        /// </summary>
        protected static double GetDistance(int x1, int y1, int x2, int y2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Get data points grouped by coordinate for density analysis
        /// </summary>
        protected Dictionary<(int x, int y), List<TData>> GetDataByCoordinate()
        {
            return _data
                .GroupBy(d => (d.X, d.Y))
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}