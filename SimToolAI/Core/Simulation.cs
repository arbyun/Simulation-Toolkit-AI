using System;
using System.Collections.Generic;
using System.Linq;
using SimToolAI.Core.Configuration;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;
using SimToolAI.Core.Rendering.RenderStrategies;
using SimToolAI.Utilities;
using UnityEngine;
using SimulationMode = SimToolAI.Core.Configuration.SimulationMode;

namespace SimToolAI.Core
{
    /// <summary>
    /// Core simulation class
    /// </summary>
    public class Simulation
    {
        #region Properties
        
        /// <summary>
        /// Match configuration
        /// </summary>
        public MatchConfig Config { get; }
        
        /// <summary>
        /// Simulation mode
        /// </summary>
        public SimulationMode Mode { get; }
        
        /// <summary>
        /// Map
        /// </summary>
        public ISimMap Map { get; protected internal set; }
        
        /// <summary>
        /// Scene
        /// </summary>
        public Scene Scene { get; protected internal set; }
        
        /// <summary>
        /// List of agents
        /// </summary>
        public List<Character> Agents { get; } = new List<Character>();
        
        /// <summary>
        /// Whether the simulation is running
        /// </summary>
        public bool IsRunning { get; private protected set; }
        
        /// <summary>
        /// Current step count
        /// </summary>
        public int CurrentStep { get; private protected set; }
        
        /// <summary>
        /// Maximum number of steps
        /// </summary>
        public int MaxSteps { get; }
        
        /// <summary>
        /// Elapsed time in seconds
        /// </summary>
        public float ElapsedTime { get; private protected set; }
        
        /// <summary>
        /// Whether the simulation has human-controlled agents
        /// </summary>
        public bool HasHumanAgents => Agents.Any(a => a is Player p && p.IsHumanControlled);
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Event raised when the simulation is initialized
        /// </summary>
        public event EventHandler Initialized;
        
        /// <summary>
        /// Event raised when the simulation is started
        /// </summary>
        public event EventHandler Started;
        
        /// <summary>
        /// Event raised when the simulation is paused
        /// </summary>
        public event EventHandler Paused;
        
        /// <summary>
        /// Event raised when the simulation is resumed
        /// </summary>
        public event EventHandler Resumed;
        
        /// <summary>
        /// Event raised when the simulation is stopped
        /// </summary>
        public event EventHandler<SimulationResult> Stopped;
        
        /// <summary>
        /// Event raised when a step is completed
        /// </summary>
        public event EventHandler<int> StepCompleted;
        
        /// <summary>
        /// Event raised when an entity is created
        /// </summary>
        public event EventHandler<Entity> OnCreate;
        
        /// <summary>
        /// Event raised when an entity is moved
        /// </summary>
        public event EventHandler<Entity> OnMove;
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Creates a new simulation with the specified configuration
        /// </summary>
        /// <param name="config">Match configuration</param>
        /// <param name="mode">Simulation mode</param>
        public Simulation(MatchConfig config, SimulationMode mode)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Mode = mode;
            
            if (mode == SimulationMode.Offline)
            {
                // Validate that there are no human agents in offline mode
                if (config.Agents.Any(a => a.BrainType == BrainType.Human))
                {
                    throw new InvalidOperationException("Human-controlled agents are not supported in offline mode");
                }
            }
            
            MaxSteps = config.MaxSteps;
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Initializes the simulation
        /// </summary>
        public void Initialize(ISimMap map, Scene scene)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));
            
            CreateAgents();
            
            CurrentStep = 0;
            ElapsedTime = 0f;
            
            // Raise the initialized event
            Initialized?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Creates agents based on the configuration
        /// </summary>
        private void CreateAgents()
        {
            // Clear any existing agents
            Agents.Clear();
            
            // Create agents from the configuration
            foreach (var agentConfig in Config.Agents)
            {
                // Determine the starting position
                int startX = agentConfig.StartX;
                int startY = agentConfig.StartY;
                
                if (agentConfig.RandomStart)
                {
                    var randomPos = Map.GetRandomWalkableLocation() ?? (5, 5);
                    startX = randomPos.Item1;
                    startY = randomPos.Item2;
                }
                
                // Create the agent
                Character agent;
                
                if (agentConfig.BrainType == BrainType.Human)
                {
                    // Create a human-controlled player
                    agent = new Player(agentConfig.Name, startX, startY, agentConfig.Awareness, this, true);
                }
                else
                {
                    // Create an AI-controlled character
                    agent = new Character(agentConfig.Name, startX, startY, agentConfig.Awareness, this);
                }
                
                // Set agent properties
                agent.MaxHealth = agentConfig.MaxHealth;
                agent.Health = agentConfig.MaxHealth;
                agent.AttackPower = agentConfig.AttackPower;
                agent.Defense = agentConfig.Defense;
                agent.Speed = agentConfig.Speed;
                
                // Raise the create event
                OnCreate?.Invoke(this, agent);
                
                // Add the agent to the scene and the list
                Scene.AddEntity(agent);
                Agents.Add(agent);
                
                // Enable field of view for the agent
                Map.ToggleFieldOfView(agent);
            }
        }
        
        /// <summary>
        /// Starts the simulation
        /// </summary>
        public void Start()
        {
            if (IsRunning)
                return;
                
            IsRunning = true;
            
            // Raise the started event
            Started?.Invoke(this, EventArgs.Empty);
            
            // If in offline mode, run the simulation to completion
            if (Mode == SimulationMode.Offline)
            {
                RunOfflineSimulation();
            }
        }
        
        /// <summary>
        /// Pauses the simulation
        /// </summary>
        public void Pause()
        {
            if (!IsRunning)
                return;
                
            IsRunning = false;
            
            // Raise the paused event
            Paused?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Resumes the simulation
        /// </summary>
        public void Resume()
        {
            if (IsRunning)
                return;
                
            IsRunning = true;
            
            // Raise the resumed event
            Resumed?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Stops the simulation
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
                return;
                
            IsRunning = false;
            
            // Create the simulation result
            var result = new SimulationResult
            {
                Steps = CurrentStep,
                ElapsedTime = ElapsedTime,
                SurvivingAgents = Agents.Where(a => a.IsAlive).ToList(),
                DefeatedAgents = Agents.Where(a => !a.IsAlive).ToList()
            };
            
            // Raise the stopped event
            Stopped?.Invoke(this, result);
        }
        
        /// <summary>
        /// Updates the simulation
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public void Update(float deltaTime)
        {
            if (!IsRunning)
                return;
                
            // Update the elapsed time
            ElapsedTime += deltaTime;
            
            // Update the scene
            Scene.Update(deltaTime);
            
            // Increment the step count
            CurrentStep++;
            
            // Raise the step completed event
            StepCompleted?.Invoke(this, CurrentStep);
            
            // Check if the simulation should stop
            if (CurrentStep >= MaxSteps || Agents.All(a => !a.IsAlive))
            {
                Stop();
            }
        }
        
        /// <summary>
        /// Runs the simulation in offline mode
        /// </summary>
        private void RunOfflineSimulation()
        {
            // Run the simulation until it stops
            while (IsRunning)
            {
                Update(0.05f); // 50ms per step
            }
        }
        
        /// <summary>
        /// Processes input for a human-controlled player
        /// </summary>
        /// <param name="entity">The entity to move</param>
        /// <param name="direction">Direction to move, or null for no movement</param>
        public bool ProcessMovement(Entity entity, System.Numerics.Vector3 direction)
        {
            // Calculate the new position based on the direction
            int newX = entity.X + (int)direction.X;
            int newY = entity.Y + (int)direction.Y;

            // Update the entity's facing direction if it's a player
            if (entity is Player player)
            {
                player.FacingDirection = direction;
            }
            
            bool success = Map.SetEntityPosition(entity, newX, newY);

            if (success)
            {
                OnMove?.Invoke(this, entity);
            }
            
            // Try to move the entity
            return Map.SetEntityPosition(entity, newX, newY);
        }
        
        /// <summary>
        /// Processes input for a human-controlled player
        /// </summary>
        /// <param name="entityId">ID of the entity</param>
        /// <param name="direction">Direction to move, or null for no movement</param>
        public void ProcessMovement(Guid entityId, System.Numerics.Vector3 direction)
        {
            // Find the target
            Entity target = Scene.GetEntity(entityId);
            
            // Process the input
            ProcessMovement(target, direction);
        }

        /// <summary>
        /// Processes the creation of a new entity
        /// </summary>
        /// <param name="entity">The entity to create</param>
        public void ProcessNewCreation(Entity entity)
        {
            Scene.AddEntity(entity);
            OnCreate?.Invoke(this, entity);
        }
        
        #endregion
    }
}