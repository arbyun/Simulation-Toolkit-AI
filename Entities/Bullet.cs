using System.Numerics;
using SimArena.Core;
using SimArena.Entities.Weapons;

namespace SimArena.Entities;

public class Bullet: Entity
{
    /// <summary>
    /// Gets the direction the bullet is moving
    /// </summary>
    public Vector3 Direction { get; private set; }
    
    /// <summary>
    /// Gets the simulation the bullet is in
    /// </summary>
    public Simulation Simulation { get; private set; }
    
    /// <summary>
    /// Gets the weapon that fired this bullet
    /// </summary>
    private Weapon Owner { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum distance the bullet can travel
    /// </summary>
    public int MaxRange { get; set; } = 50;

    /// <summary>
    /// Gets or sets the distance the bullet has traveled
    /// </summary>
    public int DistanceTraveled { get; private set; }

    /// <summary>
    /// Gets whether the bullet has reached its maximum range
    /// </summary>
    public bool ReachedMaxRange => DistanceTraveled >= MaxRange;
    
    public Bullet(int x, int y, Vector3 direction, Simulation simulation, Weapon owner) : base(x, y)
    {
        Direction = direction;
        Simulation = simulation;
        Owner = owner;
        
        Simulation.Events.StepCompleted += OnStepCompleted;
        
        // Let's position the bullet at the tip of the weapon
        X += (int)direction.X;
        Y += (int)direction.Y;
    }

    private void OnStepCompleted(object? sender, int step)
    {
        if (ReachedMaxRange)
        {
            Simulation.Events.RaiseOnDestroy(this, this);
            return;
        }
        
        Move();
    }

    private void Move()
    {
        // Calculate new position based on direction
        int newX = X;
        int newY = Y;

        newX += (int)Direction.X;
        newY += (int)Direction.Y;

        // Check if the bullet has reached its maximum range
        if (++DistanceTraveled > MaxRange)
        {
            // Bullet reached maximum range, remove it
            Simulation.Events.RaiseOnDestroy(this, this);
            return;
        }

        bool validPos = Simulation.Map.IsWalkable(newX, newY);
            
        // Check if the new position is walkable
        if (!validPos)
        {
            // Bullet hit something
            
            // Check if there's an entity at the new position
            Entity? entity = Simulation.Agents.FirstOrDefault(a => a.X == newX && a.Y == newY);
            
            if (entity != null && !entity.Equals(this) && !entity.Equals(Owner.Owner))
            {
                // Bullet hit an entity
                HandleEntityCollision(entity);
                return;
            }
            
            Simulation.Events.RaiseOnDestroy(this, this);
            return;
        }
        
            
        // Move the bullet
        X = newX;
        Y = newY;

        Simulation.Events.RaiseOnMove(this, this);
    }
    
    /// <summary>
    /// Handles collision with an entity
    /// </summary>
    /// <param name="entity">Entity the bullet collided with</param>
    private void HandleEntityCollision(Entity entity)
    {
        // If the entity is a player, damage it
        if (entity is IDamageable player)
        {
            player.TakeDamage(Owner.Damage, Owner.Owner);
        }
    }
}