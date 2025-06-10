
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RogueSharp;
using RogueSharp.MapCreation;
using SimArena.Brains;
using SimArena.Core;
using SimArena.Core.Objectives;
using SimArena.Core.Objectives.Trackers;
using SimArena.Core.Objectives.Trackers.Interfaces;
using SimArena.Entities;
using SimArena.Serialization.Configuration.Objectives;

namespace SimArena
{
    public static class Program
    {
        /// <summary>
        /// Entry point for the application
        /// </summary>
        public static void Main(string[] args)
        {
            Console.WriteLine("=== SIMULATION TESTS ===\n");
            
            // Test a single simulation with visualization (slow pace)
            RunSingleSimulationWithVisualization();
            
            // Test multiple simulations without visualization (ultra fast)
            RunMultipleSimulationsQuickly(100);
            
            // Test a simulation with different map creation strategy
            TestDifferentMapCreationStrategies();
            
            Console.WriteLine("\nAll simulation tests completed!");
        }
        
        /// <summary>
        /// Runs a single simulation with visualization (slower pace)
        /// </summary>
        private static void RunSingleSimulationWithVisualization()
        {
            Console.WriteLine("\n[TEST 1] Running a single simulation with visualization...");
            
            // Create a simulation with a random map
            var simulation = new Simulation(40, 30);
            
            // Create a deathmatch tracker
            var objective = new DeathmatchObjective(SimulationObjective.TeamDeathmatch, 2, 3);
            var tracker = new DeathmatchTracker(objective);
            simulation.SetObjectiveTracker(tracker);
            
            // Add agents to the simulation
            CreateAgents(simulation, 6, false); // Use normal mode for visualization
            
            // Set up event handlers for visualization
            simulation.Events.OnAgentKilled += (sender, agent) => 
                Console.WriteLine($"Agent {agent.Name} from Team {agent.Team} was killed!");
            
            simulation.Events.OnTeamWon += (sender, teamId) => 
                Console.WriteLine($"Team {teamId} won the match!");
                
            // Set up debug message handler for visualization
            simulation.Events.OnDebugMessage += (sender, message) => 
                Console.WriteLine(message);
            
            // Run the simulation with visualization
            var stopwatch = Stopwatch.StartNew();
            int stepCount = 0;
            
            while (!simulation.IsGameOver)
            {
                simulation.Update(0.5f); // Slower update rate for visualization
                stepCount++;
                
                // if (stepCount % 5 == 0)
                // {
                //     Console.WriteLine($"Step {stepCount} completed. {CountAlivePlayers(simulation)} agents still alive.");
                // }
                
                // small delay for visualization
                // System.Threading.Thread.Sleep(50); 
            }
            
            stopwatch.Stop();
            
            Console.WriteLine($"Simulation completed in {stopwatch.ElapsedMilliseconds}ms after {stepCount} steps");
            Console.WriteLine($"Winning team: {(simulation.WinningTeam >= 0 ? simulation.WinningTeam.ToString() : "Draw")}");
        }
        
        /// <summary>
        /// Runs multiple simulations without visualization 
        /// </summary>
        /// <param name="count">Number of simulations to run</param>
        private static void RunMultipleSimulationsQuickly(int count)
        {
            Console.WriteLine($"\n[TEST 2] Running {count} simulations without visualization...");
            
            var results = new Dictionary<int, int>(); // Team ID -> Win count
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < count; i++)
            {
                // Create a new simulation for each run
                var simulation = new Simulation(40, 30);
                
                // Create a deathmatch tracker
                var objective = new DeathmatchObjective(SimulationObjective.TeamDeathmatch, 2, 3);
                var tracker = new DeathmatchTracker(objective);
                simulation.SetObjectiveTracker(tracker);
                
                // Add agents to the simulation
                CreateAgents(simulation, 6, true); // Set fastMode to true for quick simulations
                
                // Run the simulation as fast as possible
                while (!simulation.IsGameOver)
                {
                    simulation.Update(1.0f); // Fast update rate
                }
                
                // Record the result
                if (simulation.WinningTeam >= 0)
                {
                    if (!results.ContainsKey(simulation.WinningTeam))
                        results[simulation.WinningTeam] = 0;
                    
                    results[simulation.WinningTeam]++;
                }
                
                // Show progress
                if ((i + 1) % 20 == 0 || i == count - 1)
                {
                    Console.WriteLine($"Completed {i + 1}/{count} simulations");
                }
            }
            
            stopwatch.Stop();
            
            // Show results
            Console.WriteLine("\nResults:");
            foreach (var result in results)
            {
                Console.WriteLine($"Team {result.Key}: {result.Value} wins ({result.Value * 100.0 / count:F1}%)");
            }
            
            Console.WriteLine($"\nTotal time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average time per simulation: {stopwatch.ElapsedMilliseconds / (double)count:F2}ms");
        }
        
        /// <summary>
        /// Tests simulations with different map creation strategies
        /// </summary>
        private static void TestDifferentMapCreationStrategies()
        {
            Console.WriteLine("\n[TEST 3] Testing different map creation strategies...");
            
            // 1. Test with random rooms strategy (default)
            Console.WriteLine("\nTesting with RandomRoomsMapCreationStrategy (default):");
            var randomRoomsSimulation = new Simulation(40, 30);
            RunSimulationTest(randomRoomsSimulation);
            
            // 2. Test with cellular automata strategy
            Console.WriteLine("\nTesting with CellularAutomataMapCreationStrategy:");
            var cellularStrategy = new CaveMapCreationStrategy<Map>(40, 30, 45, 2, 4);
            var cellularSimulation = new Simulation(40, 30, cellularStrategy);
            RunSimulationTest(cellularSimulation);
            
            // 3. Test with a pre-created map
            Console.WriteLine("\nTesting with a pre-created map:");
            var mapCreator = new RandomRoomsMapCreationStrategy<Map>(40, 30, 5, 10, 8);
            var preCreatedMap = mapCreator.CreateMap();
            // Modify the map in some way to demonstrate it's pre-created
            preCreatedMap.SetCellProperties(20, 15, true, true);
            var preCreatedMapSimulation = new Simulation(preCreatedMap);
            RunSimulationTest(preCreatedMapSimulation);
        }
        
        /// <summary>
        /// Helper method to run a test simulation with the given simulation object
        /// </summary>
        private static void RunSimulationTest(Simulation simulation)
        {
            // Set up the objective tracker
            var objective = new DeathmatchObjective(SimulationObjective.TeamDeathmatch, 2, 2);
            var tracker = new DeathmatchTracker(objective);
            simulation.SetObjectiveTracker(tracker);
            
            // Add agents
            CreateAgents(simulation, 4, true); // Use fast mode for test simulations
            
            // Run the simulation
            var stopwatch = Stopwatch.StartNew();
            int stepCount = 0;
            
            while (!simulation.IsGameOver && stepCount < 1000) // Prevent infinite loops
            {
                simulation.Update(1.0f);
                stepCount++;
            }
            
            stopwatch.Stop();
            
            Console.WriteLine($"Simulation completed in {stopwatch.ElapsedMilliseconds}ms after {stepCount} steps");
            Console.WriteLine($"Winning team: {(simulation.WinningTeam >= 0 ? simulation.WinningTeam.ToString() : "Draw/Timeout")}");
        }
        
        /// <summary>
        /// Creates agents for the simulation
        /// </summary>
        /// <param name="simulation">The simulation to add agents to</param>
        /// <param name="agentCount">Number of agents to create</param>
        /// <param name="fastMode">Whether to use fast mode for the agents' brains</param>
        private static void CreateAgents(Simulation simulation, int agentCount, bool fastMode = false)
        {
            var random = new Random();
            
            // Create teams (for this example, we'll use 2 teams)
            int teamCount = 2;
            
            for (int i = 0; i < agentCount; i++)
            {
                // Find a random walkable position for the agent
                int x, y;
                do
                {
                    x = random.Next(simulation.Map.Width);
                    y = random.Next(simulation.Map.Height);
                } while (!simulation.Map.IsWalkable(x, y));
                
                // Create an agent with random stats
                var brain = new ChaserBrain(simulation.Map, simulation, i % teamCount);
                
                // Set fast mode if requested
                if (fastMode)
                {
                    brain.FastMode = true;
                }
                
                var agent = new Agent(
                    x: x,
                    y: y,
                    brain: brain,
                    i % teamCount,
                    $"Agent_{i}"
                );
                
                agent.Brain.SetAgent(agent);
                
                // Add to simulation
                simulation.AddAgent(agent);
            }
        }
        
        /// <summary>
        /// Counts the number of alive players in the simulation
        /// </summary>
        private static int CountAlivePlayers(Simulation simulation)
        {
            return simulation.Agents.Count(a => a.IsAlive);
        }
    }
}