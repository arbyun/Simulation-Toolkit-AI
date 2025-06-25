using RogueSharp;
using SimArena.Core;
using SimArena.Entities;
using SimArena.Utilities;

namespace SimArena.Brains
{
    public class ChaserBrain : Brain
    {
        private readonly Simulation _gameEngine;
        private const int DetectionRange = 10; // How far the agent can "see" other agents
        private readonly bool _isPassive;
        
        public ChaserBrain(Agent agent, IMap map, Simulation gameEngine, int team, bool isPassive = false, int tickIntervalMs = 500) 
            : base(agent, map, team, tickIntervalMs)
        {
            _gameEngine = gameEngine;
            _isPassive = isPassive;
        }
        
        public ChaserBrain(IMap map, Simulation gameEngine, int team, bool isPassive = false, int tickIntervalMs = 500) 
            : base(map, team, tickIntervalMs)
        {
            _gameEngine = gameEngine;
            _isPassive = isPassive;
        }

        protected override void ExecuteThink()
        {
            // Find the nearest enemy agent
            var nearestEnemy = FindNearestEnemy();
            
            if (nearestEnemy == null)
            {
                // No enemies found, move randomly
                MoveRandomly();
                return;
            }
            
            // Calculate distance to the nearest enemy
            int distanceToEnemy = CalculateManhattanDistance(Agent.X, Agent.Y, 
                nearestEnemy.X, nearestEnemy.Y);
            
            // Check if we're adjacent to the enemy
            if (distanceToEnemy <= 1)
            {
                if (!_isPassive)
                {
                    // We're adjacent to the enemy and we're aggressive, so kill it
                    _gameEngine.KillAgent(nearestEnemy);
                    return;
                }
            }
            
            // We're not adjacent to the enemy
            if (!_isPassive)
            {
                // We're aggressive, so chase the enemy
                MoveTowardsAgent(nearestEnemy);
            }
            else if (distanceToEnemy <= DetectionRange)
            {
                // We're passive and the enemy is within detection range, so flee
                FleeFromAgent(nearestEnemy);
            }
            else
            {
                // We're passive and the enemy is far away, so move randomly
                MoveRandomly();
            }
        }
        
        private Agent FindNearestEnemy()
        {
            Agent nearestEnemy = null;
            int shortestDistance = int.MaxValue;
            
            foreach (var agent in _gameEngine.Agents)
            {
                // Skip self and agents on the same team
                if (agent == Agent || agent.Brain.Team == Team || !agent.IsAlive)
                    continue;
                
                int distance = CalculateManhattanDistance(Agent.X, Agent.Y, agent.X, agent.Y);
                
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = agent;
                }
            }
            
            return nearestEnemy;
        }
        
        private int CalculateManhattanDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }
        
        private void MoveTowardsAgent(Agent target)
        {
            var pathFinder = new IgnorantPathfinder(_map, new []{_map.GetCell(target.X, target.Y), 
                _map.GetCell(Agent.X, Agent.Y)});
            
            var path = pathFinder.ShortestPath(
                _map.GetCell(Agent.X, Agent.Y), 
                _map.GetCell(target.X, target.Y));
            
            if (path is { Length: > 1 })
            {
                var nextStep = path.StepForward();
                MoveTo(nextStep.X, nextStep.Y);
            }
            else
            {
                // If no path is found, move randomly
                MoveRandomly();
            }
        }
        
        private void FleeFromAgent(Agent threat)
        {
            // Get all walkable neighbors
            var neighbors = _map.GetBorderCellsInSquare(Agent.X, Agent.Y, 1)
                .Where(c => c.IsWalkable)
                .ToList();
            
            if (neighbors.Count == 0)
            {
                // No walkable neighbors, try to find a path to a random location
                TryFindNewWalkableCell();
                return;
            }
            
            // Calculate the direction away from the threat
            int dirX = Agent.X - threat.X;
            int dirY = Agent.Y - threat.Y;
            
            // Find the neighbor that maximizes distance from the threat
            ICell bestCell = null;
            int maxDistance = -1;
            
            foreach (var cell in neighbors)
            {
                // Calculate the potential new distance if we move to this cell
                int newDistance = CalculateManhattanDistance(cell.X, cell.Y, threat.X, threat.Y);
                
                // Prefer cells that are in the direction away from the threat
                int directionBonus = 0;
                if ((dirX > 0 && cell.X > Agent.X) || (dirX < 0 && cell.X < Agent.X))
                    directionBonus += 2;
                if ((dirY > 0 && cell.Y > Agent.Y) || (dirY < 0 && cell.Y < Agent.Y))
                    directionBonus += 2;
                
                int score = newDistance + directionBonus;
                
                if (score > maxDistance)
                {
                    maxDistance = score;
                    bestCell = cell;
                }
            }
            
            if (bestCell != null)
            {
                MoveTo(bestCell.X, bestCell.Y);
            }
            else
            {
                // Fallback to random movement
                MoveRandomly();
            }
        }
        
        private void MoveRandomly()
        {
            var (x, y) = (Agent.X, Agent.Y);
            
            var neighbors = _map.GetBorderCellsInSquare(x, y, 1).Where(c => c.IsWalkable).ToArray();

            if (neighbors.Length > 0)
            {
                var choice = neighbors[_random.Next(neighbors.Length)];
                MoveTo(choice.X, choice.Y);
            }
            else
            {
                TryFindNewWalkableCell();
            }
        }
        
        private void TryFindNewWalkableCell()
        {
            var goal = GetRandomWalkableLocation(_map, Agent);
            
            var pathFinder = new IgnorantPathfinder(_map, new []{_map.GetCell(goal.x, goal.y), 
                _map.GetCell(Agent.X, Agent.Y)});
            
            var path = pathFinder.ShortestPath(_map.GetCell(Agent.X, Agent.Y), 
                _map.GetCell(goal.x, goal.y));
            
            if (path is { Length: > 1 })
            {
                var nextStep = path.StepForward();
                MoveTo(nextStep.X, nextStep.Y);
            }
        }
    }
}