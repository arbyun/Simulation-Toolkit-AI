using System.Numerics;
using SimToolAI.Core.AI;

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
        /// <param name="simulation">The simulation instance</param>
        /// <param name="humanControlled">If true, creates a human brain; otherwise an AI brain</param>
        public Player(string name, int x, int y, int awareness, Simulation simulation, 
            bool humanControlled = false) : base(name, x, y, simulation)
        {
            if (humanControlled)
            {
                Brain = new HumanBrain(this, awareness, simulation);
            }
            else
            {
                Brain = new AIBrain(this, awareness, simulation);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes input for a human-controlled player
        /// </summary>
        /// <param name="direction">Direction to move, or null for no movement</param>
        /// <param name="attack">Whether to attack</param>
        /// <param name="target">Target to attack, or null for default direction</param>
        public bool ProcessInput(Vector3? direction, bool attack, Entity target = null)
        {
            if (!IsHumanControlled)
                return false;
                
            if (direction.HasValue)
            {
                return HumanBrain.SetMovementInput(direction.Value);
            }
            
            if (attack)
            {
                return HumanBrain.SetAttackInput(FacingDirection, target);
            }

            return false;
        }

        #endregion
    }
}