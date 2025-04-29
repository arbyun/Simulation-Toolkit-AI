using SimArena.Core.Serialization.Results;

namespace SimArena.Core.Configuration
{
    public class StepsSimulationResult : ISimulationResult
    {
        public int Steps { get; set; }
        public int MaxSteps { get; set; }

        public StepsSimulationResult(int steps, int maxSteps)
        {
            Steps = steps;
            MaxSteps = maxSteps;
        }

        public string Read() => $"Steps Taken: {Steps}/{MaxSteps}";

        public object ToSerializable() => new { Steps, MaxSteps };
    }
}