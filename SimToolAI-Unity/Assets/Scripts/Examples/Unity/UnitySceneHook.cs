using Examples.Unity.Cosmetic;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering.RenderStrategies;
using SimToolAI.Utilities;
using System.Collections.Generic;
using SimToolAI.Core.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Examples.Unity
{
    /// <summary>
    /// Example class that showcases how to use the toolkit with Unity to render a grid-based map.
    /// In a real-world scenario, you would likely want to separate these concerns into different classes or scripts.
    /// This example combines all of them into one script for simplicity and demonstration purposes.
    /// </summary>
    public class UnitySceneHook: MonoBehaviour
    {
        [Header("Map Settings")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileBase wallTile;
        [SerializeField] private TileBase floorTile;
        [SerializeField] private TextAsset mapText;
        [SerializeField] private Grid grid;

        [Header("Player Settings")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float fireRate = 0.25f;

        [Header("Input Settings")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("Camera Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3 cameraOffset;

        [Header("Bullet Pool Settings")]
        [SerializeField] private int initialPoolSize = 20;

        #region Properties

        public GameObject PlayerObject
        {
            get;
            private set;
        }

        public Player Player
        {
            get;
            private set;
        }

        public ISimMap Map
        {
            get;
            private set;
        }

        public UnityScene UnityScene
        {
            get;
            private set;
        }

        #endregion

        private InputAction _moveAction;
        private InputAction _fireAction;
        private PlayerAnimations _playerAnimations;
        private Vector3 _targetPos = Vector3.zero;
        private bool _isFiring;
        private float _timeSinceLastShot;

        // Bullet pooling
        private readonly Queue<GameObject> _bulletPool = new();
        private readonly Dictionary<Bullet, GameObject> _activeBullets = new();

        private void Awake()
        {
            if (inputActions != null)
            {
                _moveAction = inputActions.FindAction("Move");
                _fireAction = inputActions.FindAction("Fire");

                if (_moveAction != null && _fireAction != null)
                {
                    _moveAction.Enable();
                    _fireAction.Enable();

                    // Set up fire action callbacks
                    _fireAction.performed += OnFireActionPerformed;
                    _fireAction.canceled += OnFireActionCanceled;
                }
                else
                {
                    Debug.LogError("Required input actions not found in the Input Actions asset!");
                }
            }
            else
            {
                Debug.LogError("Input Actions asset is not assigned!");
            }
        }

        private void Start()
        {
            ParseMap();

            UnityScene = new UnityScene(Map);

            CreatePlayer();

            InitializeBulletPool();
        }

        private void Update()
        {
            ProcessMovementInput();
            ProcessContinuousFiring();

            mainCamera.transform.position = new Vector3(PlayerObject.transform.position.x + cameraOffset.x,
                PlayerObject.transform.position.y + cameraOffset.y, cameraOffset.z);

            UnityScene.Update(Time.deltaTime);
            UnityScene.Render();
        }

        private void OnDestroy()
        {
            if (_fireAction != null)
            {
                _fireAction.performed -= OnFireActionPerformed;
                _fireAction.canceled -= OnFireActionCanceled;
            }
        }

        #region Initialization Methods

        private void ParseMap()
        {
            GridMapParser<GridMap> map = new GridMapParser<GridMap>();

            Map = mapText ? map.LoadMapFromText(mapText.text) : map.LoadMapFromFile();

            Map.Initialize(new UnityMapRenderable(map.GetMapGrid(), Map.Height, Map.Width,
                tilemap, wallTile, floorTile));

            Map.Renderable.Render();
        }

        private void CreatePlayer()
        {
            var startPos = Map.GetRandomWalkableLocation() ?? (5, 5);

            Player = new Player("Player", startPos.Item1, startPos.Item2, 10)
            {
                Health = 100,
                MaxHealth = 100,
                AttackPower = 10,
                Defense = 5,
                Speed = moveSpeed,
                FacingDirection = Direction.Right
            };

            // Initialize the target position to the player's starting position
            _targetPos = grid.GetCellCenterWorld(new Vector3Int(Player.X, Player.Y));

            PlayerObject = Instantiate(playerPrefab);
            PlayerObject.transform.position = _targetPos;
            _playerAnimations = PlayerObject.GetComponent<PlayerAnimations>();

            Data renderData = new Data();
            renderData.Set("transform", PlayerObject.transform);
            renderData.Set("grid", grid);
            renderData.Set("entity", Player);

            UnityEntityRenderable playerRenderable = new UnityEntityRenderable(renderData);
            Player.Avatar = playerRenderable;

            UnityScene.AddEntity(Player);
            Map.ToggleFieldOfView(Player);
        }

        /// <summary>
        /// Initializes the bullet pool with a predefined number of bullet objects
        /// </summary>
        private void InitializeBulletPool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject bulletObj = Instantiate(bulletPrefab, PlayerObject.transform);
                bulletObj.SetActive(false);
                _bulletPool.Enqueue(bulletObj);
            }

            // Subscribe to the entity removed event to handle bullet recycling
            UnityScene.EntityRemoved += OnEntityRemoved;
        }

        /// <summary>
        /// Creates or reuses a Unity representation for a bullet
        /// </summary>
        private void CreateBulletObject(Bullet bullet)
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

        #endregion

        #region Input Processing

        /// <summary>
        /// Processes movement input from the input system
        /// </summary>
        private void ProcessMovementInput()
        {
            if (_moveAction != null && Player != null && PlayerObject != null)
            {
                // Check if player has reached the target position before allowing new movement
                Vector3 targetPosition = grid.GetCellCenterWorld(new Vector3Int(Player.X, Player.Y));
                float distanceToTarget = Vector3.Distance(PlayerObject.transform.position, targetPosition);

                // Only allow new movement input if the player is close to their target position
                if (distanceToTarget < 0.1f)
                {
                    // Get movement input
                    Vector2 moveInput = _moveAction.ReadValue<Vector2>();

                    if (moveInput.sqrMagnitude > 0.1f)
                    {
                        // Determine the direction based on input
                        Direction moveDirection = Direction.None;

                        // Convert 2D input to a direction
                        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                        {
                            // Horizontal movement takes precedence
                            moveDirection = moveInput.x > 0 ? Direction.Right : Direction.Left;
                        }
                        else
                        {
                            // Vertical movement - in Unity's input system, positive Y is up
                            moveDirection = moveInput.y > 0 ? Direction.Down : Direction.Up;
                        }

                        // Move the player
                        bool moved = CommandSystem.MovePlayer(moveDirection, Player, Map);

                        if (moved)
                        {
                            // Update the player's facing direction
                            Player.FacingDirection = moveDirection;

                            // Update the target position for visualization
                            _targetPos = grid.GetCellCenterWorld(new Vector3Int(Player.X, Player.Y));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Processes continuous firing when the fire button is held down
        /// </summary>
        private void ProcessContinuousFiring()
        {
            if (_isFiring && Player != null)
            {
                _timeSinceLastShot += Time.deltaTime;

                // Check if enough time has passed since the last shot
                if (_timeSinceLastShot >= fireRate)
                {
                    FireBullet();
                    _timeSinceLastShot = 0f;
                }
            }
        }

        /// <summary>
        /// Called when the fire action is performed (button pressed)
        /// </summary>
        private void OnFireActionPerformed(InputAction.CallbackContext context)
        {
            if (Player == null)
                return;

            // Start continuous firing
            _isFiring = true;

            // Fire immediately on first press
            FireBullet();
            _timeSinceLastShot = 0f;
        }

        /// <summary>
        /// Called when the fire action is canceled (button released)
        /// </summary>
        private void OnFireActionCanceled(InputAction.CallbackContext context)
        {
            // Stop continuous firing
            _isFiring = false;
        }

        /// <summary>
        /// Fires a bullet in the player's facing direction
        /// </summary>
        private void FireBullet()
        {
            // Cosmetics
            _playerAnimations.TriggerAnimation(PlayerAnimations.SHOOT_ANIMATION);

            // Fire a bullet in the player's facing direction
            var bullet = CommandSystem.FireBullet(Player, UnityScene, 8, 8);

            // Create a Unity representation for the bullet
            if (bullet != null)
            {
                bullet.MaxRange = 50;
                CreateBulletObject(bullet);
            }
        }

        #endregion

        private void OnDrawGizmos()
        {
            if (_targetPos != Vector3.zero && PlayerObject != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(PlayerObject.transform.position, _targetPos);

                // Draw a sphere at the target position
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_targetPos, 0.1f);
            }
        }
    }
}