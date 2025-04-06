using System;
using RogueSharp;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;

namespace SimToolAI.Utilities
{
    /// <summary>
    /// Command pattern implementation for game actions
    /// </summary>
    public static class CommandSystem
    {
        /// <summary>
        /// Moves an entity in the specified direction
        /// </summary>
        /// <param name="direction">Direction to move</param>
        /// <param name="entity">Entity to move</param>
        /// <param name="map">Map to move on</param>
        /// <returns>True if the entity was moved, false otherwise</returns>
        public static bool MoveEntity(Direction direction, Entity entity, ISimMap map)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (map == null)
                throw new ArgumentNullException(nameof(map));

            // Calculate the new position based on the direction
            var (dx, dy) = direction.ToVector();
            int newX = entity.X + dx;
            int newY = entity.Y + dy;

            // Update the entity's facing direction if it's a player
            if (entity is Player player)
            {
                player.FacingDirection = direction;
            }

            // Try to move the entity
            return map.SetEntityPosition(entity, newX, newY);
        }

        /// <summary>
        /// Moves a player in the specified direction
        /// </summary>
        /// <param name="direction">Direction to move</param>
        /// <param name="player">Player to move</param>
        /// <param name="map">Map to move on</param>
        /// <returns>True if the player was moved, false otherwise</returns>
        public static bool MovePlayer(Direction direction, Entity player, ISimMap map)
        {
            return MoveEntity(direction, player, map);
        }

        /// <summary>
        /// Moves an entity to a specific cell
        /// </summary>
        /// <param name="entity">Entity to move</param>
        /// <param name="cell">Cell to move to</param>
        /// <param name="map">Map to move on</param>
        /// <returns>True if the entity was moved, false otherwise</returns>
        public static bool MoveToCell(Entity entity, ICell cell, ISimMap map)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (cell == null)
                throw new ArgumentNullException(nameof(cell));

            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return map.SetEntityPosition(entity, cell.X, cell.Y);
        }

        /// <summary>
        /// Creates a bullet at the specified position
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="direction">Direction the bullet will travel</param>
        /// <param name="scene">Scene to add the bullet to</param>
        /// <param name="speed">Speed of the bullet</param>
        /// <param name="damage">Damage the bullet deals</param>
        /// <returns>The created bullet</returns>
        public static Bullet FireBullet(int x, int y, Direction direction, Scene scene, float speed = 10, int damage = 1)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));

            // Create a new bullet
            var bullet = new Bullet(x, y, direction, scene, speed, damage);
            bullet.FacingDirection = direction;

            // Add the bullet to the scene
            scene.AddEntity(bullet);

            // Trigger a render update
            scene.QueryScene<bool>("SetRenderRequired", true);

            return bullet;
        }

        /// <summary>
        /// Creates a bullet at the specified position
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="direction">Direction the bullet will travel</param>
        /// <param name="scene">Scene to add the bullet to</param>
        /// <param name="speed">Speed of the bullet</param>
        /// <param name="damage">Damage the bullet deals</param>
        /// <returns>The created bullet</returns>
        public static Bullet FireBullet(int x, int y, Direction direction, IScene scene, float speed = 10, int damage = 1)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));

            // If the scene is already a Scene, use it directly
            if (scene is Scene sceneImpl)
            {
                return FireBullet(x, y, direction, sceneImpl, speed, damage);
            }

            // Otherwise, create an adapter
            var adapter = new SceneAdapter(scene);
            return FireBullet(x, y, direction, adapter, speed, damage);
        }

        /// <summary>
        /// Creates a bullet at the player's position
        /// </summary>
        /// <param name="player">Player to fire from</param>
        /// <param name="scene">Scene to add the bullet to</param>
        /// <param name="speed">Speed of the bullet</param>
        /// <param name="damage">Damage the bullet deals</param>
        /// <returns>The created bullet</returns>
        public static Bullet FireBullet(Player player, Scene scene, float speed = 10, int damage = 1)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return FireBullet(player.X, player.Y, player.FacingDirection, scene, speed, damage);
        }

        /// <summary>
        /// Creates a bullet at the player's position
        /// </summary>
        /// <param name="player">Player to fire from</param>
        /// <param name="scene">Scene to add the bullet to</param>
        /// <param name="speed">Speed of the bullet</param>
        /// <param name="damage">Damage the bullet deals</param>
        /// <returns>The created bullet</returns>
        public static Bullet FireBullet(Player player, IScene scene, float speed = 10, int damage = 1)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return FireBullet(player.X, player.Y, player.FacingDirection, scene, speed, damage);
        }
    }
}