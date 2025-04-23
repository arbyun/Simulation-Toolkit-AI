using System;
using System.Numerics;
using SimToolAI.Core.Rendering;
using SimToolAI.Core.Rendering.RenderStrategies;

namespace SimToolAI.Core.Entities
{
    /// <summary>
    /// Represents a projectile in the simulation
    /// </summary>
    public class Bullet : Entity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the direction the bullet is traveling
        /// </summary>
        public Vector3 Direction { get; }

        /// <summary>
        /// Gets or sets the damage the bullet deals
        /// </summary>
        public int Damage { get; }

        /// <summary>
        /// Gets or sets the maximum distance the bullet can travel
        /// </summary>
        public int MaxRange { get; set; } = 50;

        /// <summary>
        /// Gets or sets the distance the bullet has traveled
        /// </summary>
        public int DistanceTraveled { get; private set; }

        /// <summary>
        /// Gets whether the bullet has reached its maximum range
        /// </summary>
        public bool ReachedMaxRange => DistanceTraveled >= MaxRange;

        /// <summary>
        /// Time since the last movement
        /// </summary>
        private float _timeSinceLastMove;

        /// <summary>
        /// Whether the bullet blocks movement
        /// </summary>
        public override bool BlocksMovement => false;
        
        /// <summary>
        /// The owner of the bullet
        /// </summary>
        private readonly Entity _owner;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new bullet with the specified parameters
        /// </summary>
        /// <param name="x">Starting X position</param>
        /// <param name="y">Starting Y position</param>
        /// <param name="direction">Direction the bullet will travel</param>
        /// <param name="owner">The owner of the bullet</param>
        /// <param name="speed">Speed of the bullet (per second)</param>
        /// <param name="damage">Damage the bullet deals</param>
        /// <param name="simulation"></param>
        public Bullet(int x, int y, Vector3 direction, Simulation simulation, Entity owner, float speed = 10, int damage = 1)
            : base("bullet", x, y, simulation)
        {
            Direction = direction;
            Speed = speed;
            Damage = damage;
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            // Create a bullet renderable
            Avatar = new ConsoleNotRenderable();
        }

        /// <summary>
        /// Creates a new bullet (will get map reference from scene)
        /// </summary>
        /// <param name="x">Starting X position</param>
        /// <param name="y">Starting Y position</param>
        /// <param name="direction">Direction the bullet will travel</param>
        /// <param name="renderable">The renderable used by the bullet </param>
        /// <param name="speed">Speed of the bullet (per second)</param>
        /// <param name="damage">Damage the bullet deals</param>
        /// <param name="simulation">The simulation instance</param>
        /// <param name="owner">The owner of the bullet</param>
        public Bullet(int x, int y, Vector3 direction, Simulation simulation, Entity owner, IRenderable renderable, 
            float speed = 10, int damage = 1)
            : base("bullet", x, y, simulation)
        {
            Direction = direction;
            Speed = speed;
            Damage = damage;
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            // Create a bullet renderable
            Avatar = renderable;
            Avatar.Connect(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the bullet state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public override void Update(float deltaTime)
        {
            // Accumulate time since last move
            _timeSinceLastMove += deltaTime;

            // Check if it's time to move
            if (_timeSinceLastMove >= 1.0f / Speed)
            {
                _timeSinceLastMove = 0;
                MoveBullet();
            }
        }

        /// <summary>
        /// Moves the bullet in its current direction
        /// </summary>
        private void MoveBullet()
        {
            // Calculate new position based on direction
            int newX = X;
            int newY = Y;

            newX += (int)Direction.X;
            newY += (int)Direction.Y;

            // Check if the bullet has reached its maximum range
            if (++DistanceTraveled > MaxRange)
            {
                // Bullet reached maximum range, remove it
                Simulation.Scene.RemoveEntity(this);
                return;
            }

            // Check if the new position is within map bounds
            if (newX < 0 || newX >= Simulation.Map.Width || newY < 0 || newY >= Simulation.Map.Height)
            {
                // Bullet went out of bounds, remove it
                Simulation.Scene.RemoveEntity(this);
                return;
            }

            // Check if the new position is walkable
            if (!Simulation.Map.IsWalkable(newX, newY))
            {
                // Bullet hit something, remove it
                Simulation.Scene.RemoveEntity(this);
                return;
            }

            // Check if there's an entity at the new position
            var entity = Simulation.Scene.GetEntityAt(newX, newY);
            if (entity != null && !entity.Equals(this) && !entity.Equals(_owner))
            {
                // Bullet hit an entity
                HandleEntityCollision(entity);
                return;
            }

            // Move the bullet
            X = newX;
            Y = newY;

            if (!(Avatar is ConsoleNotRenderable))
            {
                // Trigger a render update
                Simulation.Scene.QueryScene<bool>("SetRenderRequired", true);
            }
        }

        /// <summary>
        /// Handles collision with an entity
        /// </summary>
        /// <param name="entity">Entity the bullet collided with</param>
        private void HandleEntityCollision(Entity entity)
        {
            // If the entity is a player, damage it
            if (entity is Player player)
            {
                player.TakeDamage(Damage);
            }

            // Remove the bullet for any other entity type
            Simulation.Scene.RemoveEntity(this);
        }

        #endregion
    }
}