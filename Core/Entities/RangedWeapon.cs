using System.Numerics;

namespace SimArena.Core.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class RangedWeapon: Weapon
    {
        public float FireRate { get; set; }

        /// <summary>
        /// Ranged weapon constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="owned"></param>
        /// <param name="simulation"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public RangedWeapon(string name, int x, int y, bool owned, Simulation simulation, int width, int height) : 
            base(name, x, y, owned, simulation, width, height)
        {
        }

        /// <summary>
        /// Ranged weapon constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="owned"></param>
        /// <param name="simulation"></param>
        public RangedWeapon(string name, int x, int y, bool owned, Simulation simulation) : 
            base(name, x, y, owned, simulation)
        {
        }

        public override bool Attack(Vector3 direction)
        {
            Bullet bullet = new Bullet(X, Y, direction, Simulation, this)
            {
                FacingDirection = direction
            };
            
            Simulation.ProcessNewCreation(bullet);
            return true;
        }
    }
}