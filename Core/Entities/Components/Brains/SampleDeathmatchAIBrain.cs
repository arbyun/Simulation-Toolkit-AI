using System.Numerics;
using RogueSharp;
using SimArena.Core.SimulationElements.Map;

namespace SimArena.Core.Entities.Components
{
    public class SampleDeathmatchAIBrain : Brain
    {
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
 
                    PathFinder pathFinder = new PathFinder(Simulation.Map.Map);
                    RogueSharp.Path path = pathFinder.ShortestPath( 
                        Simulation.Map.Map.GetCell(Owner.X, Owner.Y), 
                        Simulation.Map.Map.GetCell(nearestEnemy.X, nearestEnemy.Y));
            
                    Simulation.Map.SetWalkable(Owner.X, Owner.Y, false);
                    Simulation.Map.SetWalkable(nearestEnemy.X, nearestEnemy.Y, false);
                
                    if (path == null)
                    {
                        Console.WriteLine($"{Owner.Name} is stuck.");
                        return;
                    }

                    Move(new Vector3(path.Steps.First().X, path.Steps.First().Y, 0));
                }
                
                // If we're close enough to attack, face the enemy but don't move
                if (length <= Owner.MainWeapon.Range)
                {
                    Owner.FacingDirection = new Vector3(_targetEntity.X, _targetEntity.Y, 0);
                    Owner.Attack(new Vector3(_targetEntity.X, _targetEntity.Y, 0));   
                    Console.WriteLine($"Attacking {_targetEntity.Name}.");
                }
            }
            else
            {
                // No enemy found, move randomly
                _targetEntity = null;
                MoveRandomly();
            }
        }
        
        private void MoveRandomly()
        {
            var (x, y) = (Owner.X, Owner.Y);
            
            var neighbors = Simulation.Map.Map.GetBorderCellsInSquare(x, y, 1).Where(c => c.IsWalkable).ToArray();

            if (neighbors.Length > 0)
            {
                var choice = neighbors[_random.Next(neighbors.Length)];
                Move(new Vector3(choice.X, choice.Y, 0));
            }
            else
            {
                TryFindNewWalkableCell();
            }
        }
        
        private void TryFindNewWalkableCell()
        {
            var goal = Simulation.Map.GetRandomWalkableLocation(Owner);
            
            var pathFinder = new IgnorantPathfinder(Simulation.Map.Map, new []{Simulation.Map.Map.GetCell(goal.x, goal.y), 
                Simulation.Map.Map.GetCell(Owner.X, Owner.Y)});
            
            var path = pathFinder.ShortestPath(Simulation.Map.Map.GetCell(Owner.X, Owner.Y), 
                Simulation.Map.Map.GetCell(goal.x, goal.y));
            
            if (path is { Length: > 1 })
            {
                var nextStep = path.StepForward();
                Move(new Vector3(nextStep.X, nextStep.Y, 0));
            }
        }

        private Character? FindNearestEnemy()
        {
            Character? nearestEnemy = null;
            float nearestDistance = float.MaxValue;
        
            FieldOfView characterFov = new FieldOfView(Simulation.Map.Map);
        
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
    }
}