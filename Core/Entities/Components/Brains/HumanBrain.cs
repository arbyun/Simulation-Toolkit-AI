using System.Numerics;

namespace SimArena.Core.Entities.Components
{
    public class HumanBrain: Brain
    {
        #region Properties
        
        /// <summary>
        /// The last movement direction input by the human player
        /// </summary>
        private Vector3? _lastMovementInput;
        
        /// <summary>
        /// The target of the attack, if any
        /// </summary>
        private Entity? _attackTarget;
        
        #endregion
        
        #region Constructors

        /// <summary>
        /// Creates a new human brain with the specified parameters
        /// </summary>
        /// <param name="owner">The entity this brain controls</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="simulation">Simulation instance</param>
        public HumanBrain(Character owner, int awareness, Simulation simulation) : base(owner, awareness, simulation)
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
        /// Sets the movement input from the human player
        /// </summary>
        /// <param name="direction">Direction to move</param>
        public bool SetMovementInput(Vector3 direction)
        {
            _lastMovementInput = direction;
            return Move(_lastMovementInput.Value);
        }

        /// <summary>
        /// Sets the attack input from the human player
        /// </summary>
        /// <param name="target">The attack direction</param>
        /// <param name="targetEntity">Target entity to attack</param>
        public bool SetAttackInput(Vector3 target, Entity? targetEntity)
        {
            _attackTarget = targetEntity;
            return Attack(target);
        }
        
        #endregion
    }
}