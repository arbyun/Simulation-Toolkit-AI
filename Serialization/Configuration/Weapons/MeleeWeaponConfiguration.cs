using System;

namespace SimArena.Serialization.Configuration.Weapons
{
    /// <summary>
    /// Configuration for melee weapons
    /// </summary>
    [Serializable]
    public class MeleeWeaponConfiguration : WeaponConfiguration
    {
        /// <summary>
        /// Attack speed multiplier
        /// </summary>
        public float AttackSpeed { get; set; } = 1.0f;
        
        /// <summary>
        /// Whether this weapon can hit multiple targets
        /// </summary>
        public bool AreaEffect { get; set; } = false;
        
        public MeleeWeaponConfiguration(string weaponId, int damage, float attackSpeed = 1.0f, bool areaEffect = false) 
            : base(weaponId, "Melee", damage)
        {
            AttackSpeed = attackSpeed;
            AreaEffect = areaEffect;
        }
        
        /// <summary>
        /// Parameter-less constructor for JSON deserialization
        /// </summary>
        public MeleeWeaponConfiguration() : base("", "Melee", 0) { }
    }
}