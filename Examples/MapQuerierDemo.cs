using SimArena.Brains;
using SimArena.Core;
using SimArena.Core.Queries;
using SimArena.Entities;

namespace SimArena.Examples
{
    /// <summary>
    /// Simple demonstration of MapQuerier functionality
    /// Run this to see the death tracking and analysis in action
    /// </summary>
    public static class MapQuerierDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== MapQuerier Demonstration ===");
            Console.WriteLine("This demo shows how the MapQuerier tracks deaths and provides tactical analysis.");
            Console.WriteLine();

            // Create a small simulation for demonstration
            var simulation = new Simulation(15, 15);
            var mapQuerier = MapQuerier.Instance;

            Console.WriteLine($"Map size: {simulation.Map.Width}x{simulation.Map.Height}");
            Console.WriteLine($"MapQuerier initialized: {mapQuerier.IsInitialized}");
            Console.WriteLine();

            // Add some agents
            AddDemoAgents(simulation);

            // Simulate some deaths to generate data
            SimulateDeaths(simulation);

            // Demonstrate querying capabilities
            DemonstrateQueries(mapQuerier);

            // Show analysis tools
            DemonstrateAnalysis(mapQuerier);

            Console.WriteLine("=== Demo Complete ===");
        }

        private static void AddDemoAgents(Simulation simulation)
        {
            Console.WriteLine("Adding demo agents...");

            // Add some random brain agents
            for (int i = 0; i < 4; i++)
            {
                var (x, y) = Brain.GetRandomWalkableLocation(simulation.Map);
                var brain = new RandomBrain(simulation.Map, 0);
                var agent = new Agent(x, y, brain, $"Random_{i}");
                brain.SetAgent(agent);
                simulation.AddAgent(agent);
            }

            // Add some tactical brain agents
            for (int i = 0; i < 3; i++)
            {
                var (x, y) = Brain.GetRandomWalkableLocation(simulation.Map);
                var brain = new TacticalBrain(simulation.Map, 1);
                var agent = new Agent(x, y, brain, $"Tactical_{i}");
                brain.SetAgent(agent);
                simulation.AddAgent(agent);
            }

            Console.WriteLine($"Added {simulation.Agents.Count} agents to the simulation.");
            Console.WriteLine();
        }

        private static void SimulateDeaths(Simulation simulation)
        {
            Console.WriteLine("Simulating deaths to generate data...");
            var random = new Random();

            // Simulate some deaths at various locations
            var deathPositions = new[]
            {
                (5, 5), (5, 6), (6, 5),    // Cluster 1 - high density area
                (10, 10), (10, 11),       // Cluster 2 - moderate density
                (2, 2),                   // Isolated death
                (12, 3), (13, 3),         // Cluster 3 - edge area
                (7, 8), (8, 7), (7, 7)    // Cluster 4 - central area
            };

            for (int i = 0; i < deathPositions.Length && simulation.Agents.Count > 0; i++)
            {
                var (x, y) = deathPositions[i];
                
                // Find an agent to "kill" at this position
                var agent = simulation.Agents[random.Next(simulation.Agents.Count)];
                if (agent.IsAlive)
                {
                    // Move agent to death position for demonstration
                    agent.X = x;
                    agent.Y = y;
                    
                    Console.WriteLine($"Step {i + 1}: {agent.Name} (Team {agent.Team}) died at ({x}, {y})");
                    simulation.KillAgent(agent);
                }
            }

            Console.WriteLine($"Simulated {deathPositions.Length} deaths.");
            Console.WriteLine();
        }

        private static void DemonstrateQueries(MapQuerier mapQuerier)
        {
            Console.WriteLine("=== QUERYING ===");
            Console.WriteLine();

            // Basic statistics
            Console.WriteLine($"Total deaths recorded: {mapQuerier.GetTotalDeathCount()}");
            Console.WriteLine();

            // Show all death locations
            Console.WriteLine("All death locations:");
            foreach (var death in mapQuerier.DeathLocations)
            {
                Console.WriteLine($"  - {death.AgentName} (Team {death.Team}) at ({death.X}, {death.Y}) on step {death.Step}");
            }
            Console.WriteLine();

            // Demonstrate radius queries
            Console.WriteLine("Deaths within radius 2 of position (5, 5):");
            var nearbyDeaths = mapQuerier.GetDeathsInRadius(5, 5, 2);
            foreach (var death in nearbyDeaths)
            {
                var distance = Math.Sqrt(Math.Pow(death.X - 5, 2) + Math.Pow(death.Y - 5, 2));
                Console.WriteLine($"  - {death.AgentName} at ({death.X}, {death.Y}) - Distance: {distance:F1}");
            }
            Console.WriteLine();

            // Demonstrate coordinate-specific queries
            Console.WriteLine("Deaths at specific coordinates:");
            var testCoords = new[] { (5, 5), (10, 10), (1, 1) };
            foreach (var (x, y) in testCoords)
            {
                var deathsAtCoord = mapQuerier.GetDeathsAtCoordinate(x, y);
                Console.WriteLine($"  - ({x}, {y}): {deathsAtCoord.Count} deaths");
            }
            Console.WriteLine();

            // Demonstrate team queries
            Console.WriteLine("Deaths by team:");
            for (int team = 0; team <= 1; team++)
            {
                var teamDeaths = mapQuerier.GetDeathsByTeam(team);
                Console.WriteLine($"  - Team {team}: {teamDeaths.Count} deaths");
            }
            Console.WriteLine();
        }

        private static void DemonstrateAnalysis(MapQuerier mapQuerier)
        {
            Console.WriteLine("=== ANALYSIS TOOLS ===");
            Console.WriteLine();

            // Show most dangerous areas
            Console.WriteLine("Most dangerous areas (top 3):");
            var dangerousAreas = mapQuerier.GetMostDenseDeathAreas(radius: 2, topCount: 3);
            for (int i = 0; i < dangerousAreas.Count; i++)
            {
                var area = dangerousAreas[i];
                Console.WriteLine($"  {i + 1}. Position ({area.X}, {area.Y})");
                Console.WriteLine($"     Deaths: {area.DeathCount}, Density Score: {area.DensityScore:F3}");
            }
            Console.WriteLine();

            // Demonstrate position risk assessment
            if (dangerousAreas.Count > 0)
            {
                var testPos = dangerousAreas[0];
                Console.WriteLine($"Risk assessment for position ({testPos.X}, {testPos.Y}):");
                var riskAssessment = MapAnalyzer.GetPositionRiskAssessment(mapQuerier, testPos.X, testPos.Y, 2);
                Console.WriteLine(riskAssessment);
            }

            // Show text heatmap
            Console.WriteLine("Death heatmap visualization:");
            var heatmapText = MapAnalyzer.GenerateTextHeatmap(mapQuerier, cellSize: 3);
            Console.WriteLine(heatmapText);

            // Show comprehensive analysis
            Console.WriteLine("Comprehensive death pattern analysis:");
            var fullAnalysis = MapAnalyzer.AnalyzeDeathPatterns(mapQuerier);
            Console.WriteLine(fullAnalysis);
        }

        /// <summary>
        /// Demonstrates how a brain would use MapQuerier in practice
        /// </summary>
        public static void DemonstrateTacticalUsage()
        {
            Console.WriteLine("=== TACTICAL USAGE EXAMPLE ===");
            
            var mapQuerier = MapQuerier.Instance;
            if (!mapQuerier.IsInitialized)
            {
                Console.WriteLine("MapQuerier not initialized.");
                return;
            }

            // Simulate a brain making tactical decisions
            var currentX = 6;
            var currentY = 6;

            Console.WriteLine($"Agent at position ({currentX}, {currentY})");
            Console.WriteLine();

            // Check immediate danger
            var deathsAtPosition = mapQuerier.GetDeathsAtCoordinate(currentX, currentY);
            if (deathsAtPosition.Count > 0)
            {
                Console.WriteLine($"WARNING: {deathsAtPosition.Count} agents have died at this exact position!");
            }

            // Check nearby danger
            var nearbyDeaths = mapQuerier.GetDeathsInRadius(currentX, currentY, 2);
            Console.WriteLine($"Deaths within 2 tiles: {nearbyDeaths.Count}");

            // Calculate risk level
            var riskLevel = Math.Min(1.0, nearbyDeaths.Count / 5.0);
            var riskDescription = riskLevel switch
            {
                > 0.8 => "VERY HIGH",
                > 0.6 => "HIGH", 
                > 0.4 => "MODERATE",
                > 0.2 => "LOW",
                _ => "VERY LOW"
            };

            Console.WriteLine($"Risk Level: {riskDescription} ({riskLevel:P0})");
            Console.WriteLine();

            // Tactical recommendations
            Console.WriteLine("Tactical Recommendations:");
            if (riskLevel > 0.8)
            {
                Console.WriteLine("RETREAT: Move to a safer area immediately!");
                
                // Find safer nearby positions
                Console.WriteLine("Safer nearby positions:");
                for (int dx = -2; dx <= 2; dx++)
                {
                    for (int dy = -2; dy <= 2; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        
                        var checkX = currentX + dx;
                        var checkY = currentY + dy;
                        var deathsAtCheck = mapQuerier.GetDeathsInRadius(checkX, checkY, 1);
                        
                        if (deathsAtCheck.Count < nearbyDeaths.Count)
                        {
                            Console.WriteLine($"  - ({checkX}, {checkY}): {deathsAtCheck.Count} nearby deaths");
                        }
                    }
                }
            }
            else if (riskLevel < 0.3)
            {
                Console.WriteLine("OPPORTUNITY: Consider moving toward action areas for potential kills");
                
                var opportunityAreas = mapQuerier.GetMostDenseDeathAreas(radius: 3, topCount: 3);
                Console.WriteLine("Opportunity areas (moderate risk/reward):");
                foreach (var area in opportunityAreas)
                {
                    if (area.DeathCount >= 2 && area.DeathCount <= 6)
                    {
                        var distance = Math.Sqrt(Math.Pow(area.X - currentX, 2) + Math.Pow(area.Y - currentY, 2));
                        Console.WriteLine($"  - ({area.X}, {area.Y}): {area.DeathCount} deaths, Distance: {distance:F1}");
                    }
                }
            }
            else
            {
                Console.WriteLine("BALANCED: Current position has moderate risk. Proceed with caution.");
            }

            Console.WriteLine();
        }
    }
}