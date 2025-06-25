namespace SimArena.Core.Analysis.Data
{
    /// <summary>
    /// Represents a healing event on the map
    /// </summary>
    public struct HealingData : IPositionalData
    {
        public int X { get; }
        public int Y { get; }
        public DateTime Timestamp { get; }
        public int Step { get; }
        public int HealingAmount { get; }
        public int HealerTeam { get; }
        public int PatientTeam { get; }
        public string HealerName { get; }
        public string PatientName { get; }

        public HealingData(int x, int y, int healingAmount, int healerTeam, int patientTeam,
            string healerName, string patientName, int step)
        {
            X = x;
            Y = y;
            HealingAmount = healingAmount;
            HealerTeam = healerTeam;
            PatientTeam = patientTeam;
            HealerName = healerName ?? throw new ArgumentNullException(nameof(healerName));
            PatientName = patientName ?? throw new ArgumentNullException(nameof(patientName));
            Step = step;
            Timestamp = DateTime.Now;
        }
    }
}