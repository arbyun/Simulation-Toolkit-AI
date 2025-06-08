using System;
using System.Text.Json.Serialization;
using SimArena.Serialization.Configuration.Weapons;

namespace SimArena.Serialization.Configuration
{
    [Serializable]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(MeleeWeaponConfiguration), "Melee")]
    [JsonDerivedType(typeof(RangedWeaponConfiguration), "Ranged")]
    public class WeaponConfiguration
    {
        /// <summary>
        /// Unique identifier for the weapon
        /// </summary>
        public string WeaponId { get; set; }
    
        /// <summary>
        /// Type of weapon (Melee or Ranged)
        /// </summary>
        public string WeaponType { get; set; }
    
        /// <summary>
        /// Weapon's damage value
        /// </summary>
        public int Damage { get; set; }
        
        /// <summary>
        /// Base constructor
        /// </summary>
        public WeaponConfiguration(string weaponId, string weaponType, int damage)
        {
            WeaponId = weaponId;
            WeaponType = weaponType;
            Damage = damage;
        }
        
        /// <summary>
        /// Parameter-less constructor for JSON deserialization
        /// </summary>
        public WeaponConfiguration() { }
    }
}