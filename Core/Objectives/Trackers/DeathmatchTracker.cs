using System;
using System.Collections.Generic;
using System.Linq;
using SimArena.Core.Objectives.Trackers.Interfaces;
using SimArena.Core.Results;
using SimArena.Core.Results.Objective_Results;
using SimArena.Core.Results.Result_Data;
using SimArena.Entities;
using SimArena.Serialization.Configuration;
using SimArena.Serialization.Configuration.Objectives;

namespace SimArena.Core.Objectives.Trackers
{
    /// <summary>
    /// Tracker for deathmatch objectives
    /// </summary>
    public class DeathmatchTracker : IObjectiveTracker, IKillTracker, IEventInteractor
    {
        private readonly DeathmatchObjective _objective;
        private readonly List<List<Agent>> _teams = new();
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
            if (entity is Agent character)
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
                
                // // Set the team ID in the brain if it's a SampleDeathmatchAIBrain
                // if (character.Brain is SampleDeathmatchAIBrain brain)
                // {
                //     // In a real implementation, we would set the team ID in the brain
                // }
            }
        }
        
        private void OnKill(object simulation, (Entity killer, Entity killed) entities)
        {
            if (entities.killer is Agent killer && entities.killed is Agent victim)
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

        public DeathmatchTracker(DeathmatchObjective objective)
        {
            _objective = objective;
            
            // Initialize teams
            for (int i = 0; i < objective.Teams; i++)
            {
                _teams.Add(new List<Agent>());
            }
        }
        
        public void AddCharacterToTeam(Agent character, int teamIndex)
        {
            if (teamIndex < 0 || teamIndex > _teams.Count)
                throw new ArgumentOutOfRangeException(nameof(teamIndex));
            
            _teams[teamIndex].Add(character);
        }
        
        public void OnAgentKilled(Entity killer, Entity victim)
        {
            if (victim is Agent character)
            {
                // Mark the victim as dead
                character.Kill();
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
                    Agent character = team[j];
                    var kda = character.Kda;
                    
                    teamMembers[j] = new DeathmatchSimulationResult.TeamMember(character.Name, kda);
                    
                    teamKills += kda.Kills;
                    teamDeaths += kda.Deaths;
                    teamAssists += kda.Assists;
                }
                
                var teamKda = new Kda(teamKills, teamDeaths, teamAssists);
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
}