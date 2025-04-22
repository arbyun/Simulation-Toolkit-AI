using System;
using SimToolAI.Core.AI;
using SimToolAI.Core.Rendering;
using SimToolAI.Utilities;

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
        public Brain Brain { get; }
        
        /// <summary>
        /// Reference to the scene
        /// </summary>
        protected readonly Scene Scene;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new character with the specified parameters
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="brain">Brain that controls this character</param>
        /// <param name="scene">Scene reference</param>
        public Character(string name, int x, int y, Brain brain, Scene scene) 
            : base(name, x, y, 0) // Awareness is now in the brain
        {
            Brain = brain ?? throw new ArgumentNullException(nameof(brain));
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));
        }
        
        /// <summary>
        /// Creates a new character with the specified parameters and an AI brain
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="scene">Scene reference</param>
        public Character(string name, int x, int y, int awareness, Scene scene) 
            : base(name, x, y, 0) // Awareness is now in the brain
        {
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));
            Brain = new AIBrain(this, awareness, scene);
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
                Scene.RemoveEntity(this);
                return;
            }
            
            // Update the brain
            Brain.Think(deltaTime);
            
            // Process brain decisions
            ProcessBrainDecisions();
        }
        
        /// <summary>
        /// Processes decisions made by the brain
        /// </summary>
        protected virtual void ProcessBrainDecisions()
        {
            // Process movement decision
            Direction? moveDirection = Brain.DecideMovement();
            if (moveDirection.HasValue)
            {
                CommandSystem.MoveEntity(moveDirection.Value, this, Scene.Map);
            }
            
            // Process attack decision
            Entity target = Brain.DecideAttackTarget();
            if (target != null)
            {
                Attack(target);
            }
        }
        
        /// <summary>
        /// Attacks the specified target
        /// </summary>
        /// <param name="target">Target to attack</param>
        protected virtual void Attack(Entity target)
        {
            // If the target is a character, damage it
            if (target is Character character)
            {
                character.TakeDamage(AttackPower);
            }
            // Otherwise, fire a bullet at it
            else
            {
                CommandSystem.FireBullet(this.X, this.Y, this.FacingDirection, Scene, this, 10, AttackPower);
            }
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
            
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Name} took {actualDamage} damage! Health: {previousHealth} -> {Health}");
            Console.ResetColor();
            
            if (!IsAlive)
            {
                Console.SetCursorPosition(0, 1);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Name} has been defeated!");
                Console.ResetColor();
                
                Scene.QueryScene<bool>("SetRenderRequired", true);
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