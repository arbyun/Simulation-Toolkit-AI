using System.Numerics;

namespace SimToolAI.Core.Entities
{
    public abstract class Weapon: Entity
    {
        public bool Owned { get; set; } = false;
        
        public Weapon(string name, int x, int y, Simulation simulation) : base(name, x, y, simulation)
        {
        }

        public abstract void Attack(Vector3 direction);
    }
}