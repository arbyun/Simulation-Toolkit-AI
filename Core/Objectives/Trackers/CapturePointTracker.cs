using System;
using System.Collections.Generic;
using System.Linq;
using SimArena.Core.Objectives.Trackers.Interfaces;
using SimArena.Core.Results;
using SimArena.Core.Results.Result_Data;
using SimArena.Entities;
using SimArena.Serialization.Configuration;
using SimArena.Serialization.Configuration.Objectives;

namespace SimArena.Core.Objectives.Trackers
{
    /// <summary>
    /// Tracker for capture point objectives
    /// </summary>
    public class CapturePointTracker : IObjectiveTracker, ITimeElapsedTracker, IEventInteractor
    {
        private readonly CapturePointObjective _objective;
        private float _captureProgress;
        private readonly List<List<Agent>> _teams = new();
        private int _capturingTeam = -1;
        private SimulationEvents _events;
        
        // Capture point area (rectangle)
        private int _captureStartX;
        private int _captureStartY;
        private int _captureEndX;
        private int _captureEndY;
        
        public float TimeElapsed { get; set; }
        public bool ShouldStop { get; private set; }
        
        public void InitializeEvents(SimulationEvents events)
        {
            _events = events;
            _events.OnCreate += OnCreate;
            _events.OnKill += OnKill;
        }
        
        private void OnCreate(object obj, Entity entity)
        {
            if (entity is Agent character)
            {
                // Assign the character to a team
                int teamIndex = _teams.Count > 0 ? 
                    _teams.FindIndex(t => t.Count < _objective.PlayersPerTeam) : -1;
                
                // If all teams are full, assign to the first team
                if (teamIndex == -1)
                {
                    teamIndex = 0;
                }
                
                AddCharacterToTeam(character, teamIndex);
                
                Console.WriteLine($"Assigned {character.Name} to Team {teamIndex}");
                
                // if (character.Brain is SampleCapturePointAIBrain brain)
                // {
                //     brain.SetCapturePointArea(_captureStartX, _captureStartY, _captureEndX, _captureEndY);
                //     brain.SetTeamId(teamIndex);
                // }
            }
        }
        
        private void OnKill(object simulation, (Entity killer, Entity killed) entities)
        {
            if (entities is { killer: Agent killer, killed: Agent victim })
            {
                // Update KDA stats
                killer.Kda = killer.Kda with { Kills = killer.Kda.Kills + 1 };
                victim.Kda = victim.Kda with { Deaths = victim.Kda.Deaths + 1 };
                
                // Log the kill
                int killerTeam = GetTeamIndex(killer);
                int victimTeam = GetTeamIndex(victim);
                
                Console.WriteLine($"{killer.Name} (Team {killerTeam}) killed {victim.Name} (Team {victimTeam})");
            }
        }
        
        private int GetTeamIndex(Agent character)
        {
            for (int i = 0; i < _teams.Count; i++)
            {
                if (_teams[i].Contains(character))
                {
                    return i;
                }
            }
            
            return -1;
        }
        
        public void AddCharacterToTeam(Agent character, int teamIndex)
        {
            if (teamIndex < 0 || teamIndex >= _teams.Count)
                throw new ArgumentOutOfRangeException(nameof(teamIndex));
            
            _teams[teamIndex].Add(character);
        }

        public CapturePointTracker(CapturePointObjective objective)
        {
            _objective = objective;
            
            // Initialize teams
            for (int i = 0; i < objective.Teams; i++)
            {
                _teams.Add(new List<Agent>());
            }
            
            // Set up the capture point area in the center of the map
            _captureStartX = 8;
            _captureStartY = 8;
            _captureEndX = 12;
            _captureEndY = 12;
        }
        
        public void Update(float deltaTime)
        {
            TimeElapsed += deltaTime;
            
            // Check which teams have characters on the capture point
            var teamsOnPoint = new Dictionary<int, int>();
            
            for (int i = 0; i < _teams.Count; i++)
            {
                int charactersOnPoint = 0;
                
                foreach (var character in _teams[i])
                {
                    if (character.IsAlive && IsOnCapturePoint(character))
                    {
                        charactersOnPoint++;
                    }
                }
                
                if (charactersOnPoint > 0)
                {
                    teamsOnPoint[i] = charactersOnPoint;
                }
            }
            
            // If only one team has characters on the point, they are capturing
            if (teamsOnPoint.Count == 1)
            {
                var team = teamsOnPoint.First();
                int teamIndex = team.Key;
                
                // If this is a new capturing team, log it
                if (_capturingTeam != teamIndex)
                {
                    _capturingTeam = teamIndex;
                    Console.WriteLine($"Team {teamIndex} is on the point. Team {teamIndex} capturing...");
                }
                
                // Increment the capture progress
                _captureProgress += deltaTime;
                
                // Check if the objective is complete
                if (_captureProgress >= _objective.CaptureTime)
                {
                    ShouldStop = true;
                    Console.WriteLine($"Team {teamIndex} has captured the point!");
                }
            }
            // If multiple teams have characters on the point, no one is capturing
            else if (teamsOnPoint.Count > 1)
            {
                // If a team was capturing, log that capturing has stopped
                if (_capturingTeam != -1)
                {
                    Console.WriteLine($"Multiple teams on the point. Capturing stopped...");
                    _capturingTeam = -1;
                }
            }
            // If no teams have characters on the point, no one is capturing
            else
            {
                // If a team was capturing, log that capturing has stopped
                if (_capturingTeam != -1)
                {
                    Console.WriteLine($"No teams on the point. Capturing stopped...");
                    _capturingTeam = -1;
                }
            }
        }
        
        private bool IsOnCapturePoint(Agent character)
        {
            return character.X >= _captureStartX && character.X <= _captureEndX &&
                   character.Y >= _captureStartY && character.Y <= _captureEndY;
        }
        
        public IBuildsResult GetInput()
        {
            return new StepsInput
            {
                Steps = (int)_captureProgress,
                MaxSteps = (int)_objective.CaptureTime
            };
        }
    }
}