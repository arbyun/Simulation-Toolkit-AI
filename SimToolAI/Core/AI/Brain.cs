using System;
using System.Linq;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;
using SimToolAI.Utilities;

namespace SimToolAI.Core.AI
{
    /// <summary>
    /// Base class for all brain implementations that control entity decision making
    /// </summary>
    public abstract class Brain
    {
        #region Properties
        
        /// <summary>
        /// The entity this brain controls
        /// </summary>
        protected Character Owner { get; }
        
        /// <summary>
        /// Awareness radius of the brain (for perception calculations)
        /// </summary>
        public int Awareness { get; }
        
        /// <summary>
        /// Reference to the scene
        /// </summary>
        protected Scene Scene { get; }
        
        /// <summary>
        /// Reference to the map
        /// </summary>
        protected ISimMap Map { get; }
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Creates a new brain with the specified parameters
        /// </summary>
        /// <param name="owner">The entity this brain controls</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="scene">Reference to the scene</param>
        protected Brain(Character owner, int awareness, Scene scene)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Awareness = awareness;
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));
            Map = scene.Map;
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Updates the brain state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public abstract void Think(float deltaTime);
        
        /// <summary>
        /// Decides whether to move and in which direction
        /// </summary>
        /// <returns>Direction to move, or null if no movement is desired</returns>
        public abstract Direction? DecideMovement();
        
        /// <summary>
        /// Decides whether to attack and which target
        /// </summary>
        /// <returns>Target to attack, or null if no attack is desired</returns>
        public abstract Entity DecideAttackTarget();
        
        /// <summary>
        /// Perceives entities within awareness radius
        /// </summary>
        /// <returns>Array of entities within awareness radius</returns>
        protected Entity[] PerceiveEntities()
        {
            // Get all entities in the scene
            var entities = Scene.GetEntities<Entity>();
            
            // Filter entities by distance
            return entities.Where(e => 
                e != Owner && 
                Owner.DistanceTo(e) <= Awareness && 
                Map.IsInLineOfSight(Owner.X, Owner.Y, e.X, e.Y)
            ).ToArray();
        }
        
        #endregion
    }
}