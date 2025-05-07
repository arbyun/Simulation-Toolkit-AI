using System.Numerics;
using SimArena.Core.SimulationElements.Map;
using SimArena.Core.Utilities;
using Path = System.IO.Path;

namespace SimArena.Core.Entities.Components;

public class SampleCapturePointAIBrain : Brain
 {
    private Vector3 _movementDirection = Vector3.Zero;
    private Entity? _targetEntity = null;
    private float _thinkTimer = 0;
    private const float THINK_INTERVAL = 0.5f; // Think every 0.5 seconds
    private Random _random = new Random();
    
    // Capture point area (rectangle)
    private int _captureStartX;
    private int _captureStartY;
    private int _captureEndX;
    private int _captureEndY;
    
    // Team information
    private int _teamId;
    private bool _isCapturing = false;
    
    // Respawn timer
    private float _respawnTimer = 0;
    private const float RESPAWN_TIME = 5.0f;
    private bool _waitingForRespawn = false;
    private Vector3 _respawnPosition = Vector3.Zero;
    
     public SampleCapturePointAIBrain(Character owner, int awareness, Simulation simulation) : base(owner, awareness, simulation)
     {
        // Set default capture point area in the center of the map
        _captureStartX = simulation.Map.Width / 2 - 2;
        _captureStartY = simulation.Map.Height / 2 - 2;
        _captureEndX = simulation.Map.Width / 2 + 2;
        _captureEndY = simulation.Map.Height / 2 + 2;
        
        // Assign a random team ID (0 or 1)
        _teamId = _random.Next(0, 2);

        _respawnPosition = new Vector3(owner.Position.X, owner.Position.Y, 0);
     }
    
    // Method to set the capture point area
    public void SetCapturePointArea(int startX, int startY, int endX, int endY)
    {
        _captureStartX = startX;
        _captureStartY = startY;
        _captureEndX = endX;
        _captureEndY = endY;
    }
    
    // Method to set the team ID
    public void SetTeamId(int teamId)
    {
        _teamId = teamId;
     }
 
     public override void Think(float deltaTime)
     { 
        // Update think timer
        _thinkTimer += deltaTime;
        
        // If we're dead, handle respawn
        if (!Owner.IsAlive)
        {
            if (!_waitingForRespawn)
            {
                _waitingForRespawn = true;
                _respawnTimer = 0;
                Console.WriteLine($"{Owner.Name} from Team {_teamId} was killed and will respawn in {RESPAWN_TIME} seconds.");
            }
            
            _respawnTimer += deltaTime;
            
            if (_respawnTimer >= RESPAWN_TIME)
            {
                Owner.Respawn(_respawnPosition);
                _waitingForRespawn = false;
            }
            
            return;
        }
        
        // Only think at certain intervals to improve performance
        if (_thinkTimer < THINK_INTERVAL)
        {
            return;
        }
        
        _thinkTimer = 0;
        MakeDecision();
    }

     public void MakeDecision()
     {
         // Check if we're on the capture point
        bool onCapturePoint = IsOnCapturePoint(Owner.X, Owner.Y);
        
        // Check if enemies are on the capture point
        bool enemiesOnCapturePoint = AreEnemiesOnCapturePoint();
        
        // Update capturing status
        if (onCapturePoint && !enemiesOnCapturePoint)
        {
            if (!_isCapturing)
            {
                _isCapturing = true;
                Console.WriteLine($"Team {_teamId} is on the point. Team {_teamId} capturing...");
            }
        }
        else if (_isCapturing)
        {
            _isCapturing = false;
            
            if (enemiesOnCapturePoint)
            {
                Console.WriteLine($"Enemies are on the point. Capturing stopped for Team {_teamId}...");
            }
            else if (!onCapturePoint)
            {
                Console.WriteLine($"Team {_teamId} left the capture point. Capturing stopped...");
            }
        }
        
        // Decide what to do:
        // 1. If enemies are on the capture point, attack them
        // 2. If no enemies on the capture point but we're not on it, move to it
        // 3. If we're on the capture point and no enemies, stay there
        if (enemiesOnCapturePoint)
        {
            // Find the nearest enemy on the capture point
            Character? nearestEnemy = FindNearestEnemyOnCapturePoint();
            
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
        }
        else if (!onCapturePoint)
        {
            // Move towards the capture point
            _targetEntity = null;
            
            // Calculate the center of the capture point
            int capturePointCenterX = (_captureStartX + _captureEndX) / 2;
            int capturePointCenterY = (_captureStartY + _captureEndY) / 2;
            
            Simulation.Map.SetWalkable(Owner.X, Owner.Y, true);
 
            PathFinder pathFinder = new PathFinder(Simulation.Map);
            MapPath path = pathFinder.ShortestPath( 
                    Simulation.Map.GetCell(Owner.X, Owner.Y), 
                    Simulation.Map.GetCell(capturePointCenterX, capturePointCenterY));
            
            Simulation.Map.SetWalkable(Owner.X, Owner.Y, false);

            _movementDirection = new Vector3(path.Steps.First().X, path.Steps.First().Y, 0);
        }
        else
        {
            // We're on the capture point and no enemies, stay here
            _targetEntity = null;
            _movementDirection = Vector3.Zero;
        }
        
        // Try to move in the calculated direction
        if (_movementDirection != Vector3.Zero)
        {
            int newX = Owner.X + (int)Math.Round(_movementDirection.X);
            int newY = Owner.Y + (int)Math.Round(_movementDirection.Y);
            
            // Check if the new position is valid
            if (!Simulation.Map.IsInBounds(newX, newY) || !Simulation.Map.IsWalkable(newX, newY))
            {
                // If we can't move in the desired direction, try a random direction
                _movementDirection = DirectionVector.GetRandomCardinalDirection();
                
                newX = Owner.X + (int)Math.Round(_movementDirection.X);
                newY = Owner.Y + (int)Math.Round(_movementDirection.Y);
                
                if (!Simulation.Map.IsInBounds(newX, newY) || !Simulation.Map.IsWalkable(newX, newY))
                {
                    _movementDirection = Vector3.Zero;
                }
            }
            
            if (_movementDirection != Vector3.Zero)
            {
                if (!Move(_movementDirection))
                {
                    _movementDirection = DirectionVector.GetRandomCardinalDirection();
                    Move(_movementDirection);
                }
            }
        }
     }
    
    private bool IsOnCapturePoint(int x, int y)
    {
        return x >= _captureStartX && x <= _captureEndX && y >= _captureStartY && y <= _captureEndY;
    }
    
    private bool AreEnemiesOnCapturePoint()
    {
        var characters = Simulation.GetEntities().OfType<Character>().Where(c => c.IsAlive && c != Owner);
        
        foreach (var character in characters)
        {
            // Check if the character is on the capture point
            if (IsOnCapturePoint(character.X, character.Y))
            {
                // Check if the character is from a different team
                if (GetTeamId(character) != _teamId)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private Character? FindNearestEnemyOnCapturePoint()
    {
        Character? nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        
        var characters = Simulation.GetEntities().OfType<Character>().Where(c => c.IsAlive && c != Owner);
        
        foreach (var character in characters)
        {
            // Check if the character is on the capture point
            if (IsOnCapturePoint(character.X, character.Y))
            {
                // Check if the character is from a different team
                if (GetTeamId(character) != _teamId)
                {
                    // Check if the character is in our field of view
                    if (Simulation.Map.IsInFov(character.X, character.Y))
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
            }
        }
        
        return nearestEnemy;
    }
    
    private int GetTeamId(Character character)
    {
        // In a real implementation, we would get the team ID from the character
        // For this test, we'll use a simple heuristic based on the character's name
        if (character.Brain is SampleCapturePointAIBrain brain)
        {
            return brain._teamId;
        }
        
        // If the character doesn't have a SampleCapturePointAIBrain, assume it's from a different team
        return (_teamId + 1) % 2;
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