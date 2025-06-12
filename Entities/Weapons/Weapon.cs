using System;
using System.Numerics;
using SimArena.Core;

namespace SimArena.Entities.Weapons
{
    public abstract class Weapon: Entity
    {
        /// <summary>
        /// The simulation this weapon is in
        /// </summary>
        protected Simulation Simulation { get; }
    
        /// <summary>
        /// The agent that owns this weapon
        /// </summary>
        public Agent Owner { get; set; }
    
        /// <summary>
        /// Whether this weapon is owned by an agent
        /// </summary>
        public bool Owned { get; private set; }
    
        /// <summary>
        /// Whether this weapon is currently equipped by its owner
        /// </summary>
        public bool IsEquipped { get; set; }
    
        /// <summary>
        /// The damage this weapon does
        /// </summary>
        public int Damage { get; set; }
    
        protected Weapon(int x, int y, Simulation simulation) : base(x, y)
        {
            Simulation = simulation;
            Owned = false;
        }
    
        protected Weapon(int x, int y, Simulation simulation, Agent owner) : base(x, y)
        {
            Simulation = simulation;
            Owner = owner;
            Owned = true;
        }
    
        /// <summary>
        /// Equips this weapon to the given agent.
        /// </summary>
        /// <param name="agent">The agent to equip the weapon to.</param>
        public void Equip(Agent agent)
        {
            if (Owned)
            {
                throw new Exception("Weapon is already owned by an agent.");
            }
        
            Owner = agent;
            Owned = true;
            IsEquipped = true;
        }
    
        /// <summary>
        /// Declares an attack on the given direction.
        /// </summary>
        /// <param name="direction">The direction of the attack.</param>
        public abstract bool Attack(Vector3 direction);
    }
}