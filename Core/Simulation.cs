using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using RogueSharp.MapCreation;
using SimArena.Core.Objectives;
using SimArena.Core.Objectives.Trackers.Interfaces;
using SimArena.Core.Results;
using SimArena.Core.Results.Result_Data;
using SimArena.Entities;

namespace SimArena.Core
{
    public class Simulation
    {
        public Map Map { get; private set; }
        public List<Agent> Agents { get; } = new();
        public bool IsGameOver { get; private set; }
        public int WinningTeam { get; private set; } = -1;
        public int CurrentStep { get; private set; } = 0;
        
        public SimulationEvents Events { get; } = new();
        
        private IObjectiveTracker _objectiveTracker;
        private float _timeSinceLastUpdate = 0f;

        /// <summary>
        /// Create a simulation with the specified map
        /// </summary>
        /// <param name="map">The map to use for this simulation</param>
        public Simulation(Map map)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
        }
        
        /// <summary>
        /// Create a simulation with a randomly generated map
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="mapCreationStrategy">The strategy to use for map creation</param>
        public Simulation(int width, int height, IMapCreationStrategy<Map> mapCreationStrategy = null)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
            
            // Default to RandomRoomsMapCreationStrategy if none is provided
            mapCreationStrategy ??= new RandomRoomsMapCreationStrategy<Map>(width, height, 10, 6, 6);
            
            Map = mapCreationStrategy.CreateMap();
        }

        /// <summary>
        /// Set the objective tracker for this simulation
        /// </summary>
        /// <param name="tracker">The objective tracker to use</param>
        public void SetObjectiveTracker(IObjectiveTracker tracker)
        {
            _objectiveTracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
            
            // If this tracker implements IEventInteractor, initialize it with our events
            if (tracker is IEventInteractor eventInteractor)
            {
                eventInteractor.InitializeEvents(Events);
            }
        }

        public void AddAgent(Agent agent)
        {
            Agents.Add(agent);
            
            // Raise the OnCreate event so that trackers can register the agent
            Events.RaiseOnCreate(this, agent);
        }

        public void Update(float deltaTime = 1.0f)
        {
            if (IsGameOver)
                return;
                
            _timeSinceLastUpdate += deltaTime;
            CurrentStep++;
                
            // Only update living agents
            foreach (var agent in Agents.Where(a => a.IsAlive))
            {
                agent.Brain.Think();
            }
            
            // Use objective tracker to check victory conditions if available
            if (_objectiveTracker != null)
            {
                _objectiveTracker.Update(deltaTime);
                
                if (_objectiveTracker.ShouldStop)
                {
                    // Get result data from the tracker
                    var resultBuilder = _objectiveTracker.GetInput();
                    if (resultBuilder != null)
                    {
                        // The tracker has determined the game is over
                        IsGameOver = true;
                        
                        // For backward compatibility, try to get the winning team if possible
                        if (resultBuilder is DeathmatchInput deathmatchInput)
                        {
                            WinningTeam = deathmatchInput.WinnerTeam;
                        }
                    }
                }
            }
            else
            {
                // Fallback to the original deathmatch victory condition logic
                CheckDeathmatchVictoryConditions();
            }
            
            // Raise the step completed event
            Events.RaiseStepCompleted(this, CurrentStep);
        }
        
        public void KillAgent(Agent agent)
        {
            if (!agent.IsAlive)
                return;
                
            agent.Kill();
            
            // Make the cell walkable again
            var cell = Map.GetCell(agent.X, agent.Y);
            Map.SetCellProperties(agent.X, agent.Y, cell.IsTransparent, true);
            
            Events.RaiseOnAgentKilled(this, agent);
        }
        
        /// <summary>
        /// Legacy method for checking deathmatch victory conditions
        /// Only used if no objective tracker is set
        /// </summary>
        private void CheckDeathmatchVictoryConditions()
        {
            // Get all teams that still have living agents
            var remainingTeams = Agents
                .Where(a => a.IsAlive)
                .Select(a => a.Team)
                .Distinct()
                .ToList();
                
            // If only one team remains, they win
            if (remainingTeams.Count == 1)
            {
                WinningTeam = remainingTeams[0];
                IsGameOver = true;
                Events.RaiseOnTeamWon(this, WinningTeam);
            }
            // If no teams remain (shouldn't happen normally), it's a draw
            else if (remainingTeams.Count == 0)
            {
                IsGameOver = true;
                WinningTeam = -1; // No winner
            }
        }
        
        public void Reset()
        {
            IsGameOver = false;
            WinningTeam = -1;
            _timeSinceLastUpdate = 0f;
            CurrentStep = 0;
            
            // Clear all agents
            Agents.Clear();
        }
    }
}