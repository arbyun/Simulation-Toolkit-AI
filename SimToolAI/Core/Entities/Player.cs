using System;
using SimToolAI.Core.AI;
using SimToolAI.Core.Rendering;
using SimToolAI.Utilities;

namespace SimToolAI.Core.Entities
{
    /// <summary>
    /// Represents a player-controlled entity in the simulation
    /// </summary>
    public class Player : Character
    {
        #region Properties

        /// <summary>
        /// Gets the human brain controlling this player, if any
        /// </summary>
        public HumanBrain HumanBrain => Brain as HumanBrain;
        
        /// <summary>
        /// Whether this player is human-controlled
        /// </summary>
        public bool IsHumanControlled => HumanBrain != null;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new player with the specified parameters and a human brain
        /// </summary>
        /// <param name="name">Name of the player</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="awareness">Awareness radius</param>
        /// <param name="scene">Scene reference</param>
        /// <param name="humanControlled">If true, creates a human brain; otherwise an AI brain</param>
        public Player(string name, int x, int y, int awareness, Scene scene, bool humanControlled = false) 
            : base(name, x, y, humanControlled ? 
                  (Brain)new HumanBrain(null, awareness, scene) : 
                  (Brain)new AIBrain(null, awareness, scene), scene)
        {
            // Fix circular reference in brain constructor
            if (Brain is HumanBrain humanBrain)
            {
                typeof(Brain).GetField("_owner", System.Reflection.BindingFlags.NonPublic | 
                                                 System.Reflection.BindingFlags.Instance)?.SetValue(humanBrain, this);
            }
            else if (Brain is AIBrain aiBrain)
            {
                typeof(Brain).GetField("_owner", System.Reflection.BindingFlags.NonPublic | 
                                                 System.Reflection.BindingFlags.Instance)?.SetValue(aiBrain, this);
            }
        }

        /// <summary>
        /// Creates a new player with the specified parameters and default awareness
        /// </summary>
        /// <param name="name">Name of the player</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="scene">Scene reference</param>
        public Player(string name, int x, int y, Scene scene) 
            : this(name, x, y, 10, scene, false)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes input for a human-controlled player
        /// </summary>
        /// <param name="direction">Direction to move, or null for no movement</param>
        /// <param name="attack">Whether to attack</param>
        /// <param name="target">Target to attack, or null for default direction</param>
        public void ProcessInput(Direction? direction, bool attack, Entity target = null)
        {
            if (!IsHumanControlled)
                return;
                
            if (direction.HasValue)
            {
                HumanBrain.SetMovementInput(direction.Value);
            }
            
            if (attack)
            {
                HumanBrain.SetAttackInput(target);
            }
        }

        #endregion
    }
}