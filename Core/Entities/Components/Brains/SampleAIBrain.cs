using System;
using System.Linq;
using System.Numerics;
using SimArena.Core.Utilities;

namespace SimArena.Core.Entities.Components
{
    public class SampleAiBrain: Brain
    {
        #region Properties
        
        /// <summary>
        /// Time since the last decision
        /// </summary>
        private float _timeSinceLastDecision;
        
        /// <summary>
        /// How often the AI makes decisions (in seconds)
        /// </summary>
        public float DecisionInterval { get; } = 1.0f;
        
        /// <summary>
        /// Current movement direction
        /// </summary>
        private Vector3? _currentMovementDirection;
        
        /// <summary>
        /// Current attack target
        /// </summary>
        private Entity? _currentAttackTarget;
        
        /// <summary>
        /// Random number generator for AI decisions
        /// </summary>
        private readonly Random _random = new();
        
        #endregion
        
        #region Constructors

        /// <summary>
        /// Creates a new AI brain with the specified parameters
        /// </summary>
        /// <param name="owner">The entity this brain controls</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="simulation">The simulation instance</param>
        public SampleAiBrain(Character owner, int awareness, Simulation simulation): base(owner, awareness, simulation)
        {
        }

        /// <summary>
        /// Creates a new AI brain with the specified parameters
        /// </summary>
        /// <param name="owner">The entity this brain controls</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="decisionInterval">Time between decisions in seconds</param>
        /// <param name="simulation">The simulation instance</param>
        public SampleAiBrain(Character owner, int awareness, float decisionInterval, Simulation simulation) : 
            base(owner, awareness, simulation)
        {
            DecisionInterval = decisionInterval;
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
            
            // Always try to move in the current direction, even between decisions
            if (_currentMovementDirection != null)
            {
                Move(_currentMovementDirection.Value);
            } 
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Vector3 GetMovementDirection()
        {
            return _currentMovementDirection ?? Vector3.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Entity? GetInteractionTarget()
        {
            return _currentAttackTarget;
        }

        /// <summary>
        /// Makes decisions about movement and attacks
        /// </summary>
        protected virtual void MakeDecisions()
        {
            // Perceive entities in the environment
            Entity[] perceivedEntities = PerceiveEntities();
            
            // Find the nearest player or character
            Entity? nearestTarget = perceivedEntities
                .Where(e => e is Character)
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
                        _currentMovementDirection = dx > 0 ? DirectionVector.Right : DirectionVector.Left;
                    }
                    else
                    {
                        _currentMovementDirection = dy > 0 ? DirectionVector.Down : DirectionVector.Up;
                    }
                }
                else
                {
                    // Don't move if attacking
                    Owner.Attack(new Vector3(nearestTarget.X, nearestTarget.Y, 0));
                    _currentMovementDirection = null;
                }
            }
            else
            {
                // If no target is found, try to find a valid random direction to move
                _currentAttackTarget = null;
                
                // Try all cardinal directions in random order
                var directions = new[] { DirectionVector.Up, DirectionVector.Right, DirectionVector.Down, DirectionVector.Left };
                directions = directions.OrderBy(x => _random.Next()).ToArray();
                
                foreach (var direction in directions)
                {
                    int newX = Owner.X + (int)direction.X;
                    int newY = Owner.Y + (int)direction.Y;
                    
                    if (Simulation.Map.IsInBounds(newX, newY) && Simulation.Map.IsWalkable(newX, newY))
                    {
                        _currentMovementDirection = direction;
                        break;
                    }
                }
                
                // If no valid direction was found, try to find any walkable cell nearby
                if (_currentMovementDirection == null)
                {
                    for (int radius = 1; radius <= 3; radius++)
                    {
                        for (int x = -radius; x <= radius; x++)
                        {
                            for (int y = -radius; y <= radius; y++)
                            {
                                if (x == 0 && y == 0) continue;
                                
                                int newX = Owner.X + x;
                                int newY = Owner.Y + y;
                                
                                if (Simulation.Map.IsInBounds(newX, newY) && Simulation.Map.IsWalkable(newX, newY))
                                {
                                    // Found a walkable cell, set direction towards it
                                    _currentMovementDirection = new Vector3(
                                        Math.Sign(x),
                                        Math.Sign(y),
                                        0
                                    );
                                    break;
                                }
                            }
                            if (_currentMovementDirection != null) break;
                        }
                        if (_currentMovementDirection != null) break;
                    }
                }
            }
        }
        
        #endregion
    }
}