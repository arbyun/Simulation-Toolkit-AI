using System.Linq;
using RogueSharp;
using SimArena.Core;
using SimArena.Entities;

namespace SimArena.Brains
{
    public class RandomBrain : Brain
    {
        public RandomBrain(Agent agent, IMap map, int team, int tickIntervalMs = 500) 
            : base(agent, map, team, tickIntervalMs)
        {
        }
        
        public RandomBrain(IMap map, int team, int tickIntervalMs = 500) 
            : base(map, team, tickIntervalMs)
        {
        }

        protected override void ExecuteThink()
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
            
            var pathFinder = new PathFinder(_map);
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