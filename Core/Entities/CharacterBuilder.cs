using System;
using SimArena.Core.Entities.Components;
using SimArena.Core.Entities.Components.Collision;

namespace SimArena.Core.Entities
{
    /// <summary>
    /// Builder class for creating Character instances with various configurations.
    /// Implements the Builder design pattern to simplify character creation.
    /// </summary>
    public class CharacterBuilder
    {
        private string _name;
        private int _x;
        private int _y;
        private Simulation _simulation;
        private Brain? _brain;
        private int _awareness = 1;
        private bool _isHumanControlled;
        private Weapon[] _weapons = Array.Empty<Weapon>();
        private ICollider? _collider;
        private int _width = 1;
        private int _height = 1;
        private int _maxHealth = 100;
        private int _health = 100;
        private int _attackPower = 10;
        private int _defense = 5;
        private float _speed = 1.0f;
        private float _thinkingInterval = 2.0f;

        /// <summary>
        /// Creates a new CharacterBuilder with required parameters.
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="simulation">The simulation instance</param>
        public CharacterBuilder(string name, int x, int y, Simulation simulation)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _x = x;
            _y = y;
            _simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
        }

        /// <summary>
        /// Sets a custom brain for the character.
        /// </summary>
        /// <param name="brain">The brain to use</param>
        /// <returns>This builder instance for method chaining</returns>
        public CharacterBuilder WithBrain(Brain brain)
        {
            _brain = brain ?? throw new ArgumentNullException(nameof(brain));
            return this;
        }

        /// <summary>
        /// Sets the character to be human-controlled.
        /// </summary>
        /// <param name="awareness">Awareness radius for the human brain</param>
        /// <returns>This builder instance for method chaining</returns>
        public CharacterBuilder WithHumanControl(int awareness = 10)
        {
            _isHumanControlled = true;
            _awareness = awareness;
            return this;
        }

        /// <summary>
        /// Sets the character to be AI-controlled.
        /// </summary>
        /// <param name="awareness">Awareness radius for the AI brain</param>
        /// <param name="agentConfigThinkInterval"></param>
        /// <returns>This builder instance for method chaining</returns>
        public CharacterBuilder WithAIControl(int awareness = 10, float agentConfigThinkInterval = .5f)
        {
            _isHumanControlled = false;
            _awareness = awareness;
            _thinkingInterval = agentConfigThinkInterval;
            return this;
        }

        /// <summary>
        /// Sets the weapons for the character.
        /// </summary>
        /// <param name="weapons">Array of weapons</param>
        /// <returns>This builder instance for method chaining</returns>
        public CharacterBuilder WithWeapons(params Weapon[] weapons)
        {
            _weapons = weapons ?? Array.Empty<Weapon>();
            return this;
        }

        /// <summary>
        /// Sets a custom collider for the character.
        /// </summary>
        /// <param name="collider">The collider to use</param>
        /// <returns>This builder instance for method chaining</returns>
        public CharacterBuilder WithCollider(ICollider collider)
        {
            _collider = collider ?? throw new ArgumentNullException(nameof(collider));
            return this;
        }

        /// <summary>
        /// Sets the dimensions for the character's collider.
        /// </summary>
        /// <param name="width">Width of the collider</param>
        /// <param name="height">Height of the collider</param>
        /// <returns>This builder instance for method chaining</returns>
        public CharacterBuilder WithDimensions(int width, int height)
        {
            _width = width > 0 ? width : 1;
            _height = height > 0 ? height : 1;
            return this;
        }

        /// <summary>
        /// Sets the health properties for the character.
        /// </summary>
        /// <param name="maxHealth">Maximum health</param>
        /// <param name="currentHealth">Current health (defaults to maxHealth if not specified)</param>
        /// <returns>This builder instance for method chaining</returns>
        public CharacterBuilder WithHealth(int maxHealth, int? currentHealth = null)
        {
            _maxHealth = maxHealth > 0 ? maxHealth : 1;
            _health = currentHealth ?? _maxHealth;
            return this;
        }

        /// <summary>
        /// Sets the combat stats for the character.
        /// </summary>
        /// <param name="attackPower">Attack power</param>
        /// <param name="defense">Defense</param>
        /// <returns>This builder instance for method chaining</returns>
        public CharacterBuilder WithCombatStats(int attackPower, int defense)
        {
            _attackPower = attackPower >= 0 ? attackPower : 0;
            _defense = defense >= 0 ? defense : 0;
            return this;
        }

        /// <summary>
        /// Sets the movement speed for the character.
        /// </summary>
        /// <param name="speed">Movement speed</param>
        /// <returns>This builder instance for method chaining</returns>
        public CharacterBuilder WithSpeed(float speed)
        {
            _speed = speed > 0 ? speed : 0.1f;
            return this;
        }

        /// <summary>
        /// Builds and returns a Character instance with the configured properties.
        /// </summary>
        /// <returns>A new Character instance</returns>
        public Character Build()
        {
            Character character;

            // Create the character with the appropriate constructor based on configuration
            if (_brain != null)
            {
                // Use the provided brain
                if (_collider != null)
                {
                    character = new Character(_name, _x, _y, _brain, _simulation, _weapons, _collider);
                }
                else
                {
                    character = new Character(_name, _x, _y, _brain, _simulation, _weapons, _width, _height);
                }
            }
            else
            {
                // Create a brain based on control type
                if (_collider != null)
                {
                    character = new Character(_name, _x, _y, _awareness, _simulation, _weapons, _collider, _isHumanControlled);
                }
                else
                {
                    character = new Character(_name, _x, _y, _awareness, _simulation, _weapons, _width, _height, _isHumanControlled);
                }
            }

            // Set additional properties
            character.MaxHealth = _maxHealth;
            character.Health = _health;
            character.AttackPower = _attackPower;
            character.Defense = _defense;
            character.Speed = _speed;

            return character;
        }
    }
}