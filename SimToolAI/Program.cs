using System;
using System.IO;
using SimToolAI.Core;
using SimToolAI.Core.Configuration;

namespace SimToolAI
{
    /// <summary>
    /// Main program class for the SimToolAI demo
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point for the application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        private static void Main(string[] args)
        {
            try
            {
                // Parse command line arguments
                if (args.Length < 1)
                {
                    Console.WriteLine("Usage: dotnet run <config_file>");
                    return;
                }
                
                string configPath = args[0];
                
                // Check if the configuration file exists
                if (!File.Exists(configPath))
                {
                    Console.WriteLine($"Configuration file not found: {configPath}");
                    return;
                }
                
                // Load the configuration
                MatchConfig config = MatchConfig.LoadFromFile(configPath);
                
                // Validate the configuration for console mode
                if (!config.Validate(true, out string errorMessage))
                {
                    Console.WriteLine($"Invalid configuration: {errorMessage}");
                    return;
                }
                
                // Create and run the simulation
                RunSimulation(config);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Runs a simulation with the specified configuration
        /// </summary>
        /// <param name="config">Match configuration</param>
        private static void RunSimulation(MatchConfig config)
        {
            Console.WriteLine($"Running simulation: {config.Name}");
            Console.WriteLine($"Map: {config.MapPath}");
            Console.WriteLine($"Agents: {config.Agents.Count}");
            Console.WriteLine($"Max steps: {config.MaxSteps}");
            Console.WriteLine();
            
            // Create the simulation in offline mode, since we're in console mode
            var simulation = new Simulation(config, SimulationMode.Offline);
            
            // Subscribe to simulation events
            simulation.Initialized += (sender, e) => Console.WriteLine("Simulation initialized");
            simulation.Started += (sender, e) => Console.WriteLine("Simulation started");
            
            simulation.StepCompleted += (sender, e) =>
            {
                if (e == config.MaxSteps)
                {
                    Console.WriteLine($"Step {e}/{config.MaxSteps} completed");
                }
            };
            
            simulation.Stopped += OnSimulationStopped;
            
            // Initialize and start the simulation
            simulation.Initialize();
            simulation.Start();
            
            Console.WriteLine("Simulation completed");
        }
        
        /// <summary>
        /// Called when the simulation is stopped
        /// </summary>
        private static void OnSimulationStopped(object sender, SimulationResult e)
        {
            Console.WriteLine();
            Console.WriteLine($"Simulation completed after {e.Steps} steps and {e.ElapsedTime:F2} seconds");
            Console.WriteLine();
            
            Console.WriteLine("Surviving agents:");
            foreach (var agent in e.SurvivingAgents)
            {
                Console.WriteLine($"- {agent.Name}: HP {agent.Health}/{agent.MaxHealth}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Defeated agents:");
            foreach (var agent in e.DefeatedAgents)
            {
                Console.WriteLine($"- {agent.Name}");
            }
        }
    }
}
