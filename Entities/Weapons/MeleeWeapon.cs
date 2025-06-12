using System.Linq;
using System.Numerics;
using SimArena.Core;

namespace SimArena.Entities.Weapons
{
    public class MeleeWeapon: Weapon
    {
        public MeleeWeapon(int x, int y, Simulation simulation) : base(x, y, simulation)
        {
        }

        public override bool Attack(Vector3 direction)
        {
            bool validPosition = Simulation.Map.IsWalkable(X + (int)direction.X, Y + (int)direction.Y);

            if (!validPosition)
            {
                // find what's blocking this and try to get it as an IDamageable
                // for now we'll do something a bit hacky to test this class out
                IDamageable damageable = Simulation.Agents.FirstOrDefault(a => a.X == X + (int)direction.X && a.Y == Y + (int)direction.Y) as IDamageable;
            
                if (damageable != null)
                {
                    damageable.TakeDamage(Damage, Owner);
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}