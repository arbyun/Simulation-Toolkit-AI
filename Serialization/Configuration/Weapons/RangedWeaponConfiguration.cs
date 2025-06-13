using System;
using SimArena.Core;
using SimArena.Entities;
using SimArena.Entities.Weapons;

namespace SimArena.Serialization.Configuration.Weapons
{
    /// <summary>
    /// Configuration for ranged weapons
    /// </summary>
    [Serializable]
    public class RangedWeaponConfiguration : WeaponConfiguration
    {
        /// <summary>
        /// Maximum range of the weapon
        /// </summary>
        public float Range { get; set; } = 5.0f;
        
        /// <summary>
        /// Projectile speed
        /// </summary>
        public float ProjectileSpeed { get; set; } = 10.0f;
        
        /// <summary>
        /// Rate of fire (shots per second)
        /// </summary>
        public float FireRate { get; set; } = 1.0f;
        
        public RangedWeaponConfiguration(string weaponId, int damage, float range = 5.0f, float projectileSpeed = 10.0f, float fireRate = 1.0f) 
            : base(weaponId, "Ranged", damage)
        {
            Range = range;
            ProjectileSpeed = projectileSpeed;
            FireRate = fireRate;
        }
        
        /// <summary>
        /// Parameter-less constructor for JSON deserialization
        /// </summary>
        public RangedWeaponConfiguration() : base("", "Ranged", 0) { }
        
        /// <summary>
        /// Creates a ranged weapon from this configuration
        /// </summary>
        /// <param name="x">Weapon's x position</param>
        /// <param name="y">Weapon's y position</param>
        /// <param name="simulation">The simulation</param>
        /// <param name="owner">The agent that owns the weapon</param>
        /// <returns>The created weapon</returns>
        public override Weapon CreateWeapon(int x, int y, Simulation simulation, Agent owner)
        {
            Weapon weapon = new RangedWeapon(x, y, simulation, Range, ProjectileSpeed, FireRate);

            if (owner != null)
            {
                weapon.Equip(owner);
            }
            
            return weapon;
        }
    }
}