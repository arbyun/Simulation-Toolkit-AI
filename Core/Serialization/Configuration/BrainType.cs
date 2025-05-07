namespace SimArena.Core.Configuration
{
    /// <summary>
    /// Type of brain for an agent
    /// </summary>
    public enum BrainType
    {
        /// <summary>
        /// Human-controlled brain
        /// </summary>
        Human,
        
        /// <summary>
        /// AI-controlled brain
        /// </summary>
        AI
    }

    /// <summary>
    /// Type of weapon for an agent
    /// </summary>
    public enum WeaponType
    {
        Ranged,
        Melee
    }
}