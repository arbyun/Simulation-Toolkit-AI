using RogueSharp;
using RogueSharp.MapCreation;
using SimArena.Core.Analysis;
using SimArena.Core.Analysis.Data;
using SimArena.Core.Objectives;
using SimArena.Core.Objectives.Trackers.Interfaces;
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
        public List<IMapAnalysis> MapAnalyzers { get; private set; } = new();
        
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
            InitializeMapAnalyzers();
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
            InitializeMapAnalyzers();
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
            
            // Set the simulation reference on the brain so it can access analyzers
            if (agent.Brain != null)
            {
                agent.Brain.Simulation = this;
            }
            
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
            
            // Reset all map analyzers
            foreach (var analyzer in MapAnalyzers)
            {
                analyzer.Reset();
            }
        }
        
        /// <summary>
        /// Initializes the map analyzers and subscribes to events
        /// </summary>
        private void InitializeMapAnalyzers()
        {
            if (Map != null)
            {
                // Create default analyzers
                var deathAnalysis = new DeathAnalysis();
                var damageAnalysis = new DamageAnalysis();
                var healingAnalysis = new HealingAnalysis();
                
                // Initialize them with map dimensions
                deathAnalysis.Initialize(Map.Width, Map.Height);
                damageAnalysis.Initialize(Map.Width, Map.Height);
                healingAnalysis.Initialize(Map.Width, Map.Height);
                
                // Add to the list
                MapAnalyzers.Add(deathAnalysis);
                MapAnalyzers.Add(damageAnalysis);
                MapAnalyzers.Add(healingAnalysis);
                
                // Subscribe to events
                Events.OnAgentKilled += OnAgentKilled;
                //TODO: damage and healing events 
            }
        }
        
        /// <summary>
        /// Ensures map analyzers are initialized - useful for deserialized instances
        /// </summary>
        public void EnsureMapAnalyzersInitialized()
        {
            if (MapAnalyzers.Count == 0 && Map != null)
            {
                InitializeMapAnalyzers();
            }
        }
        
        /// <summary>
        /// Get a specific type of map analyzer
        /// </summary>
        /// <typeparam name="T">The type of analyzer to get</typeparam>
        /// <returns>The analyzer of the specified type, or null if not found</returns>
        public T? GetMapAnalyzer<T>() where T : class, IMapAnalysis
        {
            return MapAnalyzers.OfType<T>().FirstOrDefault();
        }
        
        /// <summary>
        /// Get a map analyzer by analysis type name
        /// </summary>
        /// <param name="analysisType">The type of analysis (e.g., "Death", "Damage", "Healing")</param>
        /// <returns>The analyzer with the specified type, or null if not found</returns>
        public IMapAnalysis? GetMapAnalyzer(string analysisType)
        {
            return MapAnalyzers.FirstOrDefault(a => a.AnalysisType.Equals(analysisType, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Event handler for when an agent is killed - records the death location
        /// </summary>
        private void OnAgentKilled(object sender, Agent killedAgent)
        {
            var deathAnalysis = GetMapAnalyzer<DeathAnalysis>();
            if (deathAnalysis != null)
            {
                var deathData = new DeathData(killedAgent.X, killedAgent.Y, killedAgent.Name, killedAgent.Team, CurrentStep);
                deathAnalysis.RecordData(deathData);
            }
        }
    }
}