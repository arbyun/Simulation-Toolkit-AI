using SimArena.Core.Analysis.Data;

namespace SimArena.Core.Analysis
{
    /// <summary>
    /// Analyzes death patterns on the map
    /// Tracks where agents die and provides density analysis
    /// </summary>
    public class DeathAnalysis : BaseMapAnalysis<DeathData>
    {
        public override string AnalysisType => "Death";

        /// <summary>
        /// Get deaths for a specific team
        /// </summary>
        /// <param name="team">Team number</param>
        /// <returns>List of deaths for the specified team</returns>
        public List<DeathData> GetDeathsByTeam(int team)
        {
            if (!IsInitialized)
                return new List<DeathData>();

            return Data
                .Where(death => death.Team == team)
                .ToList();
        }

        /// <summary>
        /// Get the most densely populated death areas on the map
        /// </summary>
        /// <param name="radius">The radius around each point to consider for density calculation</param>
        /// <param name="topCount">Number of top density areas to return</param>
        /// <returns>List of death density areas ordered by density score</returns>
        public List<DensityArea<DeathData>> GetMostDenseAreas(int radius = 2, int topCount = 10)
        {
            if (!IsInitialized)
                return new List<DensityArea<DeathData>>();

            if (Data.Count == 0)
                return new List<DensityArea<DeathData>>();

            var densityMap = new Dictionary<(int x, int y), List<DeathData>>();
            var dataByCoordinate = GetDataByCoordinate();

            // Calculate density for each coordinate that has deaths
            foreach (var coordinate in dataByCoordinate.Keys)
            {
                var dataInRadius = GetDataInRadius(coordinate.x, coordinate.y, radius);
                if (dataInRadius.Count > 0)
                {
                    densityMap[coordinate] = dataInRadius;
                }
            }

            // Convert to density areas and calculate scores
            var densityAreas = densityMap.Select(kvp =>
            {
                var (x, y) = kvp.Key;
                var deaths = kvp.Value;
                var deathCount = deaths.Count;
                
                // Calculate density score considering both count and area coverage
                var areaSize = Math.PI * radius * radius;
                var densityScore = deathCount / areaSize;
                
                return new DensityArea<DeathData>(x, y, deathCount, densityScore, deaths);
            }).ToList();

            // Return top density areas sorted by density score
            return densityAreas
                .OrderByDescending(area => area.DensityScore)
                .ThenByDescending(area => area.DataCount)
                .Take(topCount)
                .ToList();
        }

        /// <summary>
        /// Generate a heatmap representation of death density across the map
        /// </summary>
        /// <param name="cellSize">Size of each cell in the heatmap grid</param>
        /// <returns>2D array representing death density</returns>
        public int[,] GetHeatmap(int cellSize = 3)
        {
            if (!IsInitialized)
                return new int[0, 0];

            var gridWidth = (_mapWidth + cellSize - 1) / cellSize;
            var gridHeight = (_mapHeight + cellSize - 1) / cellSize;
            var heatmap = new int[gridWidth, gridHeight];

            foreach (var death in Data)
            {
                var gridX = Math.Min(death.X / cellSize, gridWidth - 1);
                var gridY = Math.Min(death.Y / cellSize, gridHeight - 1);
                heatmap[gridX, gridY]++;
            }

            return heatmap;
        }
    }
}