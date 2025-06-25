using SimArena.Core.Analysis.Data;

namespace SimArena.Core.Analysis
{
    /// <summary>
    /// Analyzes damage patterns on the map
    /// Tracks where damage is dealt and provides density analysis
    /// </summary>
    public class DamageAnalysis : BaseMapAnalysis<DamageData>
    {
        public override string AnalysisType => "Damage";

        /// <summary>
        /// Get damage events for a specific team (as dealer)
        /// </summary>
        /// <param name="team">Team number</param>
        /// <returns>List of damage events where the specified team dealt damage</returns>
        public List<DamageData> GetDamageByDealerTeam(int team)
        {
            if (!IsInitialized)
                return new List<DamageData>();

            return Data
                .Where(damage => damage.DealerTeam == team)
                .ToList();
        }

        /// <summary>
        /// Get damage events for a specific team (as receiver)
        /// </summary>
        /// <param name="team">Team number</param>
        /// <returns>List of damage events where the specified team received damage</returns>
        public List<DamageData> GetDamageByReceiverTeam(int team)
        {
            if (!IsInitialized)
                return new List<DamageData>();

            return Data
                .Where(damage => damage.ReceiverTeam == team)
                .ToList();
        }

        /// <summary>
        /// Get total damage dealt in a radius
        /// </summary>
        /// <param name="centerX">Center X coordinate</param>
        /// <param name="centerY">Center Y coordinate</param>
        /// <param name="radius">Radius to search within</param>
        /// <returns>Total damage amount in the area</returns>
        public int GetTotalDamageInRadius(int centerX, int centerY, int radius)
        {
            var damageEvents = GetDataInRadius(centerX, centerY, radius);
            return damageEvents.Sum(d => d.DamageAmount);
        }

        /// <summary>
        /// Get the most intense damage areas on the map
        /// </summary>
        /// <param name="radius">The radius around each point to consider</param>
        /// <param name="topCount">Number of top areas to return</param>
        /// <returns>List of damage density areas ordered by total damage</returns>
        public List<DensityArea<DamageData>> GetMostIntenseDamageAreas(int radius = 2, int topCount = 10)
        {
            if (!IsInitialized)
                return new List<DensityArea<DamageData>>();

            if (Data.Count == 0)
                return new List<DensityArea<DamageData>>();

            var densityMap = new Dictionary<(int x, int y), List<DamageData>>();
            var dataByCoordinate = GetDataByCoordinate();

            // Calculate damage intensity for each coordinate
            foreach (var coordinate in dataByCoordinate.Keys)
            {
                var damageInRadius = GetDataInRadius(coordinate.x, coordinate.y, radius);
                if (damageInRadius.Count > 0)
                {
                    densityMap[coordinate] = damageInRadius;
                }
            }

            // Convert to density areas and calculate scores based on total damage
            var densityAreas = densityMap.Select(kvp =>
            {
                var (x, y) = kvp.Key;
                var damageEvents = kvp.Value;
                var totalDamage = damageEvents.Sum(d => d.DamageAmount);
                var eventCount = damageEvents.Count;
                
                // Use total damage as the density score
                return new DensityArea<DamageData>(x, y, eventCount, totalDamage, damageEvents);
            }).ToList();

            // Return top areas sorted by total damage (density score)
            return densityAreas
                .OrderByDescending(area => area.DensityScore)
                .ThenByDescending(area => area.DataCount)
                .Take(topCount)
                .ToList();
        }
    }
}