using System.Numerics;
using SimArena.Core.Entities.Components;

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
        public float Range { get; set; }
        public int Damage { get; set; }

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
            int width, int height, float range = 1.5f) : base(name, x, y, simulation, width, height)
        {
            Owned = owned;
            Range = range;
        }

        /// <summary>
        /// Creates a weapon entity.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="owned"></param>
        /// <param name="simulation"></param>
        protected Weapon(string name, int x, int y, bool owned, Simulation simulation, float range = 1.5f) 
            : base(name, x, y, simulation)
        {
            Owned = owned;
            Range = range;
        }

        /// <summary>
        /// Declares an attack on the given direction.
        /// </summary>
        /// <param name="direction">The direction of the attack.</param>
        public abstract bool Attack(Vector3 direction);
    }
}