using System.Numerics;
using SimArena.Core.Entities.Components;
using SimArena.Core.Entities.Components.Collision;

namespace SimArena.Core.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Weapon: Entity
    {
        public Entity Owner { get; set; }
        public bool Owned { get; }
        public bool IsEquipped { get; set; }

        /// <summary>
        /// Creates a weapon entity.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="owned"></param>
        /// <param name="simulation"></param>
        /// <param name="collider"></param>
        public Weapon(string name, int x, int y, bool owned, Simulation simulation, ICollider? collider) : 
            base(name, x, y, simulation, collider)
        {
            Owned = owned;
        }

        /// <summary>
        /// Creates a weapon entity.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="owned"></param>
        /// <param name="simulation"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected Weapon(string name, int x, int y, bool owned, Simulation simulation, 
            int width, int height) : base(name, x, y, simulation, width, height)
        {
            Owned = owned;
        }

        /// <summary>
        /// Creates a weapon entity.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="owned"></param>
        /// <param name="simulation"></param>
        protected Weapon(string name, int x, int y, bool owned, Simulation simulation) 
            : base(name, x, y, simulation)
        {
            Owned = owned;
        }

        /// <summary>
        /// Declares an attack on the given direction.
        /// </summary>
        /// <param name="direction">The direction of the attack.</param>
        public abstract bool Attack(Vector3 direction);
    }
}