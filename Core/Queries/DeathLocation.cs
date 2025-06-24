namespace SimArena.Core.Queries
{
    /// <summary>
    /// Represents a death location with its coordinates and metadata
    /// </summary>
    public class DeathLocation
    {
        public int X { get; }
        public int Y { get; }
        public DateTime Timestamp { get; }
        public string AgentName { get; }
        public int Team { get; }
        public int Step { get; }

        public DeathLocation(int x, int y, string agentName, int team, int step)
        {
            X = x;
            Y = y;
            AgentName = agentName;
            Team = team;
            Step = step;
            Timestamp = DateTime.UtcNow;
        }
    }
}