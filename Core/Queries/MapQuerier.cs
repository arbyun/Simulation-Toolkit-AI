using SimArena.Entities;

namespace SimArena.Core.Queries
{
    /// <summary>
    /// Singleton service for tracking and querying death locations on the map
    /// Provides functionality to track where agents die and analyze death density patterns
    /// </summary>
    public class MapQuerier
    {
        private static MapQuerier? _instance;
        private static readonly object _lock = new object();
        
        private readonly List<DeathLocation> _deathLocations;
        private int _mapWidth;
        private int _mapHeight;
        private bool _isInitialized;

        public static MapQuerier Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new MapQuerier();
                    }
                }
                return _instance;
            }
        }

        public IReadOnlyList<DeathLocation> DeathLocations => _deathLocations.AsReadOnly();
        public bool IsInitialized => _isInitialized;

        private MapQuerier()
        {
            _deathLocations = new List<DeathLocation>();
            _isInitialized = false;
        }

        /// <summary>
        /// Initializes the MapQuerier with map dimensions
        /// Should be called when a new simulation starts
        /// </summary>
        /// <param name="mapWidth">Width of the map</param>
        /// <param name="mapHeight">Height of the map</param>
        public void Initialize(int mapWidth, int mapHeight)
        {
            lock (_lock)
            {
                _mapWidth = mapWidth;
                _mapHeight = mapHeight;
                _deathLocations.Clear();
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Records a death at the specified location
        /// </summary>
        /// <param name="agent">The agent that died</param>
        /// <param name="currentStep">The current simulation step</param>
        public void RecordDeath(Agent agent, int currentStep)
        {
            if (agent == null)
                throw new ArgumentNullException(nameof(agent));

            if (!_isInitialized)
                return; // Silently ignore if not initialized

            lock (_lock)
            {
                var deathLocation = new DeathLocation(agent.X, agent.Y, agent.Name, agent.Team, currentStep);
                _deathLocations.Add(deathLocation);
            }
        }

        /// <summary>
        /// Gets the most densely populated death areas on the map
        /// </summary>
        /// <param name="radius">The radius around each point to consider for density calculation (default: 2)</param>
        /// <param name="topCount">Number of top density areas to return (default: 10)</param>
        /// <returns>List of death density areas ordered by density score (highest first)</returns>
        public List<DeathDensityArea> GetMostDenseDeathAreas(int radius = 2, int topCount = 10)
        {
            if (!_isInitialized)
                return new List<DeathDensityArea>();

            lock (_lock)
            {
                if (_deathLocations.Count == 0)
                    return new List<DeathDensityArea>();

                var densityMap = new Dictionary<(int x, int y), List<DeathLocation>>();

                // Group deaths by exact coordinates first
                var deathsByCoordinate = _deathLocations
                    .GroupBy(d => (d.X, d.Y))
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Calculate density for each coordinate that has deaths
                foreach (var coordinate in deathsByCoordinate.Keys)
                {
                    var deathsInRadius = GetDeathsInRadiusInternal(coordinate.Item1, 
                        coordinate.Item2, radius);
                    if (deathsInRadius.Count > 0)
                    {
                        densityMap[coordinate] = deathsInRadius;
                    }
                }

                // Convert to density areas and calculate scores
                var densityAreas = densityMap.Select(kvp =>
                {
                    var (x, y) = kvp.Key;
                    var deaths = kvp.Value;
                    var deathCount = deaths.Count;
                    
                    // Calculate density score considering both count and area coverage
                    // Higher score for more deaths in smaller area
                    var areaSize = Math.PI * radius * radius;
                    var densityScore = deathCount / areaSize;
                    
                    return new DeathDensityArea(x, y, deathCount, densityScore, deaths);
                }).ToList();

                // Return top density areas sorted by density score
                return densityAreas
                    .OrderByDescending(area => area.DensityScore)
                    .ThenByDescending(area => area.DeathCount)
                    .Take(topCount)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets all deaths within a specified radius of a coordinate
        /// </summary>
        /// <param name="centerX">Center X coordinate</param>
        /// <param name="centerY">Center Y coordinate</param>
        /// <param name="radius">Radius to search within</param>
        /// <returns>List of deaths within the radius</returns>
        public List<DeathLocation> GetDeathsInRadius(int centerX, int centerY, int radius)
        {
            if (!_isInitialized)
                return new List<DeathLocation>();

            lock (_lock)
            {
                return GetDeathsInRadiusInternal(centerX, centerY, radius);
            }
        }

        /// <summary>
        /// Internal method for getting deaths in radius (assumes lock is already held)
        /// </summary>
        private List<DeathLocation> GetDeathsInRadiusInternal(int centerX, int centerY, int radius)
        {
            return _deathLocations
                .Where(death => GetDistance(centerX, centerY, death.X, death.Y) <= radius)
                .ToList();
        }

        /// <summary>
        /// Gets deaths at a specific coordinate
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>List of deaths at the specified coordinate</returns>
        public List<DeathLocation> GetDeathsAtCoordinate(int x, int y)
        {
            if (!_isInitialized)
                return new List<DeathLocation>();

            lock (_lock)
            {
                return _deathLocations
                    .Where(death => death.X == x && death.Y == y)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets death statistics for a specific team
        /// </summary>
        /// <param name="team">Team number</param>
        /// <returns>List of deaths for the specified team</returns>
        public List<DeathLocation> GetDeathsByTeam(int team)
        {
            if (!_isInitialized)
                return new List<DeathLocation>();

            lock (_lock)
            {
                return _deathLocations
                    .Where(death => death.Team == team)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets the total number of deaths recorded
        /// </summary>
        /// <returns>Total death count</returns>
        public int GetTotalDeathCount()
        {
            if (!_isInitialized)
                return 0;

            lock (_lock)
            {
                return _deathLocations.Count;
            }
        }

        /// <summary>
        /// Clears all recorded death data and resets initialization state
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _deathLocations.Clear();
                _isInitialized = false;
            }
        }

        /// <summary>
        /// Calculates Euclidean distance between two points
        /// </summary>
        private static double GetDistance(int x1, int y1, int x2, int y2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Gets a heatmap representation of death density across the entire map, just for fun
        /// </summary>
        /// <param name="cellSize">Size of each cell in a heatmap grid (default: 3)</param>
        /// <returns>2D array representing death density, where higher values indicate more deaths</returns>
        public int[,] GetDeathHeatmap(int cellSize = 3)
        {
            if (!_isInitialized)
                return new int[0, 0];

            lock (_lock)
            {
                var gridWidth = (_mapWidth + cellSize - 1) / cellSize;
                var gridHeight = (_mapHeight + cellSize - 1) / cellSize;
                var heatmap = new int[gridWidth, gridHeight];

                foreach (var death in _deathLocations)
                {
                    var gridX = Math.Min(death.X / cellSize, gridWidth - 1);
                    var gridY = Math.Min(death.Y / cellSize, gridHeight - 1);
                    heatmap[gridX, gridY]++;
                }

                return heatmap;
            }
        }
    }
}