using System;

namespace SimArena.Serialization.Configuration.Weapons
{
    /// <summary>
    /// Configuration for melee weapons
    /// </summary>
    [Serializable]
    public class MeleeWeaponConfiguration : WeaponConfiguration
    {
        public MeleeWeaponConfiguration(string weaponId, int damage) 
            : base(weaponId, "Melee", damage)
        {
        }
        
        /// <summary>
        /// Parameter-less constructor for JSON deserialization
        /// </summary>
        public MeleeWeaponConfiguration() : base("", "Melee", 0) { }
    }
}