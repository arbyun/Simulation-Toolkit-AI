using System.Numerics;

namespace SimToolAI.Core.Entities
{
    public abstract class Weapon: Entity
    {
        public Entity Owner { get; set; }
        public bool Owned { get; set; } = false;
        
        public Weapon(string name, int x, int y, Entity owner,Simulation simulation) : base(name, x, y, simulation)
        {
            Owner = owner;

            if (Owner != null)
            {
                Owned = true;
            }
        }
        
        public Weapon(string name, int x, int y, Simulation simulation) : base(name, x, y, simulation)
        {
        }

        public abstract void Attack(Vector3 direction);
        
        public abstract void SetOwner(Entity entity);
    }
}