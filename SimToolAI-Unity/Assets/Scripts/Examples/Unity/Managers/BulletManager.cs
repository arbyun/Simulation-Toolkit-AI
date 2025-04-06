using Examples.Unity.Cosmetic;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Rendering;
using SimToolAI.Utilities;
using System.Collections.Generic;
using SimToolAI.Core.Rendering.RenderStrategies;
using UnityEngine;

namespace Examples.Unity.Managers
{
    /// <summary>
    /// Manages bullet pool and firing logic
    /// </summary>
    public class BulletManager : MonoBehaviour
    {
        [Header("Bullet Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float fireRate = 0.25f;
        [SerializeField] private int initialPoolSize = 20;

        /// <summary>
        /// The bullet pool
        /// </summary>
        private readonly Queue<GameObject> _bulletPool = new();

        /// <summary>
        /// The active bullets
        /// </summary>
        private readonly Dictionary<Bullet, GameObject> _activeBullets = new();

        /// <summary>
        /// Whether the player is firing
        /// </summary>
        private bool _isFiring;

        /// <summary>
        /// Time since the last shot
        /// </summary>
        private float _timeSinceLastShot;

        /// <summary>
        /// Initializes the bullet manager
        /// </summary>
        /// <param name="scene">The scene</param>
        public void Initialize(UnityScene scene)
        {
            InitializeBulletPool();

            // Subscribe to the entity removed event to handle bullet recycling
            scene.EntityRemoved += OnEntityRemoved;
        }

        /// <summary>
        /// Updates the bullet manager
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update</param>
        /// <param name="player">The player</param>
        /// <param name="playerAnimations">The player animations</param>
        /// <param name="scene">The scene</param>
        /// <param name="grid">The grid</param>
        public void ManualUpdate(float deltaTime, Player player, 
            PlayerAnimations playerAnimations, UnityScene scene, Grid grid)
        {
            if (_isFiring && player != null)
            {
                _timeSinceLastShot += deltaTime;

                // Check if enough time has passed since the last shot
                if (_timeSinceLastShot >= fireRate)
                {
                    FireBullet(player, playerAnimations, scene, grid);
                    _timeSinceLastShot = 0f;
                }
            }
        }

        /// <summary>
        /// Starts firing
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="playerAnimations">The player animations</param>
        /// <param name="scene">The scene</param>
        /// <param name="grid">The grid</param>
        public void StartFiring(Player player, PlayerAnimations playerAnimations, UnityScene scene, Grid grid)
        {
            if (player == null)
                return;

            // Start continuous firing
            _isFiring = true;

            // Fire immediately on first press
            FireBullet(player, playerAnimations, scene, grid);
            _timeSinceLastShot = 0f;
        }

        /// <summary>
        /// Stops firing
        /// </summary>
        public void StopFiring()
        {
            // Stop continuous firing
            _isFiring = false;
        }

        /// <summary>
        /// Initializes the bullet pool
        /// </summary>
        private void InitializeBulletPool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject bulletObj = Instantiate(bulletPrefab, transform);
                bulletObj.SetActive(false);
                _bulletPool.Enqueue(bulletObj);
            }
        }

        /// <summary>
        /// Fires a bullet
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="playerAnimations">The player animations</param>
        /// <param name="scene">The scene</param>
        /// <param name="grid">The grid</param>
        private void FireBullet(Player player, PlayerAnimations playerAnimations, UnityScene scene, Grid grid)
        {
            // Cosmetics
            playerAnimations.TriggerAnimation(PlayerAnimations.SHOOT_ANIMATION);

            // Fire a bullet in the player's facing direction
            var bullet = CommandSystem.FireBullet(player, scene, 8, 8);

            // Create a Unity representation for the bullet
            if (bullet != null)
            {
                bullet.MaxRange = 50;
                CreateBulletObject(bullet, grid);
            }
        }

        /// <summary>
        /// Creates or reuses a Unity representation for a bullet
        /// </summary>
        /// <param name="bullet">The bullet</param>
        /// <param name="grid">The grid</param>
        private void CreateBulletObject(Bullet bullet, Grid grid)
        {
            var bulletObj =
                // Try to get a bullet from the pool
                _bulletPool.Count > 0 ? _bulletPool.Dequeue() :
                // If the pool is empty, create a new bullet
                Instantiate(bulletPrefab);

            // Activate and position the bullet
            bulletObj.SetActive(true);
            bulletObj.transform.position = grid.GetCellCenterWorld(new Vector3Int(bullet.X, bullet.Y));
            bulletObj.name = $"Bullet_{bullet.Id}";

            // Track the active bullet
            _activeBullets[bullet] = bulletObj;

            // Set up the bullet's renderable
            Data bulletRenderableData = new Data();
            bulletRenderableData.Set("transform", bulletObj.transform);
            bulletRenderableData.Set("grid", grid);
            bulletRenderableData.Set("entity", bullet);

            UnityEntityRenderable bulletRenderable = new UnityEntityRenderable(bulletRenderableData);
            bullet.Avatar = bulletRenderable;
        }

        /// <summary>
        /// Handles entity removal events to recycle bullet objects
        /// </summary>
        private void OnEntityRemoved(object sender, EntityEventArgs e)
        {
            if (e.Entity is Bullet bullet && _activeBullets.TryGetValue(bullet, out GameObject bulletObj))
            {
                // Return the bullet to the pool
                bulletObj.SetActive(false);
                _bulletPool.Enqueue(bulletObj);

                // Remove from active bullets
                _activeBullets.Remove(bullet);
            }
        }
    }
}