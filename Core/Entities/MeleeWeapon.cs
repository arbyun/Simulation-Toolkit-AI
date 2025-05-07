using System.Numerics;
using SimArena.Core.Entities.Components.Collision;

namespace SimArena.Core.Entities;

public class MeleeWeapon: Weapon
{
    public MeleeWeapon(string name, int x, int y, bool owned, Simulation simulation, 
        ICollider? collider, float range = 1.5f) : base(name, x, y, owned, simulation, collider, range)
    {
    }

    public MeleeWeapon(string name, int x, int y, bool owned, Simulation simulation, 
        int width, int height, float range = 1.5f) : base(name, x, y, owned, simulation, width, height, range)
    {
    }

    public MeleeWeapon(string name, int x, int y, bool owned, Simulation simulation, 
        float range = 1.5f) : base(name, x, y, owned, simulation, range)
    {
    }

    public override bool Attack(Vector3 direction)
    {
        bool validPosition = Collider != null && Collider.IsValidPosition(Simulation.Map, Position.X + 
            (int)direction.X, Position.Y + (int)direction.Y);

        if (!validPosition)
        {
            Entity? entity = Simulation.GetEntityAt(Position.X + (int)direction.X, Position.Y + (int)direction.Y);
            Console.WriteLine($"Weapon {Name} collides with {entity?.Name}.");
            
            if (entity is Character character)
            {
                character.TakeDamage(Damage);
                return true;
            }

            return false;
        }

        return false;
    }
}