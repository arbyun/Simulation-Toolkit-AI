using System.Numerics;
using SimArena.Core;

namespace SimArena.Entities.Weapons
{
    public class RangedWeapon: Weapon
    {
        /// <summary>
        /// The range of this weapon
        /// </summary>
        public float Range { get; private set; }
    
        /// <summary>
        /// The speed of the projectile
        /// </summary>
        public float ProjectileSpeed { get; private set; }
    
        /// <summary>
        /// The fire rate of this weapon
        /// </summary>
        public float FireRate { get; private set; }
    
        public RangedWeapon(int x, int y, Simulation simulation, float range, float projectileSpeed, float fireRate) : base(x, y, simulation)
        {
            Range = range;
            ProjectileSpeed = projectileSpeed;
            FireRate = fireRate;
        }

        public override bool Attack(Vector3 direction)
        {
            Bullet bullet = new Bullet(X, Y, direction, Simulation, this);
            Simulation.Events.RaiseOnCreate(this, bullet);
            return true;
        }
    }
}