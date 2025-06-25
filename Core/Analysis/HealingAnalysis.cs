using SimArena.Core.Analysis.Data;

namespace SimArena.Core.Analysis
{
    /// <summary>
    /// Analyzes healing patterns on the map
    /// Tracks where healing occurs and provides density analysis
    /// </summary>
    public class HealingAnalysis : BaseMapAnalysis<HealingData>
    {
        public override string AnalysisType => "Healing";

        /// <summary>
        /// Get healing events for a specific team (as healer)
        /// </summary>
        /// <param name="team">Team number</param>
        /// <returns>List of healing events where the specified team provided healing</returns>
        public List<HealingData> GetHealingByHealerTeam(int team)
        {
            if (!IsInitialized)
                return new List<HealingData>();

            return Data
                .Where(healing => healing.HealerTeam == team)
                .ToList();
        }

        /// <summary>
        /// Get healing events for a specific team (as patient)
        /// </summary>
        /// <param name="team">Team number</param>
        /// <returns>List of healing events where the specified team received healing</returns>
        public List<HealingData> GetHealingByPatientTeam(int team)
        {
            if (!IsInitialized)
                return new List<HealingData>();

            return Data
                .Where(healing => healing.PatientTeam == team)
                .ToList();
        }

        /// <summary>
        /// Get total healing done in a radius
        /// </summary>
        /// <param name="centerX">Center X coordinate</param>
        /// <param name="centerY">Center Y coordinate</param>
        /// <param name="radius">Radius to search within</param>
        /// <returns>Total healing amount in the area</returns>
        public int GetTotalHealingInRadius(int centerX, int centerY, int radius)
        {
            var healingEvents = GetDataInRadius(centerX, centerY, radius);
            return healingEvents.Sum(h => h.HealingAmount);
        }

        /// <summary>
        /// Get the most intense healing areas on the map
        /// </summary>
        /// <param name="radius">The radius around each point to consider</param>
        /// <param name="topCount">Number of top areas to return</param>
        /// <returns>List of healing density areas ordered by total healing</returns>
        public List<DensityArea<HealingData>> GetMostIntenseHealingAreas(int radius = 2, int topCount = 10)
        {
            if (!IsInitialized)
                return new List<DensityArea<HealingData>>();

            if (Data.Count == 0)
                return new List<DensityArea<HealingData>>();

            var densityMap = new Dictionary<(int x, int y), List<HealingData>>();
            var dataByCoordinate = GetDataByCoordinate();

            // Calculate healing intensity for each coordinate
            foreach (var coordinate in dataByCoordinate.Keys)
            {
                var healingInRadius = GetDataInRadius(coordinate.x, coordinate.y, radius);
                if (healingInRadius.Count > 0)
                {
                    densityMap[coordinate] = healingInRadius;
                }
            }

            // Convert to density areas and calculate scores based on total healing
            var densityAreas = densityMap.Select(kvp =>
            {
                var (x, y) = kvp.Key;
                var healingEvents = kvp.Value;
                var totalHealing = healingEvents.Sum(h => h.HealingAmount);
                var eventCount = healingEvents.Count;
                
                // Use total healing as the density score
                return new DensityArea<HealingData>(x, y, eventCount, totalHealing, healingEvents);
            }).ToList();

            // Return top areas sorted by total healing (density score)
            return densityAreas
                .OrderByDescending(area => area.DensityScore)
                .ThenByDescending(area => area.DataCount)
                .Take(topCount)
                .ToList();
        }
    }
}