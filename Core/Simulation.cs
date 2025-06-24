using RogueSharp;
using RogueSharp.MapCreation;
using SimArena.Core.Objectives;
using SimArena.Core.Objectives.Trackers.Interfaces;
using SimArena.Core.Queries;
using SimArena.Core.Results.Result_Data;
using SimArena.Entities;

namespace SimArena.Core
{
    public class Simulation
    {
        public Map Map { get; protected set; }
        public List<Agent> Agents { get; } = new();
        public List<Entity> Entities { get; } = new();
        public bool IsGameOver { get; protected set; }
        public int WinningTeam { get; protected set; } = -1;
        public int CurrentStep { get; protected set; } = 0;
        
        public SimulationEvents Events { get; } = new();
        public MapQuerier MapQuerier => MapQuerier.Instance;
        
        protected IObjectiveTracker _objectiveTracker;

        /// <summary>
        /// Empty constructor, only intended to be used for deserialization of
        /// GameConfiguration in subclasses
        /// </summary>
        public Simulation()
        {}

        /// <summary>
        /// Create a simulation with the specified map
        /// </summary>
        /// <param name="map">The map to use for this simulation</param>
        public Simulation(Map map)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            InitializeMapQuerier();
        }
        
        /// <summary>
        /// Create a simulation with a randomly generated map
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="mapCreationStrategy">The strategy to use for map creation</param>
        public Simulation(int width, int height, IMapCreationStrategy<Map>? mapCreationStrategy = null)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
            
            // Default to RandomRoomsMapCreationStrategy if none is provided
            mapCreationStrategy ??= new RandomRoomsMapCreationStrategy<Map>(width, height, 10, 6, 6);
            
            Map = mapCreationStrategy.CreateMap();
            InitializeMapQuerier();
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
            Entities.Add(agent);
            
            // Raise the OnCreate event so that trackers can register the agent
            Events.RaiseOnCreate(this, agent);
        }
        
        public void AddEntity(Entity entity)
        {
            if (entity is Agent agent)
            {
                AddAgent(agent);
                return;
            }
            
            Entities.Add(entity);
            
            // Raise the OnCreate event so that trackers can register the entity
            Events.RaiseOnCreate(this, entity);
        }

        public void Update(float deltaTime = 1.0f)
        {
            if (IsGameOver)
                return;

            if (CurrentStep == 1)
            {
                Events.RaiseStarted(this);
            }

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
        
        public void RemoveEntity(Entity entity)
        {
            if (entity is Agent agent)
            {
                KillAgent(agent);
            }
            
            Entities.Remove(entity);
            
            Events.RaiseOnDestroy(this, entity);
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
        
        public T GetEntity<T>() where T : Entity
        {
            return Entities.OfType<T>().First();
        }
        
        public T GetEntity<T>(string id) where T : Entity
        {
            Guid.TryParse(id, out var guid);
            return Entities.OfType<T>().First(e => e.Id == guid);
        }
        
        public IEnumerable<T> GetEntities<T>() where T : Entity
        {
            return Entities.OfType<T>();
        }
        
        public void Reset()
        {
            IsGameOver = false;
            WinningTeam = -1;
            CurrentStep = 0;
            
            // Clear all agents
            Agents.Clear();
            
            // Reset map querier data
            MapQuerier.Reset();
        }
        
        /// <summary>
        /// Initializes the MapQuerier and subscribes to death events
        /// </summary>
        private void InitializeMapQuerier()
        {
            if (Map != null)
            {
                MapQuerier.Initialize(Map.Width, Map.Height);
                
                // Subscribe to agent death events to track death locations
                Events.OnAgentKilled += OnAgentKilled;
            }
        }
        
        /// <summary>
        /// Ensures MapQuerier is initialized - useful for deserialized instances
        /// </summary>
        public void EnsureMapQuerierInitialized()
        {
            if (!MapQuerier.IsInitialized && Map != null)
            {
                InitializeMapQuerier();
            }
        }
        
        /// <summary>
        /// Event handler for when an agent is killed - records the death location
        /// </summary>
        private void OnAgentKilled(object sender, Agent killedAgent)
        {
            MapQuerier.RecordDeath(killedAgent, CurrentStep);
        }
    }
}