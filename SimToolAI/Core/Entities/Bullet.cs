using System;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;
using SimToolAI.Core.Rendering.RenderStrategies;
using SimToolAI.Utilities;

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
        public Direction Direction { get; }

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
        /// Reference to the map
        /// </summary>
        private ISimMap _map;

        /// <summary>
        /// Reference to the scene
        /// </summary>
        private readonly Scene _scene;

        /// <summary>
        /// Time since the last movement
        /// </summary>
        private float _timeSinceLastMove;

        /// <summary>
        /// Whether the bullet blocks movement
        /// </summary>
        public override bool BlocksMovement => false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new bullet with the specified parameters
        /// </summary>
        /// <param name="x">Starting X position</param>
        /// <param name="y">Starting Y position</param>
        /// <param name="direction">Direction the bullet will travel</param>
        /// <param name="map">Map reference</param>
        /// <param name="scene">Scene reference</param>
        /// <param name="bulletSpeed">Speed of the bullet (cells per second)</param>
        /// <param name="speed">Speed of the bullet (pixels per second)</param>
        /// <param name="damage">Damage the bullet deals</param>
        public Bullet(int x, int y, Direction direction, ISimMap map, Scene scene, float speed = 10, int damage = 1)
            : base("bullet", x, y, 1)
        {
            Direction = direction;
            Speed = speed;
            Damage = damage;
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _scene = scene ?? throw new ArgumentNullException(nameof(scene));

            // Create a bullet renderable
            Avatar = new ConsoleEntityRenderable('*', ConsoleColor.Red, ConsoleColor.Black, this);
        }

        /// <summary>
        /// Creates a new bullet (will get map reference from scene)
        /// </summary>
        /// <param name="x">Starting X position</param>
        /// <param name="y">Starting Y position</param>
        /// <param name="direction">Direction the bullet will travel</param>
        /// <param name="scene">Scene reference</param>
        /// <param name="bulletSpeed">Speed of the bullet (cells per second)</param>
        /// <param name="speed">Speed of the bullet (pixels per second)</param>
        /// <param name="damage">Damage the bullet deals</param>
        public Bullet(int x, int y, Direction direction, Scene scene, float speed = 10, int damage = 1)
            : base("bullet", x, y, 1)
        {
            Direction = direction;
            Speed = speed;
            Damage = damage;
            _scene = scene ?? throw new ArgumentNullException(nameof(scene));

            // Get the map from the scene
            _map = scene.Map;

            // Create a bullet renderable
            Avatar = new ConsoleEntityRenderable('*', ConsoleColor.Red, ConsoleColor.Black, this);
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
            // If we don't have a map or scene reference yet, try to get them
            if (_map == null)
            {
                // Find the scene this bullet is in
                if (_scene != null)
                {
                    _map = _scene.QueryScene<ISimMap>("GetMap");
                }
                else
                {
                    return; // Can't move without scene and map
                }
            }

            // Calculate new position based on direction
            int newX = X;
            int newY = Y;

            var vector = Direction.ToVector();
            newX += vector.Item1;
            newY += vector.Item2;

            // Check if the bullet has reached its maximum range
            if (++DistanceTraveled > MaxRange)
            {
                // Bullet reached maximum range, remove it
                _scene.RemoveEntity(this);
                return;
            }

            // Check if the new position is within map bounds
            if (newX < 0 || newX >= _map.Width || newY < 0 || newY >= _map.Height)
            {
                // Bullet went out of bounds, remove it
                _scene.RemoveEntity(this);
                return;
            }

            // Check if the new position is walkable
            if (!_map.IsWalkable(newX, newY))
            {
                // Bullet hit something, remove it
                _scene.RemoveEntity(this);
                return;
            }

            // Check if there's an entity at the new position
            var entity = _scene.GetEntityAt(newX, newY);
            if (entity != null && !entity.Equals(this))
            {
                // Bullet hit an entity
                HandleEntityCollision(entity);
                return;
            }

            // Move the bullet
            X = newX;
            Y = newY;

            // Trigger a render update
            _scene.QueryScene<bool>("SetRenderRequired", true);
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
                return;
            }

            // Remove the bullet
            _scene.RemoveEntity(this);
        }

        #endregion
    }
}