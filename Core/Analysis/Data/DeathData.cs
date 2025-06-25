namespace SimArena.Core.Analysis.Data
{
    /// <summary>
    /// Represents a death event on the map
    /// </summary>
    public struct DeathData : IPositionalData
    {
        public int X { get; }
        public int Y { get; }
        public DateTime Timestamp { get; }
        public int Step { get; }
        public string AgentName { get; }
        public int Team { get; }

        public DeathData(int x, int y, string agentName, int team, int step)
        {
            X = x;
            Y = y;
            AgentName = agentName ?? throw new ArgumentNullException(nameof(agentName));
            Team = team;
            Step = step;
            Timestamp = DateTime.Now;
        }
    }
}