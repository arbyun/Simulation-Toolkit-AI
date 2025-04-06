using SimToolAI.Utilities;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Examples.Unity.Managers
{
    /// <summary>
    /// Manages input processing
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private InputActionAsset inputActions;

        /// <summary>
        /// Event triggered when the fire action is performed
        /// </summary>
        public event Action FireActionPerformed;

        /// <summary>
        /// Event triggered when the fire action is canceled
        /// </summary>
        public event Action FireActionCanceled;

        /// <summary>
        /// The move action
        /// </summary>
        private InputAction _moveAction;

        /// <summary>
        /// The fire action
        /// </summary>
        private InputAction _fireAction;

        /// <summary>
        /// Initializes the input manager
        /// </summary>
        public void Initialize()
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

        /// <summary>
        /// Cleans up the input manager
        /// </summary>
        public void Cleanup()
        {
            if (_fireAction != null)
            {
                _fireAction.performed -= OnFireActionPerformed;
                _fireAction.canceled -= OnFireActionCanceled;
            }
        }

        /// <summary>
        /// Gets the movement direction from the input
        /// </summary>
        /// <returns>The movement direction, or Direction.None if no input</returns>
        public Direction GetMovementDirection()
        {
            if (_moveAction == null)
                return Direction.None;

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

                return moveDirection;
            }

            return Direction.None;
        }

        /// <summary>
        /// Called when the fire action is performed
        /// </summary>
        private void OnFireActionPerformed(InputAction.CallbackContext context)
        {
            FireActionPerformed?.Invoke();
        }

        /// <summary>
        /// Called when the fire action is canceled
        /// </summary>
        private void OnFireActionCanceled(InputAction.CallbackContext context)
        {
            FireActionCanceled?.Invoke();
        }
    }
}