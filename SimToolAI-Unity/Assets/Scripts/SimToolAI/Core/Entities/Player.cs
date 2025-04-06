using System;

namespace SimToolAI.Core.Entities
{
    /// <summary>
    /// Represents a player-controlled entity in the simulation
    /// </summary>
    public class Player : Entity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the player's health
        /// </summary>
        public int Health { get; set; } = 100;

        /// <summary>
        /// Gets or sets the player's maximum health
        /// </summary>
        public int MaxHealth { get; set; } = 100;

        /// <summary>
        /// Gets or sets the player's attack power
        /// </summary>
        public int AttackPower { get; set; } = 10;

        /// <summary>
        /// Gets or sets the player's defense
        /// </summary>
        public int Defense { get; set; } = 5;

        /// <summary>
        /// Gets whether the player is alive
        /// </summary>
        public bool IsAlive => Health > 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new player with the specified parameters
        /// </summary>
        /// <param name="name">Name of the player</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="awareness">Awareness radius</param>
        public Player(string name, int x, int y, int awareness) : base(name, x, y, awareness)
        {
        }

        /// <summary>
        /// Creates a new player with the specified parameters and default awareness
        /// </summary>
        /// <param name="name">Name of the player</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        public Player(string name, int x, int y) : base(name, x, y, 10)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies damage to the player
        /// </summary>
        /// <param name="amount">Amount of damage to apply</param>
        /// <returns>True if the player was damaged, false if the player died</returns>
        public bool TakeDamage(int amount)
        {
            if (!IsAlive)
                return false;

            // Apply defense reduction
            int actualDamage = Math.Max(1, amount - Defense);

            Health = Math.Max(0, Health - actualDamage);

            return IsAlive;
        }

        /// <summary>
        /// Heals the player
        /// </summary>
        /// <param name="amount">Amount of health to restore</param>
        public void Heal(int amount)
        {
            if (!IsAlive)
                return;

            Health = Math.Min(MaxHealth, Health + amount);
        }

        /// <summary>
        /// Returns a string representation of this player
        /// </summary>
        /// <returns>A string representation of this player</returns>
        public override string ToString()
        {
            return $"{Name} (HP: {Health}/{MaxHealth})";
        }

        #endregion
    }
}