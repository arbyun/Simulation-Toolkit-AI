using SimArena.Core.Configuration;
using SimArena.Core.Entities;
using SimArena.Core.Entities.Components;
using SimArena.Core.Serialization.Configuration;
using SimArena.Core.Serialization.Results;

namespace SimArena.Core.Serialization.Objectives
{
    /// <summary>
    /// Tracker for deathmatch objectives
    /// </summary>
    public class DeathmatchTracker : IObjectiveTracker, IKillTracker, IEventInteractor
    {
        private readonly DeathmatchObjective _objective;
        private readonly List<List<Character>> _teams = new();
        private int _winnerTeam = -1;
        private SimulationEvents _events;
        
        public bool ShouldStop { get; private set; }
        
        public void InitializeEvents(SimulationEvents events)
        {
            _events = events;
            _events.OnKill += OnKill;
            _events.OnCreate += OnCreate;
        }
        
        private void OnCreate(object simulation, Entity entity)
        {
            if (entity is Character character)
            {
                // Assign the character to a team
                int teamIndex = -1;
                
                if (_teams.Count > 0)
                {
                    if (_teams.Count + 1 <= _objective.Teams)
                    {
                        teamIndex = _teams.Count + 1;
                    }
                    else
                    {
                        teamIndex = _teams.FindIndex(t => t.Count < _objective.PlayersPerTeam);
                    }
                }
                
                // If all teams are full, assign to the first team
                if (teamIndex == -1)
                {
                    teamIndex = 0;
                }
                
                AddCharacterToTeam(character, teamIndex);
                
                Console.WriteLine($"Assigned {character.Name} to Team {teamIndex}");
                
                // Set the team ID in the brain if it's a SampleDeathmatchAIBrain
                if (character.Brain is SampleDeathmatchAIBrain brain)
                {
                    // In a real implementation, we would set the team ID in the brain
                }
            }
        }
        
        private void OnKill(object simulation, (Entity killer, Entity killed) entities)
        {
            if (entities.killer is Character killer && entities.killed is Character victim)
            {
                // Update KDA stats
                killer.Kda = killer.Kda with { Kills = killer.Kda.Kills + 1 };
                victim.Kda = victim.Kda with { Deaths = victim.Kda.Deaths + 1 };
                
                // Log the kill
                int killerTeam = GetTeamIndex(killer);
                int victimTeam = GetTeamIndex(victim);
                
                Console.WriteLine($"{killer.Name} (Team {killerTeam}) killed {victim.Name} (Team {victimTeam})");
                
                // Check if the objective is complete
                CheckObjectiveCompletion();
            }
        }
        
        private int GetTeamIndex(Character character)
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

        public DeathmatchTracker(DeathmatchObjective objective)
        {
            _objective = objective;
            
            // Initialize teams
            for (int i = 0; i < objective.Teams; i++)
            {
                _teams.Add(new List<Character>());
            }
        }
        
        public void AddCharacterToTeam(Character character, int teamIndex)
        {
            if (teamIndex < 0 || teamIndex > _teams.Count)
                throw new ArgumentOutOfRangeException(nameof(teamIndex));
            
            _teams[teamIndex].Add(character);
        }
        
        public void OnAgentKilled(Entity killer, Entity victim)
        {
            if (victim is Character character)
            {
                // Mark the victim as dead
                character.Health = 0;
            }
            
            CheckObjectiveCompletion();
        }
        
        private void CheckObjectiveCompletion()
        {
            // Count teams with living members
            int teamsWithLiving = 0;
            int lastLivingTeam = -1;
            
            for (int i = 0; i < _teams.Count; i++)
            {
                if (_teams[i].Any(c => c.IsAlive))
                {
                    teamsWithLiving++;
                    lastLivingTeam = i;
                }
            }
            
            // If only one team has living members, the objective is complete
            if (teamsWithLiving == 1)
            {
                _winnerTeam = lastLivingTeam;
                ShouldStop = true;
                Console.WriteLine($"Team {_winnerTeam} wins the deathmatch!");
            }
            // If no teams have living members, it's a draw
            else if (teamsWithLiving == 0)
            {
                _winnerTeam = -1;
                ShouldStop = true;
                Console.WriteLine("Deathmatch ended in a draw!");
            }
        }
        
        public IBuildsResult GetInput()
        {
            // Create team data for the result
            var teamData = new DeathmatchSimulationResult.Team[_teams.Count];
            
            for (int i = 0; i < _teams.Count; i++)
            {
                var team = _teams[i];
                var teamMembers = new DeathmatchSimulationResult.TeamMember[team.Count];
                
                int teamKills = 0;
                int teamDeaths = 0;
                int teamAssists = 0;
                
                for (int j = 0; j < team.Count; j++)
                {
                    Character character = team[j];
                    var kda = character.Kda;
                    
                    teamMembers[j] = new DeathmatchSimulationResult.TeamMember(character.Name, kda);
                    
                    teamKills += kda.Kills;
                    teamDeaths += kda.Deaths;
                    teamAssists += kda.Assists;
                }
                
                var teamKda = new DeathmatchSimulationResult.KDA(teamKills, teamDeaths, teamAssists);
                teamData[i] = new DeathmatchSimulationResult.Team(i, teamKda, teamMembers);
            }
            
            return new DeathmatchInput
            {
                WinnerTeam = _winnerTeam,
                TotalTeams = _teams.Count,
                Teams = teamData
            };
        }
        
        public void Update(float deltaTime)
        {
            CheckObjectiveCompletion();
        }
    }
    
    /// <summary>
    /// Tracker for capture point objectives
    /// </summary>
    public class CapturePointTracker : IObjectiveTracker, ITimeElapsedTracker, IEventInteractor
    {
        private readonly CapturePointObjective _objective;
        private float _captureProgress;
        private readonly List<List<Character>> _teams = new();
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
            if (entity is Character character)
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
                
                if (character.Brain is SampleCapturePointAIBrain brain)
                {
                    brain.SetCapturePointArea(_captureStartX, _captureStartY, _captureEndX, _captureEndY);
                    brain.SetTeamId(teamIndex);
                }
            }
        }
        
        private void OnKill(object simulation, (Entity killer, Entity killed) entities)
        {
            if (entities is { killer: Character killer, killed: Character victim })
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
        
        private int GetTeamIndex(Character character)
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
        
        public void AddCharacterToTeam(Character character, int teamIndex)
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
                _teams.Add(new List<Character>());
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
        
        private bool IsOnCapturePoint(Character character)
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
    
    /// <summary>
    /// Tracker for defend objectives
    /// </summary>
    public class DefendTracker : IObjectiveTracker, ITimeElapsedTracker
    {
        private readonly DefendObjective _objective;
        private float _objectiveHealth = 100;
        
        public float TimeElapsed { get; set; }
        public bool ShouldStop { get; private set; }
        
        public DefendTracker(DefendObjective objective)
        {
            _objective = objective;
        }
        
        public void Update(float deltaTime)
        {
            // Increment the time elapsed
            TimeElapsed += deltaTime;
            
            // In a real implementation, we would check if the objective is being attacked
            // and update the objective health accordingly
            
            // For testing, we'll just randomly decrease the objective health
            if (new Random().Next(0, 10) == 0)
            {
                _objectiveHealth -= 5;
                Console.WriteLine($"Objective health decreased to {_objectiveHealth}%");
            }
            
            // Check if the objective is complete
            if (_objectiveHealth <= _objective.ObjectiveThreshold || TimeElapsed >= _objective.MaxMatchTime)
            {
                ShouldStop = true;
            }
        }
        
        public IBuildsResult GetInput()
        {
            // In a real implementation, we would return more detailed information about the defend progress
            return new StepsInput
            {
                Steps = (int)TimeElapsed,
                MaxSteps = (int)_objective.MaxMatchTime
            };
        }
    }
}