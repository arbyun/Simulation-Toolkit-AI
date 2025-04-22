using SimToolAI.Core.Entities;
using SimToolAI.Core.Rendering;
using SimToolAI.Utilities;

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
        private Direction? _lastMovementInput;
        
        /// <summary>
        /// Whether the human player has requested an attack
        /// </summary>
        private bool _attackRequested;
        
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
        /// <param name="scene">Reference to the scene</param>
        public HumanBrain(Character owner, int awareness, Scene scene) 
            : base(owner, awareness, scene)
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
            // Human brains don't think autonomously - they respond to input
            // This method is intentionally left empty
        }
        
        /// <summary>
        /// Decides whether to move and in which direction
        /// </summary>
        /// <returns>Direction to move, or null if no movement is desired</returns>
        public override Direction? DecideMovement()
        {
            // Return the last movement input and clear it
            Direction? result = _lastMovementInput;
            _lastMovementInput = null;
            return result;
        }
        
        /// <summary>
        /// Decides whether to attack and which target
        /// </summary>
        /// <returns>Target to attack, or null if no attack is desired</returns>
        public override Entity DecideAttackTarget()
        {
            // Return the attack target if an attack was requested and clear it
            if (_attackRequested)
            {
                _attackRequested = false;
                Entity target = _attackTarget;
                _attackTarget = null;
                return target;
            }
            
            return null;
        }
        
        /// <summary>
        /// Sets the movement input from the human player
        /// </summary>
        /// <param name="direction">Direction to move</param>
        public void SetMovementInput(Direction direction)
        {
            _lastMovementInput = direction;
        }
        
        /// <summary>
        /// Sets the attack input from the human player
        /// </summary>
        /// <param name="target">Target to attack, or null for default direction</param>
        public void SetAttackInput(Entity target = null)
        {
            _attackRequested = true;
            _attackTarget = target;
        }
        
        #endregion
    }
}