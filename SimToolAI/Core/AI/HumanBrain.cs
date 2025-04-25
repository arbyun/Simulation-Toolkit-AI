using System.Numerics;
using External.SimToolAI.SimToolAI.Core.AI;
using SimToolAI.Core.Entities;

namespace SimToolAI.Core.AI
{
    /// <summary>
    /// Brain implementation for human-controlled entities
    /// </summary>
    public class HumanBrain : Brain
    {
        #region Properties
        
        /// <summary>
        /// The last movement direction input by the human player
        /// </summary>
        private Vector3? _lastMovementInput;
        
        /// <summary>
        /// The target of the attack, if any
        /// </summary>
        private Entity _attackTarget;
        
        #endregion
        
        #region Constructors

        /// <summary>
        /// Creates a new human brain with the specified parameters
        /// </summary>
        /// <param name="owner">The entity this brain controls</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="simulation">Simulation instance</param>
        public HumanBrain(Character owner, int awareness, Simulation simulation) 
            : base(owner, awareness, simulation)
        {
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Updates the brain state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public override void Think(float deltaTime)
        {
        }

        /// <summary>
        /// Gets the movement direction requested by the human player
        /// </summary>
        /// <returns>The movement direction</returns>
        public override Vector3 GetMovementDirection()
        {
            if (_lastMovementInput != null) 
                return _lastMovementInput.Value;

            return new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Gets the interaction target (if any)
        /// </summary>
        /// <returns>The interaction target, or null if none is set</returns>
        public override Entity GetInteractionTarget()
        {
            return _attackTarget;
        }
        
        /// <summary>
        /// Sets the movement input from the human player
        /// </summary>
        /// <param name="direction">Direction to move</param>
        public bool SetMovementInput(Vector3 direction)
        {
            // Store the input for future reference
            _lastMovementInput = direction;
            
            // Always update the facing direction, even if we can't move
            Owner.FacingDirection = direction;
            
            // Immediately try to move in the given direction
            // The simulation will handle the actual movement and raise events
            return Move(direction);
        }

        /// <summary>
        /// Sets the attack input from the human player
        /// </summary>
        /// <param name="target">The attack direction</param>
        /// <param name="targetEntity">Target entity to attack</param>
        public bool SetAttackInput(Vector3 target, Entity targetEntity)
        {
            _attackTarget = targetEntity;
            return Attack(target);
        }
        
        #endregion
    }
}