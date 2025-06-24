namespace SimArena.Core.Queries
{
    /// <summary>
    /// Represents a death density area with coordinates and death count
    /// </summary>
    public class DeathDensityArea
    {
        public int X { get; }
        public int Y { get; }
        public int DeathCount { get; }
        public double DensityScore { get; }
        public List<DeathLocation> Deaths { get; }

        public DeathDensityArea(int x, int y, int deathCount, double densityScore, List<DeathLocation> deaths)
        {
            X = x;
            Y = y;
            DeathCount = deathCount;
            DensityScore = densityScore;
            Deaths = deaths ?? new List<DeathLocation>();
        }
    }
}