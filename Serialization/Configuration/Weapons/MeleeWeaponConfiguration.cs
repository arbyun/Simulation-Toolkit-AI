using SimArena.Core;
using SimArena.Entities;
using SimArena.Entities.Weapons;

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
        
        /// <summary>
        /// Creates a melee weapon from this configuration
        /// </summary>
        /// <param name="x">Weapon's x position</param>
        /// <param name="y">Weapon's y position</param>
        /// <param name="simulation">The simulation</param>
        /// <param name="owner">The agent that owns the weapon</param>
        /// <returns>The created weapon</returns>
        public override Weapon CreateWeapon(int x, int y, Simulation simulation, Agent owner)
        {
            Weapon weapon = new MeleeWeapon(x, y, simulation);

            if (owner != null)
            {
                weapon.Equip(owner);
            }
            
            return weapon;
        }
    }
}