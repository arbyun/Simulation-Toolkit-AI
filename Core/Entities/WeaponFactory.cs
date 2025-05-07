namespace SimArena.Core.Entities
{
    /// <summary>
    /// Factory class for creating common types of weapons.
    /// </summary>
    public static class WeaponFactory
    {
        /// <summary>
        /// Creates a standard pistol weapon.
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="simulation">The simulation instance</param>
        /// <param name="isEquipped">Whether the weapon is initially equipped</param>
        /// <returns>A new pistol weapon</returns>
        public static RangedWeapon CreatePistol(int x, int y, Simulation simulation, bool isEquipped = true)
        {
            return new RangedWeapon("Pistol", x, y, isEquipped, simulation)
            {
                Damage = 15,
                Range = 8,
                FireRate = 0.5f
            };
        }

        /// <summary>
        /// Creates a rifle weapon with higher damage and range than a pistol.
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="simulation">The simulation instance</param>
        /// <param name="isEquipped">Whether the weapon is initially equipped</param>
        /// <returns>A new rifle weapon</returns>
        public static RangedWeapon CreateRifle(int x, int y, Simulation simulation, bool isEquipped = true)
        {
            return new RangedWeapon("Rifle", x, y, isEquipped, simulation)
            {
                Damage = 25,
                Range = 15,
                FireRate = 0.3f
            };
        }

        /// <summary>
        /// Creates a shotgun weapon with high damage but limited range.
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="simulation">The simulation instance</param>
        /// <param name="isEquipped">Whether the weapon is initially equipped</param>
        /// <returns>A new shotgun weapon</returns>
        public static RangedWeapon CreateShotgun(int x, int y, Simulation simulation, bool isEquipped = true)
        {
            return new RangedWeapon("Shotgun", x, y, isEquipped, simulation)
            {
                Damage = 40,
                Range = 5,
                FireRate = 0.8f
            };
        }

        /// <summary>
        /// Creates a sniper rifle weapon with very high damage and range.
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="simulation">The simulation instance</param>
        /// <param name="isEquipped">Whether the weapon is initially equipped</param>
        /// <returns>A new sniper rifle weapon</returns>
        public static RangedWeapon CreateSniperRifle(int x, int y, Simulation simulation, bool isEquipped = true)
        {
            return new RangedWeapon("Sniper Rifle", x, y, isEquipped, simulation)
            {
                Damage = 50,
                Range = 25,
                FireRate = 1.5f
            };
        }

        /// <summary>
        /// Creates a melee knife weapon.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public static MeleeWeapon CreateKnife(int startX, int startY, Simulation simulation)
        {
            return new MeleeWeapon("Knife", startX, startY, true, simulation)
            {
                Damage = 10
            };
        }
    }
}