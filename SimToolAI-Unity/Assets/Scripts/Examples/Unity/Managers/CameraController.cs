using UnityEngine;

namespace Examples.Unity.Managers
{
    /// <summary>
    /// Controls the camera to follow the player
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3 cameraOffset;

        /// <summary>
        /// Updates the camera position to follow the player
        /// </summary>
        /// <param name="playerPosition">The player's position</param>
        public void UpdateCameraPosition(Vector3 playerPosition)
        {
            if (mainCamera == null)
                return;

            mainCamera.transform.position = new Vector3(
                playerPosition.x + cameraOffset.x,
                playerPosition.y + cameraOffset.y,
                cameraOffset.z);
        }
    }
}