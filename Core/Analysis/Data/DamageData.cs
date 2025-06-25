namespace SimArena.Core.Analysis.Data
{
    /// <summary>
    /// Represents a damage event on the map
    /// </summary>
    public struct DamageData : IPositionalData
    {
        public int X { get; }
        public int Y { get; }
        public DateTime Timestamp { get; }
        public int Step { get; }
        public int DamageAmount { get; }
        public int DealerTeam { get; }
        public int ReceiverTeam { get; }
        public string DealerName { get; }
        public string ReceiverName { get; }

        public DamageData(int x, int y, int damageAmount, int dealerTeam, int receiverTeam, 
            string dealerName, string receiverName, int step)
        {
            X = x;
            Y = y;
            DamageAmount = damageAmount;
            DealerTeam = dealerTeam;
            ReceiverTeam = receiverTeam;
            DealerName = dealerName ?? throw new ArgumentNullException(nameof(dealerName));
            ReceiverName = receiverName ?? throw new ArgumentNullException(nameof(receiverName));
            Step = step;
            Timestamp = DateTime.Now;
        }
    }
}