using SimArena.Core.Configuration;

namespace SimArena.Core.Entities.Components
{
    /// <summary>
    /// Factory for creating brain instances based on the simulation objective
    /// </summary>
    public static class BrainFactory
    {
        /// <summary>
        /// Creates a brain based on the simulation objective
        /// </summary>
        /// <param name="owner">The character that owns the brain</param>
        /// <param name="awareness">The awareness radius</param>
        /// <param name="simulation">The simulation instance</param>
        /// <param name="humanControlled">Whether the brain should be human-controlled</param>
        /// <returns>A brain instance</returns>
        public static Brain CreateBrain(Character owner, int awareness, Simulation simulation, bool humanControlled)
        {
            if (humanControlled)
            {
                return new HumanBrain(owner, awareness, simulation);
            }

            // Create a brain based on the objective type
            var objectiveType = simulation.Config.Objective.TypeEnum;

            return objectiveType switch
            {
                SimulationObjective.TeamDeathmatch => new SampleDeathmatchAIBrain(owner, awareness, simulation),
                SimulationObjective.CapturePoint => new SampleCapturePointAIBrain(owner, awareness, simulation),
                _ => new SampleAiBrain(owner, awareness, simulation)
            };
        }
    }
}