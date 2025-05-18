using System.Numerics;
using RogueSharp;
using SimArena.Core.Configuration;
using SimArena.Core.Entities;
using SimArena.Core.Entities.Components;
using SimArena.Core.Serialization.Objectives;
using SimArena.Core.Serialization.Results;
using SimArena.Core.SimulationElements.Map;

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
        public IMapBridge Map { get; private set; }
    
        /// <summary>
        /// List of agents
        /// </summary>
        public List<Character> Agents => EntityManager.GetEntities<Character>().ToList();

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

        /// <summary>
        /// Entity manager
        /// </summary>
        public EntityManager EntityManager { get; set; } = new();

        #endregion

        private IObjectiveTracker _tracker;
        
        #region Constructors
    
        /// <summary>
        /// Creates a new simulation with the specified configuration
        /// </summary>
        /// <param name="config">Match configuration</param>
        /// <param name="mode">Simulation mode</param>
        public Simulation(GameConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Mode = config.RealtimeMode ? SimulationMode.Realtime : SimulationMode.Offline;
        
            if (Mode == SimulationMode.Offline)
            {
                // Validate that there are no human agents in offline mode
                if (config.Agents.Any(a => a.BrainType == BrainType.Human))
                {
                    throw new InvalidOperationException("Human-controlled agents are not supported in offline mode");
                }
            }
            
            _tracker = config.Objective.CreateTracker();
            
            if (_tracker is IEventInteractor eventInteractor)
            {
                eventInteractor.InitializeEvents(Events);
            }
        }
    
        #endregion
    
        #region Methods
    
        /// <summary>
        /// Initializes the simulation
        /// </summary>
        public void Initialize(IMapBridge map)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));

            if (Map.Map == null)
            {
                throw new Exception("Map bridge must have a map generated.");
            }

            CreateAgents();
        
            Events.RaiseInitialized(this);
        }
    
        /// <summary>
        /// Creates agents based on the configuration
        /// </summary>
        private void CreateAgents()
        {
            // Clear any existing agents
            EntityManager.Clear();
        
            // Create agents from the configuration
            foreach (var agentConfig in Config.Agents)
            {
                // Determine the starting position
                int startX = agentConfig.StartX;
                int startY = agentConfig.StartY;
            
                
                    var randomPos = Map.GetRandomWalkableLocation();
                    startX = randomPos.x;
                    startY = randomPos.y;
                

                CharacterBuilder builder = new(agentConfig.Name, startX, startY, this);

                if (agentConfig.OwnedWeaponIds.Length > 0)
                {
                    var ownedWeapons = new List<Weapon>();
                    
                    foreach (var weaponId in agentConfig.OwnedWeaponIds)
                    {
                        foreach (var weapon in Config.Weapons.Where(weapon => weapon.WeaponId == weaponId))
                        {
                            switch (weapon.WeaponType)
                            {
                                case WeaponType.Melee:
                                    MeleeWeapon meleeWeapon = new MeleeWeapon("melee", startX, startY, 
                                        true, this, weapon.Range)
                                    {
                                        Damage = weapon.Damage
                                    };
                                    ownedWeapons.Add(meleeWeapon);
                                    break;
                                    
                                case WeaponType.Ranged:
                                    RangedWeapon rangedWeapon = new RangedWeapon("ranged", startX, startY, true,
                                        this)
                                    {
                                        Damage = weapon.Damage,
                                        Range = weapon.Range
                                    };
                                    ownedWeapons.Add(rangedWeapon);
                                    break;
                            }
                        }
                    }
                    
                    builder.WithWeapons(ownedWeapons.ToArray())
                        .WithDimensions(1, 1)
                        .WithHealth(agentConfig.MaxHealth)
                        .WithCombatStats(agentConfig.AttackPower, agentConfig.Defense)
                        .WithSpeed(agentConfig.Speed);
                }
                else
                {
                    // Create a default weapon for the agent
                    MeleeWeapon defaultWeapon = WeaponFactory.CreateKnife(startX, startY, this);
            
                    // Use the builder pattern to create the character
                    builder.WithWeapons(defaultWeapon)
                        .WithDimensions(1, 1)
                        .WithHealth(agentConfig.MaxHealth)
                        .WithCombatStats(agentConfig.AttackPower, agentConfig.Defense)
                        .WithSpeed(agentConfig.Speed);
                }
                
                // Set the control type based on the configuration
                if (agentConfig.BrainType == BrainType.Human)
                {
                    builder.WithHumanControl(agentConfig.Awareness);
                }
                else
                {
                    builder.WithAIControl(agentConfig.Awareness, agentConfig.ThinkInterval);
                }
                
                // Build the character
                Character agent = builder.Build();
                
                // Raise the create event
                Events.RaiseOnCreate(this, agent);
            
                EntityManager.Set(agent, Map.Map.GetCell(agent.X, agent.Y));
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

            foreach (var agent in Agents)
            {
                agent.Brain.Think(deltaTime);
            }
            
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

            // Check if the new position is valid before attempting to move
            if (!Map.IsInBounds(newX, newY) || !Map.Map.IsWalkable(newX, newY))
            {
                return false;
            }

            // Update the entity's facing direction if it's a player
            if (entity is Character player)
            {
                player.FacingDirection = direction;
            }
        
            // Try to move the entity
            bool success = Map.SetEntityPosition(entity, newX, newY);

            if (success)
            {
                EntityManager.Set(entity, Map.Map.GetCell(newX, newY));
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
            Entity? target = EntityManager.Get(entityId);
            
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
                EntityManager.Set(entity, Map.Map.GetCell(entity.X, entity.Y));
                Events.RaiseOnMove(this, entity);
            }
        }

        /// <summary>
        /// Processes the creation of a new entity
        /// </summary>
        /// <param name="entity">The entity to create</param>
        public void ProcessNewCreation(Entity entity)
        {
            EntityManager.Set(entity, Map.Map.GetCell(entity.X, entity.Y));
            Events.RaiseOnCreate(this, entity);
        }
        
        /// <summary>
        /// Destroys an entity
        /// </summary>
        /// <param name="entity">The entity to destroy</param>
        public void Destroy(Entity entity)
        {
            EntityManager.Remove(entity);
            Events.RaiseOnDestroy(this, entity);
        }

        /// <summary>
        /// Handles the death of an entity
        /// </summary>
        /// <param name="killer">The entity that killed the other</param>
        /// <param name="killed">The entity that was killed</param>
        public void ProcessDeath(Entity killer, Entity killed)
        {
            Events.RaiseKill(this, (killer, killed));
        }

        /// <summary>
        /// Handles damage dealt between two entities
        /// </summary>
        /// <param name="attacker">The entity that attacked</param>
        /// <param name="defender">The entity that was attacked</param>
        public void ProcessDamage(Entity attacker, Entity defender)
        {
            Events.RaiseDamage(this, (attacker, defender));
        }

        /// <summary>
        /// Handles healing between two entities
        /// </summary>
        /// <param name="healer">The entity that healed the other</param>
        /// <param name="healed">The entity that was healed</param>
        public void ProcessHeal(Entity healer, Entity healed)
        {
            Events.RaiseHeal(this, (healer, healed));
        }
        
        public IEnumerable<Entity> GetEntities()
        {
            return EntityManager.GetEntities<Entity>();
        }

        public Entity GetEntityAt(int newX, int newY)
        {
            return EntityManager.Get(Map.Map.GetCell(newX, newY));
        }
        
        #endregion
    }
}