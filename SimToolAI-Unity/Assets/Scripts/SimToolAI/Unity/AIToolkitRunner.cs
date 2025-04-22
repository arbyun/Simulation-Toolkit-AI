using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimToolAI.Core;
using SimToolAI.Core.Configuration;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;
using SimToolAI.Utilities;

namespace SimToolAI.Unity
{
    /// <summary>
    /// Unity MonoBehaviour for running AIToolkit simulations
    /// </summary>
    public class AIToolkitRunner : MonoBehaviour
    {
        #region Inspector Fields
        
        [Header("Configuration")]
        [SerializeField] private string configPath = "";
        [SerializeField] private bool loadOnStart = true;
        
        [Header("Simulation")]
        [SerializeField] private bool autoStart = true;
        [SerializeField] private float updateInterval = 0.05f;
        
        [Header("Visualization")]
        [SerializeField] private Grid grid;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject agentPrefab;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// The current simulation
        /// </summary>
        public Simulation Simulation { get; private set; }
        
        /// <summary>
        /// The match configuration
        /// </summary>
        public MatchConfig Config { get; private set; }
        
        /// <summary>
        /// Dictionary of entity GameObjects
        /// </summary>
        private Dictionary<Guid, GameObject> _entityObjects = new Dictionary<Guid, GameObject>();
        
        /// <summary>
        /// Whether the simulation is initialized
        /// </summary>
        public bool IsInitialized => Simulation != null;
        
        /// <summary>
        /// Whether the simulation is running
        /// </summary>
        public bool IsRunning => Simulation != null && Simulation.IsRunning;
        
        #endregion
        
        #region Unity Lifecycle
        
        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        private void Start()
        {
            if (loadOnStart && !string.IsNullOrEmpty(configPath))
            {
                LoadConfiguration(configPath);
                
                if (autoStart && IsInitialized)
                {
                    StartSimulation();
                }
            }
        }
        
        /// <summary>
        /// Called when the script is destroyed
        /// </summary>
        private void OnDestroy()
        {
            StopSimulation();
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Loads a configuration file
        /// </summary>
        /// <param name="path">Path to the configuration file</param>
        public void LoadConfiguration(string path)
        {
            try
            {
                // Load the configuration
                Config = MatchConfig.LoadFromFile(path);
                
                // Validate the configuration
                if (!Config.Validate(false, out string errorMessage))
                {
                    Debug.LogError($"Invalid configuration: {errorMessage}");
                    return;
                }
                
                // Initialize the simulation
                InitializeSimulation();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading configuration: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Initializes the simulation
        /// </summary>
        public void InitializeSimulation()
        {
            // Clean up any existing simulation
            CleanupSimulation();
            
            // Determine the simulation mode
            SimulationMode mode = Config.RealtimeMode ? SimulationMode.Realtime : SimulationMode.Offline;
            
            // If there are human agents, force realtime mode
            if (Config.Agents.Exists(a => a.BrainType == BrainType.Human))
            {
                mode = SimulationMode.Realtime;
            }
            
            // Create the simulation
            Simulation = new Simulation(Config, mode);
            
            // Subscribe to simulation events
            Simulation.Initialized += OnSimulationInitialized;
            Simulation.Started += OnSimulationStarted;
            Simulation.Paused += OnSimulationPaused;
            Simulation.Resumed += OnSimulationResumed;
            Simulation.Stopped += OnSimulationStopped;
            Simulation.StepCompleted += OnSimulationStepCompleted;
            
            // Initialize the simulation
            Simulation.Initialize();
        }
        
        /// <summary>
        /// Starts the simulation
        /// </summary>
        public void StartSimulation()
        {
            if (!IsInitialized)
                return;
                
            Simulation.Start();
            
            // Start the update coroutine
            StartCoroutine(UpdateSimulation());
        }
        
        /// <summary>
        /// Pauses the simulation
        /// </summary>
        public void PauseSimulation()
        {
            if (!IsRunning)
                return;
                
            Simulation.Pause();
        }
        
        /// <summary>
        /// Resumes the simulation
        /// </summary>
        public void ResumeSimulation()
        {
            if (IsRunning)
                return;
                
            Simulation.Resume();
        }
        
        /// <summary>
        /// Stops the simulation
        /// </summary>
        public void StopSimulation()
        {
            if (!IsInitialized)
                return;
                
            Simulation.Stop();
            CleanupSimulation();
        }
        
        /// <summary>
        /// Processes input for a human-controlled player
        /// </summary>
        /// <param name="playerId">ID of the player</param>
        /// <param name="direction">Direction to move, or null for no movement</param>
        /// <param name="attack">Whether to attack</param>
        /// <param name="targetId">ID of the target to attack, or null for default direction</param>
        public void ProcessPlayerInput(Guid playerId, Direction? direction, bool attack, Guid? targetId = null)
        {
            if (!IsRunning)
                return;
                
            Simulation.ProcessPlayerInput(playerId, direction, attack, targetId);
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Cleans up the simulation
        /// </summary>
        private void CleanupSimulation()
        {
            if (Simulation != null)
            {
                // Unsubscribe from simulation events
                Simulation.Initialized -= OnSimulationInitialized;
                Simulation.Started -= OnSimulationStarted;
                Simulation.Paused -= OnSimulationPaused;
                Simulation.Resumed -= OnSimulationResumed;
                Simulation.Stopped -= OnSimulationStopped;
                Simulation.StepCompleted -= OnSimulationStepCompleted;
                
                // Stop the simulation
                Simulation.Stop();
                Simulation = null;
            }
            
            // Destroy entity GameObjects
            foreach (var obj in _entityObjects.Values)
            {
                Destroy(obj);
            }
            
            _entityObjects.Clear();
        }
        
        /// <summary>
        /// Updates the simulation
        /// </summary>
        private IEnumerator UpdateSimulation()
        {
            while (IsRunning)
            {
                // Update the simulation
                Simulation.Update(updateInterval);
                
                // Update entity visualizations
                UpdateEntityVisualizations();
                
                // Wait for the next update
                yield return new WaitForSeconds(updateInterval);
            }
        }
        
        /// <summary>
        /// Updates entity visualizations
        /// </summary>
        private void UpdateEntityVisualizations()
        {
            if (Simulation == null || Simulation.Scene == null)
                return;
                
            // Update existing entities
            foreach (var entity in Simulation.Scene.GetEntities<Entity>())
            {
                if (_entityObjects.TryGetValue(entity.Id, out GameObject obj))
                {
                    // Update the object's position
                    obj.transform.position = grid.GetCellCenterWorld(new Vector3Int(entity.X, entity.Y));
                    
                    // Update the object's rotation based on facing direction
                    float angle = entity.FacingDirection switch
                    {
                        Direction.Up => 0,
                        Direction.Right => 90,
                        Direction.Down => 180,
                        Direction.Left => 270,
                        _ => 0
                    };
                    
                    obj.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
                else
                {
                    // Create a new object for the entity
                    CreateEntityObject(entity);
                }
            }
            
            // Remove objects for entities that no longer exist
            List<Guid> entitiesToRemove = new List<Guid>();
            
            foreach (var id in _entityObjects.Keys)
            {
                if (Simulation.Scene.GetEntity(id) == null)
                {
                    entitiesToRemove.Add(id);
                }
            }
            
            foreach (var id in entitiesToRemove)
            {
                if (_entityObjects.TryGetValue(id, out GameObject obj))
                {
                    Destroy(obj);
                    _entityObjects.Remove(id);
                }
            }
        }
        
        /// <summary>
        /// Creates a GameObject for an entity
        /// </summary>
        /// <param name="entity">Entity to create a GameObject for</param>
        private void CreateEntityObject(Entity entity)
        {
            // Determine which prefab to use
            GameObject prefab = entity is Player ? playerPrefab : agentPrefab;
            
            // Create the object
            GameObject obj = Instantiate(prefab, grid.GetCellCenterWorld(new Vector3Int(entity.X, entity.Y)), Quaternion.identity);
            obj.name = entity.Name;
            
            // Add the object to the dictionary
            _entityObjects[entity.Id] = obj;
            
            // Set up the object's components
            if (entity is Character character)
            {
                // Set up health bar, etc.
                var healthBar = obj.GetComponentInChildren<HealthBar>();
                if (healthBar != null)
                {
                    healthBar.SetMaxHealth(character.MaxHealth);
                    healthBar.SetHealth(character.Health);
                }
            }
        }
        
        #endregion
        
        #region Event Handlers
        
        /// <summary>
        /// Called when the simulation is initialized
        /// </summary>
        private void OnSimulationInitialized(object sender, EventArgs e)
        {
            Debug.Log("Simulation initialized");
        }
        
        /// <summary>
        /// Called when the simulation is started
        /// </summary>
        private void OnSimulationStarted(object sender, EventArgs e)
        {
            Debug.Log("Simulation started");
        }
        
        /// <summary>
        /// Called when the simulation is paused
        /// </summary>
        private void OnSimulationPaused(object sender, EventArgs e)
        {
            Debug.Log("Simulation paused");
        }
        
        /// <summary>
        /// Called when the simulation is resumed
        /// </summary>
        private void OnSimulationResumed(object sender, EventArgs e)
        {
            Debug.Log("Simulation resumed");
        }
        
        /// <summary>
        /// Called when the simulation is stopped
        /// </summary>
        private void OnSimulationStopped(object sender, SimulationResult e)
        {
            Debug.Log($"Simulation stopped after {e.Steps} steps and {e.ElapsedTime} seconds");
            Debug.Log($"Surviving agents: {e.SurvivingAgents.Count}");
            Debug.Log($"Defeated agents: {e.DefeatedAgents.Count}");
        }
        
        /// <summary>
        /// Called when a simulation step is completed
        /// </summary>
        private void OnSimulationStepCompleted(object sender, int e)
        {
            // Update entity visualizations
            UpdateEntityVisualizations();
        }
        
        #endregion
    }
}