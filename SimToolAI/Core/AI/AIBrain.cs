using System;
using System.Linq;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Rendering;
using SimToolAI.Utilities;

namespace SimToolAI.Core.AI
{
    /// <summary>
    /// Base class for AI-controlled entity brains
    /// </summary>
    public class AIBrain : Brain
    {
        #region Properties
        
        /// <summary>
        /// Time since the last decision
        /// </summary>
        private float _timeSinceLastDecision;
        
        /// <summary>
        /// How often the AI makes decisions (in seconds)
        /// </summary>
        public float DecisionInterval { get; set; } = 1.0f;
        
        /// <summary>
        /// Current movement direction
        /// </summary>
        private Direction? _currentMovementDirection;
        
        /// <summary>
        /// Current attack target
        /// </summary>
        private Entity _currentAttackTarget;
        
        /// <summary>
        /// Random number generator for AI decisions
        /// </summary>
        private readonly Random _random = new Random();
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Creates a new AI brain with the specified parameters
        /// </summary>
        /// <param name="owner">The entity this brain controls</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="scene">Reference to the scene</param>
        public AIBrain(Character owner, int awareness, Scene scene) 
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
            _timeSinceLastDecision += deltaTime;
            
            // Make new decisions at the specified interval
            if (_timeSinceLastDecision >= DecisionInterval)
            {
                _timeSinceLastDecision = 0;
                MakeDecisions();
            }
        }
        
        /// <summary>
        /// Makes decisions about movement and attacks
        /// </summary>
        protected virtual void MakeDecisions()
        {
            // Perceive entities in the environment
            Entity[] perceivedEntities = PerceiveEntities();
            
            // Find the nearest player or character
            Entity nearestTarget = perceivedEntities
                .Where(e => e is Player || e is Character)
                .OrderBy(e => Owner.DistanceTo(e))
                .FirstOrDefault();
            
            if (nearestTarget != null)
            {
                // If a target is found, move towards it and attack if close enough
                _currentAttackTarget = Owner.DistanceTo(nearestTarget) <= 1 ? nearestTarget : null;
                
                if (_currentAttackTarget == null)
                {
                    // Move towards the target
                    int dx = nearestTarget.X - Owner.X;
                    int dy = nearestTarget.Y - Owner.Y;
                    
                    if (Math.Abs(dx) > Math.Abs(dy))
                    {
                        _currentMovementDirection = dx > 0 ? Direction.Right : Direction.Left;
                    }
                    else
                    {
                        _currentMovementDirection = dy > 0 ? Direction.Down : Direction.Up;
                    }
                }
                else
                {
                    // Don't move if attacking
                    _currentMovementDirection = null;
                }
            }
            else
            {
                // If no target is found, move randomly
                _currentAttackTarget = null;
                _currentMovementDirection = (Direction)_random.Next(4); // Random direction
            }
        }
        
        /// <summary>
        /// Decides whether to move and in which direction
        /// </summary>
        /// <returns>Direction to move, or null if no movement is desired</returns>
        public override Direction? DecideMovement()
        {
            return _currentMovementDirection;
        }
        
        /// <summary>
        /// Decides whether to attack and which target
        /// </summary>
        /// <returns>Target to attack, or null if no attack is desired</returns>
        public override Entity DecideAttackTarget()
        {
            return _currentAttackTarget;
        }
        
        #endregion
    }
}