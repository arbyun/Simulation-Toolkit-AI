using System.Numerics;
using SimArena.Core.Configuration;
using SimArena.Core.Entities;
using SimArena.Core.Entities.Components;
using SimArena.Core.Serialization.Objectives;
using SimArena.Core.Serialization.Results;
using SimArena.Core.SimulationElements.Map;
using SimArena.Core.SimulationElements.Scene;

namespace SimArena.Core
{
    public class Simulation
    {
        #region Properties
        
        /// <summary>
        /// Match configuration
        /// </summary>
        public GameConfig Config { get; }
    
        /// <summary>
        /// Simulation mode
        /// </summary>
        public SimulationMode Mode { get; }
    
        /// <summary>
        /// Map
        /// </summary>
        public IMap Map { get; protected internal set; }
    
        /// <summary>
        /// Scene
        /// </summary>
        private IScene Scene { get; set; }
    
        /// <summary>
        /// List of agents
        /// </summary>
        public List<Character> Agents { get; } = new();
    
        /// <summary>
        /// Whether the simulation is running
        /// </summary>
        public bool IsRunning { get; private protected set; }
        
        /// <summary>
        /// Events raised by the simulation
        /// </summary>
        public SimulationEvents Events { get; } = new();
    
        /// <summary>
        /// Whether the simulation has human-controlled agents
        /// </summary>
        public bool HasHumanAgents => Agents.Any(a => a.Brain is HumanBrain);
    
        #endregion
        
        private IObjectiveTracker _tracker;
        private readonly SimulationObjective _objective;
        
        #region Constructors
    
        /// <summary>
        /// Creates a new simulation with the specified configuration
        /// </summary>
        /// <param name="config">Match configuration</param>
        /// <param name="mode">Simulation mode</param>
        public Simulation(GameConfig config, SimulationMode mode)
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
            
            _objective = config.Objective.TypeEnum;
            _tracker = config.Objective.CreateTracker();
            
            if (_tracker is IStepTracker stepTracker)
            {
                stepTracker.StepCompleted += (step) => Events.RaiseStepCompleted(this, step);
            }
        }
    
        #endregion
    
        #region Methods
    
        /// <summary>
        /// Initializes the simulation
        /// </summary>
        public void Initialize(IMap map, Scene scene)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));
        
            CreateAgents();
        
            // Raise the initialized event
            Events.RaiseInitialized(this);
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
            
                // Create a default weapon for the agent
                RangedWeapon defaultWeapon = WeaponFactory.CreatePistol(startX, startY, this);
            
                // Use the builder pattern to create the character
                var builder = new CharacterBuilder(agentConfig.Name, startX, startY, this)
                    .WithWeapons(defaultWeapon)
                    .WithDimensions(1, 1)
                    .WithHealth(agentConfig.MaxHealth)
                    .WithCombatStats(agentConfig.AttackPower, agentConfig.Defense)
                    .WithSpeed(agentConfig.Speed);
                
                // Set the control type based on the configuration
                if (agentConfig.BrainType == BrainType.Human)
                {
                    builder.WithHumanControl(agentConfig.Awareness);
                }
                else
                {
                    builder.WithAIControl(agentConfig.Awareness);
                }
                
                // Build the character
                Character agent = builder.Build();
                
                // Raise the create event
                Events.RaiseOnCreate(this, agent);
            
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
            Events.RaiseStarted(this);
        
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
            Events.RaisePaused(this);
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
            Events.RaiseResumed(this);
        }
    
        /// <summary>
        /// Stops the simulation
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;

            var input = GetSimulationInput();

            ISimulationResult result = input.CreateBuilder().Build(input);
            
            Events.RaiseStopped(this, result);
        }
        
        private IBuildsResult GetSimulationInput()
        {
            return _tracker.GetInput();
        }
        
        /// <summary>
        /// Updates the simulation
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public void Update(float deltaTime)
        {
            if (!IsRunning)
                return;
        
            // Update the scene
            Scene.Update(deltaTime);
            
            _tracker.Update(deltaTime);
        
            // Check if the simulation should stop
            if (_tracker.ShouldStop)
                Stop();
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
        public bool ProcessMovement(Entity? entity, Vector3 direction)
        {
            if (entity == null)
            {
                return false;
            }
            
            // Calculate the new position based on the direction
            int newX = entity.X + (int)direction.X;
            int newY = entity.Y + (int)direction.Y;

            // Update the entity's facing direction if it's a player
            if (entity is Character player)
            {
                player.FacingDirection = direction;
            }
        
            // Try to move the entity
            bool success = Map.SetEntityPosition(entity, newX, newY);

            if (success)
            {
                Events.RaiseOnMove(this, entity);
            }
        
            return success;
        }
    
        /// <summary>
        /// Processes input for a human-controlled player
        /// </summary>
        /// <param name="entityId">ID of the entity</param>
        /// <param name="direction">Direction to move, or null for no movement</param>
        public bool ProcessMovement(Guid entityId, Vector3 direction)
        {
            // Find the target
            Entity? target = Scene.GetEntity(entityId);
            
            if (target == null)
            {
                return false;
            }
        
            // Process the input
            return ProcessMovement(target, direction);
        }
        
        /// <summary>
        /// Processes movement without manually moving the entity; assumes that the entity will be moved elsewhere
        /// </summary>
        /// <param name="entity">The entity to process</param>
        public void ProcessMovement(Entity? entity)
        {
            if (entity != null)
            {
                Events.RaiseOnMove(this, entity);
            }
        }

        /// <summary>
        /// Processes the creation of a new entity
        /// </summary>
        /// <param name="entity">The entity to create</param>
        public void ProcessNewCreation(Entity entity)
        {
            Scene.AddEntity(entity);
            Events.RaiseOnCreate(this, entity);
        }
        
        /// <summary>
        /// Destroys an entity
        /// </summary>
        /// <param name="entity">The entity to destroy</param>
        public void Destroy(Entity entity)
        {
            Scene.RemoveEntity(entity);
            Events.RaiseOnDestroy(this, entity);
        }
        
        public IEnumerable<Entity> GetEntities()
        {
            return Scene.GetEntities<Entity>();
        }

        public Entity? GetEntityAt(int newX, int newY)
        {
            return Scene.GetEntityAt(newX, newY);
        }
        
        #endregion
    }
}