using System.Text;

namespace SimArena.Core.Queries
{
    /// <summary>
    /// Utility class for analyzing map data and providing strategic insights
    /// based on death patterns and density information
    /// </summary>
    /// Useful for our game designer friend, if he wants to see how the agents are performing on a map.
    public static class MapAnalyzer
    {
        /// <summary>
        /// Analyzes death patterns and provides strategic recommendations
        /// </summary>
        /// <param name="mapQuerier">The map querier containing death data</param>
        /// <returns>Strategic analysis report</returns>
        public static string AnalyzeDeathPatterns(MapQuerier mapQuerier)
        {
            if (mapQuerier == null)
                throw new ArgumentNullException(nameof(mapQuerier));

            var report = new StringBuilder();
            var totalDeaths = mapQuerier.GetTotalDeathCount();

            report.AppendLine("=== DEATH PATTERN ANALYSIS ===");
            report.AppendLine($"Total Deaths Recorded: {totalDeaths}");
            report.AppendLine();

            if (totalDeaths == 0)
            {
                report.AppendLine("No deaths recorded yet.");
                return report.ToString();
            }

            // Analyze top death zones
            var topDeathAreas = mapQuerier.GetMostDenseDeathAreas(radius: 3, topCount: 5);
            report.AppendLine("TOP 5 DEATH ZONES:");
            for (int i = 0; i < topDeathAreas.Count; i++)
            {
                var area = topDeathAreas[i];
                report.AppendLine($"{i + 1}. Position ({area.X}, {area.Y}) - {area.DeathCount} deaths (Density: {area.DensityScore:F2})");
            }
            report.AppendLine();

            // Team analysis
            var deathsByTeam = mapQuerier.DeathLocations
                .GroupBy(d => d.Team)
                .ToDictionary(g => g.Key, g => g.ToList());

            report.AppendLine("DEATHS BY TEAM:");
            foreach (var teamData in deathsByTeam.OrderBy(kvp => kvp.Key))
            {
                report.AppendLine($"Team {teamData.Key}: {teamData.Value.Count} deaths");
            }
            report.AppendLine();

            // Strategic recommendations, just for fun, to show how the analysis could be used for certain brain types :)
            report.AppendLine("STRATEGIC RECOMMENDATIONS:");
            if (topDeathAreas.Count > 0)
            {
                var mostDangerous = topDeathAreas[0];
                report.AppendLine($"AVOID: Area around ({mostDangerous.X}, {mostDangerous.Y}) - Highest death density");
                report.AppendLine($"OPPORTUNITY: Same area could be good for ambushes (high traffic)");
                
                if (topDeathAreas.Count > 1)
                {
                    report.AppendLine($"ALTERNATIVE ROUTES: Consider paths avoiding " +
                                      $"top {Math.Min(3, topDeathAreas.Count)} death zones");
                }
            }

            return report.ToString();
        }

        /// <summary>
        /// Gets strategic recommendations for a specific position
        /// </summary>
        /// <param name="mapQuerier">The map querier containing death data</param>
        /// <param name="x">X coordinate to analyze</param>
        /// <param name="y">Y coordinate to analyze</param>
        /// <param name="radius">Radius to check around the position</param>
        /// <returns>Risk assessment and recommendations</returns>
        public static string GetPositionRiskAssessment(MapQuerier mapQuerier, int x, int y, int radius = 3)
        {
            if (mapQuerier == null)
                throw new ArgumentNullException(nameof(mapQuerier));

            var report = new StringBuilder();
            var deathsInArea = mapQuerier.GetDeathsInRadius(x, y, radius);
            var deathsAtExactPosition = mapQuerier.GetDeathsAtCoordinate(x, y);

            report.AppendLine($"=== RISK ASSESSMENT FOR POSITION ({x}, {y}) ===");
            report.AppendLine($"Deaths at exact position: {deathsAtExactPosition.Count}");
            report.AppendLine($"Deaths within {radius} tiles: {deathsInArea.Count}");

            // Risk level calculation
            var riskLevel = CalculateRiskLevel(deathsAtExactPosition.Count, deathsInArea.Count);
            report.AppendLine($"Risk Level: {riskLevel}");
            report.AppendLine();

            // Recommendations based on risk; again, just simulating how a brain could interpret the analysis
            // since I have no time to implement one right now, these are suggestions for future implementations
            report.AppendLine("RECOMMENDATIONS:");
            switch (riskLevel)
            {
                case "VERY HIGH":
                    report.AppendLine("AVOID this position at all costs");
                    report.AppendLine("Consider alternative routes");
                    report.AppendLine("If must pass through, move quickly and be ready for combat");
                    break;
                case "HIGH":
                    report.AppendLine("Exercise extreme caution");
                    report.AppendLine("Good position for experienced players seeking combat");
                    report.AppendLine("Prepare defensive measures before entering");
                    break;
                case "MODERATE":
                    report.AppendLine("Proceed with caution");
                    report.AppendLine("Keep weapons ready");
                    report.AppendLine("Good risk/reward balance for aggressive players");
                    break;
                case "LOW":
                    report.AppendLine("Relatively safe area");
                    report.AppendLine("Good for defensive positioning");
                    report.AppendLine("May be less action but safer for survival");
                    break;
                case "VERY LOW":
                    report.AppendLine("Very safe area");
                    report.AppendLine("Excellent for new players or defensive strategies");
                    report.AppendLine("May lack action/opportunities for kills");
                    break;
            }

            return report.ToString();
        }

        /// <summary>
        /// Generates a simple text-based heatmap visualization
        /// </summary>
        /// <param name="mapQuerier">The map querier containing death data</param>
        /// <param name="cellSize">Size of each cell in the heatmap</param>
        /// <returns>Text representation of the death heatmap</returns>
        public static string GenerateTextHeatmap(MapQuerier mapQuerier, int cellSize = 5)
        {
            if (mapQuerier == null)
                throw new ArgumentNullException(nameof(mapQuerier));

            var heatmap = mapQuerier.GetDeathHeatmap(cellSize);
            var report = new StringBuilder();

            report.AppendLine("=== DEATH HEATMAP ===");
            report.AppendLine("Legend: . = 0 deaths, + = 1-2, * = 3-5, # = 6-10, @ = 11+");
            report.AppendLine();

            for (int y = 0; y < heatmap.GetLength(1); y++)
            {
                for (int x = 0; x < heatmap.GetLength(0); x++)
                {
                    var deaths = heatmap[x, y];
                    char symbol = deaths switch
                    {
                        0 => '.',
                        1 or 2 => '+',
                        3 or 4 or 5 => '*',
                        >= 6 and <= 10 => '#',
                        _ => '@'
                    };
                    report.Append(symbol);
                }
                report.AppendLine();
            }

            return report.ToString();
        }

        /// <summary>
        /// Finds the safest path between two points based on death density
        /// </summary>
        /// <param name="mapQuerier">The map querier containing death data</param>
        /// <param name="startX">Starting X coordinate</param>
        /// <param name="startY">Starting Y coordinate</param>
        /// <param name="endX">Ending X coordinate</param>
        /// <param name="endY">Ending Y coordinate</param>
        /// <returns>List of coordinates representing the safest path</returns>
        public static List<(int x, int y)> FindSafestPath(MapQuerier mapQuerier, int startX, int startY, int endX, int endY)
        {
            // This is a simplified implementation - in a real scenario, you'd want to use A* or similar pathfinding
            // with death density as a cost factor
            var path = new List<(int x, int y)>();
            
            // For now, return a simple direct path with risk assessment
            var dx = Math.Sign(endX - startX);
            var dy = Math.Sign(endY - startY);
            
            int currentX = startX, currentY = startY;
            path.Add((currentX, currentY));
            
            while (currentX != endX || currentY != endY)
            {
                if (currentX != endX) currentX += dx;
                if (currentY != endY) currentY += dy;
                path.Add((currentX, currentY));
            }
            
            return path;
        }

        private static string CalculateRiskLevel(int exactDeaths, int areaDeaths)
        {
            if (exactDeaths >= 5 || areaDeaths >= 15) return "VERY HIGH";
            if (exactDeaths >= 3 || areaDeaths >= 10) return "HIGH";
            if (exactDeaths >= 2 || areaDeaths >= 5) return "MODERATE";
            if (exactDeaths >= 1 || areaDeaths >= 2) return "LOW";
            return "VERY LOW";
        }
    }
}