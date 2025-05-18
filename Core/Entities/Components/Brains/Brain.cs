using System;
using System.Linq;
using System.Numerics;

namespace SimArena.Core.Entities.Components
{
    public abstract class Brain
    {
        #region Properties
        
        /// <summary>
        /// The entity this brain controls
        /// </summary>
        public Character Owner { get; }
        
        /// <summary>
        /// Awareness radius of the brain (for perception calculations)
        /// </summary>
        public int Awareness { get; }
        
        /// <summary>
        /// Reference to the simulation
        /// </summary>
        public Simulation Simulation { get; }
        
        #endregion
        
        #region Constructors

        /// <summary>
        /// Creates a new brain with the specified parameters
        /// </summary>
        /// <param name="owner">The entity this brain controls</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="simulation">Reference to the simulation</param>
        protected Brain(Character owner, int awareness, Simulation simulation)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Awareness = awareness;
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Updates the brain state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public abstract void Think(float deltaTime);

        protected bool Move(Vector3 direction)
        {
            return Simulation.ProcessMovement(Owner, direction);
        }
        
        protected bool Attack(Vector3 target)
        {
            return Owner.Attack(target);
        }
        
        /// <summary>
        /// Perceives entities within awareness radius
        /// </summary>
        /// <returns>Array of entities within awareness radius</returns>
        protected Entity[] PerceiveEntities()
        {
            // Get all entities in the scene
            var entities = Simulation.GetEntities();
            
            // Filter entities by distance
            return entities.Where(e => 
                !e.Equals(Owner) && 
                Owner.DistanceTo(e) <= Awareness && 
                Simulation.Map.IsInLineOfSight(Owner.X, Owner.Y, e.X, e.Y)
            ).ToArray();
        }
        
        #endregion
    }
}