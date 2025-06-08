namespace SimArena.Entities
{
    public record Kda(int Kills, int Deaths, int Assists)
    {
        public int Kills { get; set; } = Kills;
        public int Deaths { get; set; } = Deaths;
        public int Assists { get; set; } = Assists;

        public void Deconstruct(out int kills, out int deaths, out int assists)
        {
            kills = Kills;
            deaths = Deaths;
            assists = Assists;
        }
    }
}