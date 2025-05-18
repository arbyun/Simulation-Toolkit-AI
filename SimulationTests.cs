using System;
using System.Diagnostics;
using RogueSharp;
using RogueSharp.MapCreation;
using SimArena.Core;
using SimArena.Core.Configuration;
using SimArena.Core.Serialization.Configuration;
using SimArena.Core.SimulationElements.Map;

namespace SimArena
{
    public static class SimulationTests
    {
        /// <summary>
        /// Tests the Simulation class with a StepsObjective
        /// </summary>
        public static void TestSimulationWithStepsObjective()
        {
            Console.WriteLine("Starting Steps Objective Simulation Test...");
            
            // Create a steps objective
            StepsObjective objective = new StepsObjective(SimulationObjective.Steps, 10);
            
            // Create a game configuration
            GameConfig config = CreateGameConfig(objective);
            
            // Create and run the simulation
            RunSimulation(config);
            
            Console.WriteLine("Steps Objective Simulation Test Completed!");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Tests the Simulation class with a DeathmatchObjective
        /// </summary>
        public static void TestSimulationWithDeathmatchObjective()
        {
            Console.WriteLine("Starting Deathmatch Objective Simulation Test...");
            
            // Create a deathmatch objective
            DeathmatchObjective objective = new DeathmatchObjective(SimulationObjective.TeamDeathmatch, 
                2, 1);
            
            // Create a game configuration
            GameConfig config = CreateGameConfig(objective);
            
            // Create and run the simulation
            RunSimulation(config);
            
            Console.WriteLine("Deathmatch Objective Simulation Test Completed!");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Tests the Simulation class with a CapturePointObjective
        /// </summary>
        public static void TestSimulationWithCapturePointObjective()
        {
            Console.WriteLine("Starting Capture Point Objective Simulation Test...");
            
            // Create a capture point objective
            CapturePointObjective objective = new CapturePointObjective(SimulationObjective.CapturePoint, 2, 
                1, 5f, 10f);
            
            // Create a game configuration
            GameConfig config = CreateGameConfig(objective);
            
            // Create and run the simulation
            RunSimulation(config);
            
            Console.WriteLine("Capture Point Objective Simulation Test Completed!");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Tests the Simulation class with a DefendObjective
        /// </summary>
        public static void TestSimulationWithDefendObjective()
        {
            Console.WriteLine("Starting Defend Objective Simulation Test...");
            
            // Create a defend objective
            DefendObjective objective = new DefendObjective(SimulationObjective.DefendObjective, 2, 
                1, 10f);
            
            // Create a game configuration
            GameConfig config = CreateGameConfig(objective);
            
            // Create and run the simulation
            RunSimulation(config);
            
            Console.WriteLine("Defend Objective Simulation Test Completed!");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Tests that the Simulation class throws an exception when trying to create
        /// a simulation with human agents in offline mode
        /// </summary>
        public static void TestSimulationWithHumanAgentsInOfflineMode()
        {
            Console.WriteLine("Testing Simulation with Human Agents in Offline Mode...");
            
            // Create a steps objective
            StepsObjective objective = new StepsObjective(SimulationObjective.Steps, 10);
            
            // Create a game configuration with human agents
            GameConfig config = CreateGameConfig(objective, true);
            
            try
            {
                // Create a simulation in offline mode
                Simulation simulation = new Simulation(config);
                
                Console.WriteLine("ERROR: Expected an InvalidOperationException but none was thrown!");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Successfully caught expected exception: {ex.Message}");
            }
            
            Console.WriteLine("Human Agents in Offline Mode Test Completed!");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Creates a game configuration with the specified objective
        /// </summary>
        /// <param name="objective">The objective to use</param>
        /// <param name="useHumanAgents">Whether to use human agents</param>
        /// <returns>A game configuration</returns>
        private static GameConfig CreateGameConfig(ObjectiveConfig objective, bool useHumanAgents = false)
        {
            // Create a game configuration
            GameConfig config = new GameConfig
            {
                Name = $"{objective.TypeEnum} Test",
                MapPath = "test_map.json", // placeholder, we'll create a map in memory
                Objective = objective
            };
            
            // Add some agents
            config.Agents.Add(new AgentConfig
            {
                Name = "Agent 1",
                BrainType = useHumanAgents ? BrainType.Human : BrainType.AI,
                RandomStart = true,
                Awareness = 10,
                MaxHealth = 100,
                AttackPower = 10,
                Defense = 5,
                Speed = 1.0f
            });
            
            config.Agents.Add(new AgentConfig
            {
                Name = "Agent 2",
                BrainType = BrainType.AI,
                RandomStart = true,
                Awareness = 10,
                MaxHealth = 100,
                AttackPower = 10,
                Defense = 5,
                Speed = 1.0f
            });
            
            return config;
        }
        
        /// <summary>
        /// Creates and runs a simulation with the specified configuration
        /// </summary>
        /// <param name="config">The game configuration to use</param>
        private static void RunSimulation(GameConfig config)
        {
            // Create a simulation in offline mode
            Simulation simulation = new Simulation(config);
            
            // Create a map
            var strat = new RandomRoomsMapCreationStrategy<Map>(40,60,6,10, 5);
            GridMapBridge bridge = new GridMapBridge(simulation);
            bridge.GenerateMap(strat.CreateMap());
            
            // Set up event handlers
            simulation.Events.Started += (_, _) => Console.WriteLine("Simulation started");
            simulation.Events.Stopped += (_, result) => Console.WriteLine($"Simulation stopped with result: {result.Read()}");
            simulation.Events.StepCompleted += (_, step) => Console.WriteLine($"Step {step} completed");
            simulation.Events.OnMove += (_, move) => Console.WriteLine($"{move.Name} moved to ({move.X}, {move.Y})");
            simulation.Events.OnDamage += (_, damage) => Console.WriteLine($"{damage.Item1.Name} dealt damage to {damage.Item2.Name}");
            
            // Initialize the simulation
            simulation.Initialize(bridge);
            
            // Start the simulation
            simulation.Start();
            
            double deltaTime = 0;
            double secondframe = 0;
            double counter = 0;
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            while (simulation.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                double firstFrame = ts.TotalMilliseconds;
            
                deltaTime = firstFrame - secondframe;
                
                simulation.Update(Convert.ToSingle(deltaTime));
            }
        }
    }
}