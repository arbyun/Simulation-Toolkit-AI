using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimArena.Core.Objectives.Trackers.Interfaces;
using SimArena.Core.Results.Result_Data;
using SimArena.Entities;

namespace SimArena.Core
{
    /// <summary>
    /// Handles logging of match events and saving them to a file
    /// </summary>
    public class MatchLogger
    {
        private readonly Simulation _simulation;
        private readonly string _outputFolder;
        private readonly List<string> _logs = new List<string>();
        private readonly Dictionary<int, List<Agent>> _teams = new Dictionary<int, List<Agent>>();
        private int _currentStep;
        private DateTime _startTime;
        private IObjectiveTracker _objectiveTracker;
        
        public MatchLogger(Simulation simulation, string outputFolder)
        {
            _simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
            _outputFolder = outputFolder;
            _startTime = DateTime.Now;
            
            // Subscribe to simulation events
            _simulation.Events.OnAgentKilled += OnAgentKilled;
            _simulation.Events.OnTeamWon += OnTeamWon;
            _simulation.Events.OnDebugMessage += OnDebugMessage;
            _simulation.Events.OnCreate += OnCreate;
            _simulation.Events.OnKill += OnKill;
            _simulation.Events.StepCompleted += OnStepCompleted;
        }

        public void SetObjectiveTracker(IObjectiveTracker tracker)
        {
            _objectiveTracker = tracker;
        }
        
        private void OnCreate(object sender, Entity entity)
        {
            if (entity is Agent agent)
            {
                // Add agent to team tracking
                if (!_teams.ContainsKey(agent.Team))
                {
                    _teams[agent.Team] = new List<Agent>();
                }
                
                _teams[agent.Team].Add(agent);
            }
        }
        
        private void OnStepCompleted(object sender, int step)
        {
            _currentStep = step;
        }
        
        private void OnKill(object sender, (Entity killer, Entity killed) entities)
        {
            if (entities.killer is Agent killer && entities.killed is Agent victim)
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string logEntry = $"[Step {_currentStep}, {timestamp}] Agent {victim.Name} from Team {victim.Team} was killed by {killer.Name} from Team {killer.Team}!";
                _logs.Add(logEntry);
                
                // Add team status
                AddTeamStatusLog();
            }
        }
        
        private void OnAgentKilled(object sender, Agent agent)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = $"[Step {_currentStep}, {timestamp}] Agent {agent.Name} from Team {agent.Team} was killed!";
            _logs.Add(logEntry);
            
            // Add team status
            AddTeamStatusLog();
        }
        
        private void AddTeamStatusLog()
        {
            var teamStatus = new StringBuilder();
            int totalAlive = 0;
            
            foreach (var team in _teams)
            {
                int aliveCount = team.Value.Count(a => a.IsAlive);
                totalAlive += aliveCount;
                teamStatus.Append($"Team {team.Key}: {aliveCount} Agent{(aliveCount != 1 ? "s" : "")} Alive. ");
            }
            
            teamStatus.Append($"Total Alive: {totalAlive}");
            _logs.Add(teamStatus.ToString());
        }
        
        private void OnTeamWon(object sender, int teamId)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = $"[Step {_currentStep}, {timestamp}] Team {teamId} won the match!";
            _logs.Add(logEntry);
        }
        
        private void OnDebugMessage(object sender, string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = $"[Step {_currentStep}, {timestamp}] {message}";
            _logs.Add(logEntry);
        }
        
        /// <summary>
        /// Saves the match logs to a file in the specified output folder
        /// </summary>
        public void SaveLogs()
        {
            if (string.IsNullOrEmpty(_outputFolder))
                return;
                
            try
            {
                // Create the output folder if it doesn't exist
                if (!Directory.Exists(_outputFolder))
                {
                    Directory.CreateDirectory(_outputFolder);
                }
                
                // Generate a unique filename based on timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string filename = Path.Combine(_outputFolder, $"match_log_{timestamp}.txt");
                
                // Build the log content
                var content = new StringBuilder();
                
                // Add header information
                content.AppendLine("== INFO ==");
                
                // Get objective information if available
                if (_objectiveTracker != null && _objectiveTracker.GetInput() is DeathmatchInput deathmatchInput)
                {
                    var objective = deathmatchInput;
                    content.AppendLine($"Objective Type: {objective.GetType().ToString() ?? "Unknown"}");
                    content.AppendLine($"Objective Details: Teams: {objective.TotalTeams}, Players Per Team: {objective.Teams[0].TeamMembers.Length}");
                }
                else
                {
                    content.AppendLine("Objective Type: Unknown");
                    content.AppendLine("Objective Details: Not available");
                }
                
                content.AppendLine($"Map: {_simulation.Map.Width}x{_simulation.Map.Height}");
                content.AppendLine();
                
                // Add team information
                content.AppendLine("== TEAMS ==");
                foreach (var team in _teams)
                {
                    content.AppendLine($"Team {team.Key}");
                    content.AppendLine("------");
                    foreach (var agent in team.Value)
                    {
                        content.AppendLine(agent.Name);
                    }
                    content.AppendLine();
                }
                
                // Add match logs
                content.AppendLine("== MATCH LOGS ==");
                foreach (var log in _logs)
                {
                    content.AppendLine(log);
                }
                content.AppendLine();
                
                // Add results
                content.AppendLine("== RESULTS ==");
                content.AppendLine($"Winner: {(_simulation.WinningTeam >= 0 ? $"Team {_simulation.WinningTeam}" : "Draw")}");
                content.AppendLine();
                
                // Add KDA statistics
                content.AppendLine("== KDA ==");
                
                // Team collective KDAs
                foreach (var team in _teams)
                {
                    int teamKills = team.Value.Sum(a => a.Kda.Kills);
                    int teamDeaths = team.Value.Sum(a => a.Kda.Deaths);
                    int teamAssists = team.Value.Sum(a => a.Kda.Assists);
                    
                    content.AppendLine($"Team {team.Key} Collective KDA: {teamKills} / {teamDeaths} / {teamAssists}");
                }
                content.AppendLine();
                
                // Individual KDAs
                content.AppendLine("Individual KDAs");
                foreach (var team in _teams)
                {
                    foreach (var agent in team.Value)
                    {
                        content.AppendLine($"{agent.Name}: {agent.Kda.Kills} / {agent.Kda.Deaths} / {agent.Kda.Assists}");
                    }
                }
                
                // Write to file
                File.WriteAllText(filename, content.ToString());
                
                Console.WriteLine($"Match log saved to: {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving match log: {ex.Message}");
            }
        }
    }
}