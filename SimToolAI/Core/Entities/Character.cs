using System;
using System.Numerics;
using External.SimToolAI.SimToolAI.Core.AI;
using SimToolAI.Core.AI;

namespace SimToolAI.Core.Entities
{
    /// <summary>
    /// Represents a character entity with health, combat abilities, and a brain
    /// </summary>
    public class Character : Entity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the character's health
        /// </summary>
        public int Health { get; set; } = 100;

        /// <summary>
        /// Gets or sets the character's maximum health
        /// </summary>
        public int MaxHealth { get; set; } = 100;

        /// <summary>
        /// Gets or sets the character's attack power
        /// </summary>
        public int AttackPower { get; set; } = 10;

        /// <summary>
        /// Gets or sets the character's defense
        /// </summary>
        public int Defense { get; set; } = 5;

        /// <summary>
        /// Gets whether the character is alive
        /// </summary>
        public bool IsAlive => Health > 0;
        
        /// <summary>
        /// Gets the brain that controls this character
        /// </summary>
        public Brain Brain { get; protected set; }
        
        /// <summary>
        /// Owned weapons
        /// </summary>
        public Weapon[] Weapons { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new character with the specified parameters
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="brain">Brain that controls this character</param>
        /// <param name="simulation">The simulation instance</param>
        public Character(string name, int x, int y, Brain brain, Simulation simulation) 
            : base(name, x, y, simulation) // Awareness is now in the brain
        {
            Brain = brain ?? throw new ArgumentNullException(nameof(brain));
        }
        
        public Character(string name, int x, int y, Simulation simulation) 
            : base(name, x, y, simulation)
        {
            Brain = new AIBrain(this, 1, simulation);
        }

        /// <summary>
        /// Creates a new character with the specified parameters
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="brain">Brain that controls this character</param>
        /// <param name="simulation">The simulation instance</param>
        /// <param name="weapons">The weapons owned by the character</param>
        public Character(string name, int x, int y, Brain brain, Simulation simulation, Weapon[] weapons) 
            : base(name, x, y, simulation) // Awareness is now in the brain
        {
            Brain = brain ?? throw new ArgumentNullException(nameof(brain));
            Weapons = weapons;

            foreach (var w in Weapons)
            {
                w.Owned = true;
            }
        }

        /// <summary>
        /// Creates a new character with the specified parameters and an AI brain
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="simulation"></param>
        public Character(string name, int x, int y, int awareness, Simulation simulation) 
            : base(name, x, y, simulation)
        {
            Brain = new AIBrain(this, awareness, simulation);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the character state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public override void Update(float deltaTime)
        {
            // If the character is dead, remove it from the scene
            if (!IsAlive)
            {
                Simulation.Scene.RemoveEntity(this);
                return;
            }
            
            // Update the brain
            Brain.Think(deltaTime);
        }
        
        /// <summary>
        /// Applies damage to the character
        /// </summary>
        /// <param name="amount">Amount of damage to apply</param>
        /// <returns>True if the character was damaged, false if the character died</returns>
        public virtual bool TakeDamage(int amount)
        {
            if (!IsAlive)
                return false;

            // Apply defense reduction
            int actualDamage = Math.Max(1, amount - Defense);
            int previousHealth = Health;

            Health = Math.Max(0, Health - actualDamage);
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Name} took {actualDamage} damage! Health: {previousHealth} -> {Health}");
            Console.ResetColor();
            
            if (!IsAlive)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Name} has been defeated!");
                Console.ResetColor();
                
                Simulation.Scene.QueryScene<bool>("SetRenderRequired", true);
            }

            return IsAlive;
        }

        /// <summary>
        /// Heals the character
        /// </summary>
        /// <param name="amount">Amount of health to restore</param>
        public void Heal(int amount)
        {
            if (!IsAlive)
                return;

            Health = Math.Min(MaxHealth, Health + amount);
        }
        
        /// <summary>
        /// Attacks another entity
        /// </summary>
        /// <param name="target">Direction towards which to attack</param>
        /// <returns>If the attack was successful</returns>
        public bool Attack(Vector3 target)
        {
            if (Weapons.Length == 0)
            {
             // Choose random weapon for now
             Weapons[new Random().Next(0, Weapons.Length)].Attack(target);
             return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a string representation of this character
        /// </summary>
        /// <returns>A string representation of this character</returns>
        public override string ToString()
        {
            return $"{Name} (HP: {Health}/{MaxHealth})";
        }

        #endregion

        
    }
}