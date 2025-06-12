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
        public readonly DeathmatchObjective _objective;
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
                
                // Assign to a team based on the agent's Team property
                teamIndex = character.Team;
                
                // Make sure the team index is valid
                if (teamIndex < 0 || teamIndex >= _teams.Count)
                {
                    // If the team index is invalid, assign to a team with space
                    teamIndex = _teams.FindIndex(t => t.Count < _objective.PlayersPerTeam);
                    
                    // If all teams are full or no valid team found, assign to the first team
                    if (teamIndex == -1)
                    {
                        teamIndex = 0;
                    }
                }
                
                AddCharacterToTeam(character, teamIndex);
                
                // Log team assignment if debug messaging is enabled
                if (_events != null)
                {
                    _events.RaiseDebugMessage(this, $"Assigned {character.Name} to Team {teamIndex}");
                }
                
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
                // Update KDA stats for killer and victim
                killer.Kda = killer.Kda with { Kills = killer.Kda.Kills + 1 };
                victim.Kda = victim.Kda with { Deaths = victim.Kda.Deaths + 1 };
                
                // Handle assists - check if victim has any recent attackers other than the killer
                if (victim.RecentAttackers != null && victim.RecentAttackers.Count > 0)
                {
                    var assistAgent = victim.RecentAttackers.LastOrDefault(a => a != killer);
                    if (assistAgent != null)
                    {
                        assistAgent.Kda = assistAgent.Kda with { Assists = assistAgent.Kda.Assists + 1 };
                    }
                }
                
                // Log the kill
                int killerTeam = GetTeamIndex(killer);
                int victimTeam = GetTeamIndex(victim);
                
                // Log the kill if debug messaging is enabled
                if (_events != null)
                {
                    _events.RaiseDebugMessage(this, $"{killer.Name} (Team {killerTeam}) killed {victim.Name} (Team {victimTeam})");
                }
                
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
                
                // Log the kill if debug messaging is enabled
                if (_events != null)
                {
                    _events.RaiseDebugMessage(this, $"Agent {character.Name} from Team {character.Team} was killed!");
                }
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
                // Only print detailed messages in debug mode
                if (_events != null)
                {
                    _events.RaiseDebugMessage(this, $"Team {_winnerTeam} wins the deathmatch!");
                }
            }
            // If no teams have living members, it's a draw
            else if (teamsWithLiving == 0)
            {
                _winnerTeam = -1;
                ShouldStop = true;
                // Only print detailed messages in debug mode
                if (_events != null)
                {
                    _events.RaiseDebugMessage(this, "Deathmatch ended in a draw!");
                }
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