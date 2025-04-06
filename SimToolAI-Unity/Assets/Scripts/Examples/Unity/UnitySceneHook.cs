using Examples.Unity.Managers;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;
using SimToolAI.Utilities;
using UnityEngine;

namespace Examples.Unity
{
    /// <summary>
    /// Example class that showcases how to use the toolkit with Unity to render a grid-based map.
    /// This class coordinates the different managers that handle specific responsibilities.
    /// </summary>
    public class UnitySceneHook : MonoBehaviour
    {
        [SerializeField] private MapManager mapManager;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private BulletManager bulletManager;
        [SerializeField] private CameraController cameraController;

        /// <summary>
        /// The Unity scene
        /// </summary>
        public UnityScene UnityScene { get; private set; }

        /// <summary>
        /// The map
        /// </summary>
        public ISimMap Map => mapManager?.Map;

        /// <summary>
        /// The player
        /// </summary>
        public Player Player => playerManager?.Player;

        /// <summary>
        /// The player GameObject
        /// </summary>
        public GameObject PlayerObject => playerManager?.PlayerObject;

        private void Awake()
        {
            // Initialize the input manager
            inputManager.Initialize();

            // Subscribe to input events
            inputManager.FireActionPerformed += OnFireActionPerformed;
            inputManager.FireActionCanceled += OnFireActionCanceled;
        }

        private void Start()
        {
            // Initialize the map
            mapManager.Initialize();

            // Create the Unity scene
            UnityScene = new UnityScene(mapManager.Map);

            // Create the player
            playerManager.CreatePlayer(mapManager.Map, UnityScene, mapManager.Grid);

            // Initialize the bullet manager
            bulletManager.Initialize(UnityScene);
        }

        private void Update()
        {
            // Process movement input
            ProcessMovementInput();

            // Update the bullet manager
            bulletManager.ManualUpdate(Time.deltaTime, playerManager.Player, 
                playerManager.PlayerAnimations, UnityScene, mapManager.Grid);

            // Update the camera position
            cameraController.UpdateCameraPosition(playerManager.PlayerObject.transform.position);

            // Update and render the scene
            UnityScene.Update(Time.deltaTime);
            UnityScene.Render();
        }

        private void OnDestroy()
        {
            // Clean up the input manager
            inputManager.Cleanup();

            // Unsubscribe from input events
            inputManager.FireActionPerformed -= OnFireActionPerformed;
            inputManager.FireActionCanceled -= OnFireActionCanceled;
        }

        /// <summary>
        /// Processes movement input from the input system
        /// </summary>
        private void ProcessMovementInput()
        {
            // Only allow new movement input if the player is close to their target position
            if (playerManager.HasReachedTargetPosition(mapManager.Grid))
            {
                // Get movement direction
                Direction moveDirection = inputManager.GetMovementDirection();

                if (moveDirection != Direction.None)
                {
                    // Move the player
                    playerManager.MovePlayer(moveDirection, mapManager.Map, mapManager.Grid);
                }
            }
        }

        /// <summary>
        /// Called when the fire action is performed (button pressed)
        /// </summary>
        private void OnFireActionPerformed()
        {
            bulletManager.StartFiring(playerManager.Player, playerManager.PlayerAnimations, UnityScene, mapManager.Grid);
        }

        /// <summary>
        /// Called when the fire action is canceled (button released)
        /// </summary>
        private void OnFireActionCanceled()
        {
            bulletManager.StopFiring();
        }

        private void OnDrawGizmos()
        {
            if (playerManager != null && playerManager.PlayerObject != null)
            {
                Vector3 targetPos = playerManager.GetTargetPosition();
                if (targetPos != Vector3.zero)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(playerManager.PlayerObject.transform.position, targetPos);

                    // Draw a sphere at the target position
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(targetPos, 0.1f);
                }
            }
        }
    }
}