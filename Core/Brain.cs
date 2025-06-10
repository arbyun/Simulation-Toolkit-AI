using System;
using RogueSharp;
using SimArena.Entities;

namespace SimArena.Core
{
    public abstract class Brain
    {
        protected Agent Agent;
        protected readonly IMap _map;
        protected readonly int _tickIntervalMs;
        protected readonly Random _random = new();
        protected DateTime _lastDecisionTime = DateTime.MinValue; // Initialize to MinValue to ensure first Think() call executes
        
        public int Team { get; }
        public bool FastMode { get; set; } = false; // When true, bypasses time checks for ultra-fast simulation
        
        public event Action<Agent, int, int>? OnMove;

        private bool _initialized => Agent != null;

        protected Brain(Agent agent, IMap map, int team, int tickIntervalMs = 500)
        {
            Agent = agent;
            _map = map;
            _tickIntervalMs = tickIntervalMs;
            Team = team;
        }
        
        protected Brain(IMap map, int team, int tickIntervalMs = 500)
        {
            _map = map;
            _tickIntervalMs = tickIntervalMs;
            Team = team;
        }
        
        public void SetAgent(Agent agent)
        {
            Agent = agent;
        }
        
        public virtual void Think()
        {
            if (!_initialized)
            {
                throw new Exception("Brain not initialized.");
            }
            
            // In fast mode, always execute thinking logic without time checks
            if (FastMode)
            {
                ExecuteThink();
                return;
            }
            
            // In normal mode, check if tick interval has passed since last time we made a decision
            if ((DateTime.UtcNow - _lastDecisionTime).TotalMilliseconds < _tickIntervalMs)
                return;
            
            // If so then continue to this
            _lastDecisionTime = DateTime.UtcNow;
            
            // Execute brain-specific logic
            ExecuteThink();
        }
        
        protected abstract void ExecuteThink();
        
        protected void MoveTo(int newX, int newY)
        {
            if (_map.IsWalkable(newX, newY))
            {
                var current = _map.GetCell(Agent.X, Agent.Y);
                _map.SetCellProperties(Agent.X, Agent.Y, current.IsTransparent, true);
               
                Agent.X = newX;
                Agent.Y = newY;
                
                var newer = _map.GetCell(newX, newY);
                _map.SetCellProperties(newX, newY, newer.IsTransparent, false);
                
                OnMove?.Invoke(Agent, newX, newY);
            }
        }
        
        public static (int x, int y) GetRandomWalkableLocation(IMap map, Agent agent = null)
        {
            var minX = 0;
            var maxX = map.Width - 1;
            var minY = 0;
            var maxY = map.Height - 1;
            
            // Ensure bounds are within map limits
            minX = Math.Clamp(minX, 0, map.Width - 1);
            maxX = Math.Clamp(maxX, 0, map.Width - 1);
            minY = Math.Clamp(minY, 0, map.Height - 1);
            maxY = Math.Clamp(maxY, 0, map.Height - 1);

            // Check if there's any walkable space in the area
            bool hasWalkableSpace = false;
            
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (agent != null)
                    {
                        if (x == agent.X && y == agent.Y)
                        {
                            continue;
                        }
                    }
                    
                    if (map.IsWalkable(x, y))
                    {
                        hasWalkableSpace = true;
                        break;
                    }
                }
                
                if (hasWalkableSpace) 
                    break;
            }

            if (!hasWalkableSpace)
            {
                if (agent != null)
                {
                    return (agent.X, agent.Y);
                }
                else
                {
                    throw new Exception("No place available to move to.");
                }
            }
        
            Random rand = new();
            
            // Try to find a random walkable location
            for (int i = 0; i < 100; i++)
            {
                int x = rand.Next(minX, maxX + 1);
                int y = rand.Next(minY, maxY + 1);

                if (map.IsWalkable(x, y))
                    return (x, y);
            }

            throw new InvalidOperationException("Could not find a walkable location after 100 attempts.");
        }
    }
}