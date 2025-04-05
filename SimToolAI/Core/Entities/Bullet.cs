using System;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;
using SimToolAI.Core.Rendering.RenderStrategies;
using SimToolAI.Utilities;

namespace SimToolAI.Core.Entities
{
    public class Bullet : Entity
    {
        public Direction Direction { get; set; }
        public float Speed { get; set; }
        public int Damage { get; set; }
        
        private ISimMap _map;
        private Scene _scene;
        private float _timeSinceLastMove = 0;

        /// <summary>
        /// Creates a new bullet
        /// </summary>
        /// <param name="x">Starting X position</param>
        /// <param name="y">Starting Y position</param>
        /// <param name="direction">Direction the bullet will travel</param>
        /// <param name="map">Our map</param>
        /// <param name="scene">Our scene</param>
        /// <param name="speed">Speed of the bullet (cells per second)</param>
        /// <param name="damage">Damage the bullet deals</param>
        public Bullet(int x, int y, Direction direction, ISimMap map, Scene scene, float speed = 10, int damage = 1)
            : base("bullet", x, y, 1)
        {
            Direction = direction;
            Speed = speed;
            Damage = damage;
            _map = map;
            _scene = scene;

            // Create a bullet renderable
            Avatar = new ConsoleEntityRenderable('*', ConsoleColor.Red, ConsoleColor.Black, this);
        }

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

        private void MoveBullet()
        {
            // Calculate new position based on direction
            int newX = X;
            int newY = Y;

            var vector = Direction.ToVector();
            newX += vector.Item1;
            newY += vector.Item2;

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

            // Move the bullet
            X = newX;
            Y = newY;

            // Trigger a render update
            _scene.QueryScene<bool>("SetRenderRequired", true);
        }
    }
}