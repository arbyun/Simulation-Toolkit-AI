using System.Text.Json.Serialization;
using SimArena.Core;
using SimArena.Entities;
using SimArena.Entities.Weapons;
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
        
        /// <summary>
        /// Factory method to create an actual weapon instance
        /// </summary>
        public virtual Weapon CreateWeapon(int x, int y, Simulation simulation, Agent owner)
        {
            throw new NotImplementedException("This method must be overridden in derived classes.");
        }
    }
}