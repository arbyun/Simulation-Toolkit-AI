using System;

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
    }
}