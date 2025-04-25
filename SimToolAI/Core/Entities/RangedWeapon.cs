using System.Numerics;

namespace SimToolAI.Core.Entities
{
    public class RangedWeapon: Weapon
    {
        public RangedWeapon(string name, int x, int y, Entity owner, Simulation simulation) : base(name, x, y, 
            owner, simulation)
        {
        }
        
        public RangedWeapon(string name, int x, int y,Simulation simulation) : base(name, x, y, simulation)
        {
        }

        public override void Attack(Vector3 direction)
        {
            // Create a new bullet
            var bullet = new Bullet(Owner.X, Owner.Y, direction, Simulation, this)
            {
                FacingDirection = direction
            };
            
            Simulation.ProcessNewCreation(bullet);
        }

        public override void SetOwner(Entity entity)
        {
            Owner = entity;
        }
    }
}