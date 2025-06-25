using SimArena.Brains;
using SimArena.Core;
using SimArena.Core.Analysis;
using SimArena.Entities;

namespace SimArena.Examples
{
    /// <summary>
    /// Simple demonstration of MapAnalysis functionality
    /// Run this to see the death tracking and analysis in action
    /// </summary>
    public static class MapAnalysisDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== MapAnalysis Demonstration ===");
            Console.WriteLine("This demo shows how the MapAnalysis system can track i.e." +
                              " deaths and provides tactical analysis.");
            Console.WriteLine();

            // Create a small simulation for demonstration
            var simulation = new Simulation(15, 15);

            Console.WriteLine($"Map size: {simulation.Map.Width}x{simulation.Map.Height}");
            Console.WriteLine($"Map analyzers initialized: {simulation.MapAnalyzers.Count}");
            Console.WriteLine();

            // Add some agents
            AddDemoAgents(simulation);

            // Simulate some deaths to generate data
            SimulateDeaths(simulation);

            // Get the death analysis
            var deathAnalysis = simulation.GetMapAnalyzer<DeathAnalysis>();
            if (deathAnalysis != null)
            {
                // Demonstrate querying capabilities
                DemonstrateQueries(deathAnalysis);

                // Show analysis tools
                DemonstrateAnalysis(deathAnalysis);
            }

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

        private static void DemonstrateQueries(DeathAnalysis deathAnalysis)
        {
            Console.WriteLine("=== QUERYING CAPABILITIES ===");
            Console.WriteLine();

            // Basic statistics
            Console.WriteLine($"Total deaths recorded: {deathAnalysis.GetTotalDataCount()}");
            Console.WriteLine();

            // Show all death locations
            Console.WriteLine("All death locations:");
            foreach (var death in deathAnalysis.Data)
            {
                Console.WriteLine($"  - {death.AgentName} (Team {death.Team}) at ({death.X}, {death.Y}) on step {death.Step}");
            }
            Console.WriteLine();

            // Demonstrate radius queries
            Console.WriteLine("Deaths within radius 2 of position (5, 5):");
            var nearbyDeaths = deathAnalysis.GetDataInRadius(5, 5, 2);
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
                var deathsAtCoord = deathAnalysis.GetDataAtCoordinate(x, y);
                Console.WriteLine($"  - ({x}, {y}): {deathsAtCoord.Count} deaths");
            }
            Console.WriteLine();

            // Demonstrate team queries
            Console.WriteLine("Deaths by team:");
            for (int team = 0; team <= 1; team++)
            {
                var teamDeaths = deathAnalysis.GetDeathsByTeam(team);
                Console.WriteLine($"  - Team {team}: {teamDeaths.Count} deaths");
            }
            Console.WriteLine();
        }

        private static void DemonstrateAnalysis(DeathAnalysis deathAnalysis)
        {
            Console.WriteLine("=== ANALYSIS EXAMPLE ===");
            Console.WriteLine();

            // Show most dangerous areas
            Console.WriteLine("Most dangerous areas (top 3):");
            var dangerousAreas = deathAnalysis.GetMostDenseAreas(radius: 2, topCount: 3);
            for (int i = 0; i < dangerousAreas.Count; i++)
            {
                var area = dangerousAreas[i];
                Console.WriteLine($"  {i + 1}. Position ({area.X}, {area.Y})");
                Console.WriteLine($"     Deaths: {area.DataCount}, Density Score: {area.DensityScore:F3}");
            }
            Console.WriteLine();
        }
    }
}