using System.Numerics;
using SimArena.Core.Configuration;
using SimArena.Core.Entities.Components;
using SimArena.Core.Entities.Components.Collision;

namespace SimArena.Core.Entities
{
    /// <summary>
    /// Represents a character in the simulation, which can be controlled by either AI or a human player.
    /// Characters have health, attack capabilities, and can use weapons.
    /// </summary>
    public class Character: Entity
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
        
        /// <summary>
        /// The main weapon currently equipped by the character.
        /// This defaults to the first weapon in the array if no weapon is equipped.
        /// </summary>
        public Weapon MainWeapon => Weapons.FirstOrDefault(w => w.IsEquipped) ?? Weapons.First();

        /// <summary>
        /// Kills, deaths, assists
        /// </summary>
        public DeathmatchSimulationResult.KDA Kda { get; internal set; } = new(0,0,0);

        /// <summary>
        /// Last entity that dealt damage to this character
        /// </summary>
        private Entity _lastDamager;
        
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
        /// <param name="collider">The collider used by the character</param>
        public Character(string name, int x, int y, Brain brain, Simulation simulation, ICollider? collider) 
            : base(name, x, y, simulation, collider)
        {
            Weapons = [];
            Brain = brain ?? throw new ArgumentNullException(nameof(brain));
        }
        
        /// <summary>
        /// Creates a new character with the specified parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="brain"></param>
        /// <param name="simulation"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Character(string name, int x, int y, Brain brain, Simulation simulation, int width, int height) 
            : base(name, x, y, simulation, width, height)
        {
            Weapons = [];
            Brain = brain ?? throw new ArgumentNullException(nameof(brain));
        }
        
        /// <summary>
        /// Creates a new character with the specified parameters and an AI brain
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="simulation"></param>
        /// <param name="collider"></param>
        public Character(string name, int x, int y, Simulation simulation, ICollider? collider) 
            : base(name, x, y, simulation, collider)
        {
            Weapons = [];
            Brain = BrainFactory.CreateBrain(this, 1, simulation, false);
        }

        /// <summary>
        /// Creates a new character with the specified parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="simulation"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Character(string name, int x, int y, Simulation simulation, int width, int height) 
            : base(name, x, y, simulation, width, height)
        {
            Weapons = [];
            Brain = BrainFactory.CreateBrain(this, 1, simulation, false);
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
        /// <param name="collider">Collider used by the character</param>
        public Character(string name, int x, int y, Brain brain, Simulation simulation, Weapon[] weapons, ICollider? collider) 
            : base(name, x, y, simulation, collider) 
        {
            Brain = brain ?? throw new ArgumentNullException(nameof(brain));
            Weapons = weapons;
        }
        
        /// <summary>
        /// Creates a new character with the specified parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="brain"></param>
        /// <param name="simulation"></param>
        /// <param name="weapons"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Character(string name, int x, int y, Brain brain, Simulation simulation, Weapon[] weapons, int width, int height) 
            : base(name, x, y, simulation, width, height) 
        {
            Brain = brain ?? throw new ArgumentNullException(nameof(brain));
            Weapons = weapons;
        }

        /// <summary>
        /// Creates a new character with the specified parameters and an AI brain
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="simulation">The simulation instance</param>
        /// <param name="weapons">Weapons owned by the character</param>
        /// <param name="collider">The collider used by the character</param>
        public Character(string name, int x, int y, int awareness, Simulation simulation, Weapon[] weapons, ICollider? collider) 
            : base(name, x, y, simulation, collider)
        {
            Weapons = weapons;
            Brain = BrainFactory.CreateBrain(this, awareness, simulation, false);
        }

        /// <summary>
        /// Creates a new character with the specified parameters and an AI brain
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="awareness"></param>
        /// <param name="simulation"></param>
        /// <param name="weapons"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Character(string name, int x, int y, int awareness, Simulation simulation, 
            Weapon[] weapons, int width, int height) : base(name, x, y, simulation, width, height)
        {
            Weapons = weapons;
            Brain = BrainFactory.CreateBrain(this, awareness, simulation, false);
        }
        
        /// <summary>
        /// Creates a new character with the specified parameters and an AI brain
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="simulation">The simulation instance</param>
        /// <param name="weapons">Weapons owned by the character</param>
        /// <param name="collider">The collider used by the character</param>
        /// <param name="humanControlled">If the character should be controlled by a human player</param>
        public Character(string name, int x, int y, int awareness, Simulation simulation, 
            Weapon[] weapons, ICollider? collider, bool humanControlled) : base(name, x, y, simulation, collider)
        {
            Weapons = weapons;
            Brain = BrainFactory.CreateBrain(this, awareness, simulation, humanControlled);
        }
        
        /// <summary>
        /// Creates a new character with the specified parameters and an AI brain
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="awareness"></param>
        /// <param name="simulation"></param>
        /// <param name="weapons"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="humanControlled"></param>
        public Character(string name, int x, int y, int awareness, Simulation simulation, 
            Weapon[] weapons, int width, int height, bool humanControlled) : base(name, x, y, simulation, width, height)
        {
            Weapons = weapons;
            Brain = BrainFactory.CreateBrain(this, awareness, simulation, humanControlled);
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
                Kda = Kda with { Deaths = Kda.Deaths + 1 };
                Simulation.ProcessDeath(_lastDamager, this);
                Simulation.Destroy(this);
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
        public virtual bool TakeDamage(int amount, Entity? attacker = null, bool log = false)
        {
            if (!IsAlive)
                return false;

            // Apply defense reduction
            int actualDamage = Math.Max(1, amount - Defense);
            int previousHealth = Health;

            Health = Math.Max(0, Health - actualDamage);

            if (attacker != null)
            {
                _lastDamager = attacker;
            }
            
            Simulation.ProcessDamage(attacker, this);

            if (log)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Name} took {actualDamage} damage! " +
                                  $"Health: {previousHealth} -> {Health}");
                Console.ResetColor();
            
                if (!IsAlive)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{Name} has been defeated!");
                    Console.ResetColor();
                }
            }

            return IsAlive;
        }

        /// <summary>
        /// Heals the character
        /// </summary>
        /// <param name="amount">Amount of health to restore</param>
        public void Heal(int amount, Entity? healer = null)
        {
            if (!IsAlive)
                return;

            Health = Math.Min(MaxHealth, Health + amount);
            Simulation.ProcessHeal(healer, this);
        }
        
        /// <summary>
        /// Attacks in the specified direction using one of the character's weapons
        /// </summary>
        /// <param name="direction">Direction towards which to attack</param>
        /// <returns>True if the attack was successful, false if no weapons are available</returns>
        public bool Attack(Vector3 direction)
        {
            if (Weapons.Length == 0)
            {
                return false;
            }

            // Find the first equipped weapon, or use any weapon if none are equipped
            Weapon? weaponToUse = Weapons.FirstOrDefault(w => w.IsEquipped) ?? Weapons.FirstOrDefault();
            
            if (weaponToUse == null)
            {
                return false;
            }
            
            return weaponToUse.Attack(direction);
        }
        
        /// <summary>
        /// Attacks using a specific weapon by index
        /// </summary>
        /// <param name="weaponIndex">Index of the weapon to use</param>
        /// <param name="direction">Direction towards which to attack</param>
        /// <returns>True if the attack was successful, false if the weapon index is invalid</returns>
        public bool AttackWithWeapon(int weaponIndex, Vector3 direction)
        {
            if (weaponIndex < 0 || weaponIndex >= Weapons.Length)
            {
                return false;
            }
            
            return Weapons[weaponIndex].Attack(direction);
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

        /// <summary>
        /// Respawns the character at the given position
        /// </summary>
        /// <param name="position">The position where the character will respawn</param>
        public void Respawn(Vector3 position)
        {
            Health = MaxHealth;
            bool result = Simulation.Map.SetEntityPosition(this, (int)position.X, (int)position.Y);

            if (result)
            {
                Console.WriteLine($"{Name} respawned at ({position.X}, {position.Y}).");
            }
            else
            {
                Console.WriteLine($"Failed to respawn {Name} at ({position.X}, {position.Y}).");
                var randomPos = Simulation.Map.GetRandomWalkableLocation();
                    Simulation.Map.SetEntityPosition(this, randomPos.Value.x, randomPos.Value.y);
            }
        }
    }
}