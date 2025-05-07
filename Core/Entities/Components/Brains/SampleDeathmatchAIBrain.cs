using System.Numerics;
using SimArena.Core.SimulationElements.Map;
using SimArena.Core.Utilities;

namespace SimArena.Core.Entities.Components;

public class SampleDeathmatchAIBrain : Brain
 {
    private Vector3 _movementDirection = Vector3.Zero;
    private Entity? _targetEntity = null;
    private float _thinkTimer = 0;
    private const float THINK_INTERVAL = 0.5f; // Think every 0.5 seconds
    private Random _random = new Random();

     public SampleDeathmatchAIBrain(Character owner, int awareness, Simulation simulation) 
         : base(owner, awareness, simulation)
     {
     }
 
     public override void Think(float deltaTime)
     {
         if (!Owner.IsAlive)
         {
             return;
         }
         
        // Update think timer
        _thinkTimer += deltaTime;
        
        // Only think at certain intervals to improve performance
        if (_thinkTimer < THINK_INTERVAL)
            return;
        
        _thinkTimer = 0;
        MakeDecision();
    }
     
     private void MakeDecision()
     {
        // Find the nearest enemy
        Character? nearestEnemy = FindNearestEnemy();
        
        if (nearestEnemy != null)
        {
            // We found an enemy, move towards them and set them as our target
            _targetEntity = nearestEnemy;
            
            // Calculate direction to move towards the enemy
            int dx = nearestEnemy.X - Owner.X;
            int dy = nearestEnemy.Y - Owner.Y;
            
            // Normalize the direction
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            
            if (length > 0)
            {
                Simulation.Map.SetWalkable(Owner.X, Owner.Y, true);
                Simulation.Map.SetWalkable(nearestEnemy.X, nearestEnemy.Y, true);
 
                PathFinder pathFinder = new PathFinder(Simulation.Map);
                MapPath path = pathFinder.ShortestPath( 
                    Simulation.Map.GetCell(Owner.X, Owner.Y), 
                    Simulation.Map.GetCell(nearestEnemy.X, nearestEnemy.Y));
            
                Simulation.Map.SetWalkable(Owner.X, Owner.Y, false);
                Simulation.Map.SetWalkable(nearestEnemy.X, nearestEnemy.Y, false);
                
                if (path == null)
                {
                    Console.WriteLine($"{Owner.Name} is stuck.");
                    return;
                }

                _movementDirection = new Vector3(path.Steps.First().X, path.Steps.First().Y, 0);
            }
                
            // If we're close enough to attack, face the enemy but don't move
            if (length <= Owner.MainWeapon.Range)
            {
                Owner.FacingDirection = _movementDirection;
                Owner.Attack(_movementDirection);   
                Console.WriteLine($"Attacking {_targetEntity.Name}.");
                _movementDirection = Vector3.Zero;
            }
        }
        else
        {
            // No enemy found, move randomly
            _targetEntity = null;
            
            // Generate a random direction occasionally
            if (_random.Next(0, 5) == 0 || _movementDirection == Vector3.Zero)
            {
                (int x, int y)? tuple = Simulation.Map.GetRandomWalkableLocation();
                
                PathFinder pathFinder = new PathFinder(Simulation.Map);
                MapPath path = pathFinder.ShortestPath( 
                    Simulation.Map.GetCell(Owner.X, Owner.Y), 
                    Simulation.Map.GetCell(tuple.Value.x, tuple.Value.y));

                if (path == null)
                {
                    Console.WriteLine($"{Owner.Name} is stuck.");
                    return;
                }
                
                _movementDirection = new Vector3(path.Steps.First().X, path.Steps.First().Y, 0);
            }
        }

        if (_movementDirection != Vector3.Zero)
        {
            Move(_movementDirection);
        }
     }

    private Character? FindNearestEnemy()
    {
        Character? nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        
        FieldOfView characterFov = new FieldOfView(Simulation.Map);
        
        // Get all characters in the simulation
        var characters = Simulation
            .GetEntities()
            .OfType<Character>()
            .Where(c => c.IsAlive && !c.Equals(Owner));
        
        foreach (var character in characters)
        {
            characterFov.ComputeFov(Owner.X, Owner.Y, Awareness, true);
            
            // Check if the character is in our field of view
            if (characterFov.IsInFov(character.X, character.Y))
            {
                // Check if we have line of sight to the character
                if (Simulation.Map.IsInLineOfSight(Owner.X, Owner.Y, character.X, character.Y))
                {
                    // Calculate distance to the character
                    float distance = Simulation.Map.GetDistance(Owner.X, Owner.Y, character.X, character.Y);
                    
                    // If this character is closer than the current nearest, update the nearest
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = character;
                    }
                }
            }
        }
        
        return nearestEnemy;
     }
 
     public override Vector3 GetMovementDirection()
     {
        return _movementDirection;
     }
 
     public override Entity? GetInteractionTarget()
     { 
         return _targetEntity;
     }
 }