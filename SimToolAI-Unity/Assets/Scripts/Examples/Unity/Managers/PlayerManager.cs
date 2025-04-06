using Examples.Unity.Cosmetic;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;
using SimToolAI.Core.Rendering.RenderStrategies;
using SimToolAI.Utilities;
using UnityEngine;

namespace Examples.Unity.Managers
{
    /// <summary>
    /// Manages player creation and movement
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private float moveSpeed = 5f;

        /// <summary>
        /// The player GameObject
        /// </summary>
        public GameObject PlayerObject { get; private set; }

        /// <summary>
        /// The player entity
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// The player animations component
        /// </summary>
        public PlayerAnimations PlayerAnimations { get; private set; }

        /// <summary>
        /// The target position for the player
        /// </summary>
        private Vector3 _targetPos = Vector3.zero;

        /// <summary>
        /// Creates the player at a random walkable location
        /// </summary>
        /// <param name="map">The map</param>
        /// <param name="scene">The scene</param>
        /// <param name="grid">The grid</param>
        public void CreatePlayer(ISimMap map, UnityScene scene, Grid grid)
        {
            var startPos = map.GetRandomWalkableLocation() ?? (5, 5);

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
            PlayerAnimations = PlayerObject.GetComponent<PlayerAnimations>();

            Data renderData = new Data();
            renderData.Set("transform", PlayerObject.transform);
            renderData.Set("grid", grid);
            renderData.Set("entity", Player);

            UnityEntityRenderable playerRenderable = new UnityEntityRenderable(renderData);
            Player.Avatar = playerRenderable;

            scene.AddEntity(Player);
            map.ToggleFieldOfView(Player);
        }

        /// <summary>
        /// Moves the player in the specified direction
        /// </summary>
        /// <param name="moveDirection">The direction to move</param>
        /// <param name="map">The map</param>
        /// <param name="grid">The grid</param>
        /// <returns>True if the player moved, false otherwise</returns>
        public bool MovePlayer(Direction moveDirection, ISimMap map, Grid grid)
        {
            bool moved = CommandSystem.MovePlayer(moveDirection, Player, map);

            if (moved)
            {
                // Update the player's facing direction
                Player.FacingDirection = moveDirection;

                // Update the target position for visualization
                _targetPos = grid.GetCellCenterWorld(new Vector3Int(Player.X, Player.Y));
            }

            return moved;
        }

        /// <summary>
        /// Gets the current target position
        /// </summary>
        public Vector3 GetTargetPosition()
        {
            return _targetPos;
        }

        /// <summary>
        /// Checks if the player has reached the target position
        /// </summary>
        /// <param name="grid">The grid</param>
        /// <returns>True if the player has reached the target position, false otherwise</returns>
        public bool HasReachedTargetPosition(Grid grid)
        {
            if (Player == null || PlayerObject == null)
                return false;

            Vector3 targetPosition = grid.GetCellCenterWorld(new Vector3Int(Player.X, Player.Y));
            float distanceToTarget = Vector3.Distance(PlayerObject.transform.position, targetPosition);

            return distanceToTarget < 0.1f;
        }
    }
}