using System.Collections.Generic;
using SimToolAI.Core.Entities;

namespace SimToolAI.Core
{
    /// <summary>
    /// Simulation result
    /// </summary>
    public class SimulationResult
    {
        /// <summary>
        /// Number of steps simulated
        /// </summary>
        public int Steps { get; set; }
        
        /// <summary>
        /// Elapsed time in seconds
        /// </summary>
        public float ElapsedTime { get; set; }
        
        /// <summary>
        /// List of surviving agents
        /// </summary>
        public List<Character> SurvivingAgents { get; set; } = new List<Character>();
        
        /// <summary>
        /// List of defeated agents
        /// </summary>
        public List<Character> DefeatedAgents { get; set; } = new List<Character>();
    }
}