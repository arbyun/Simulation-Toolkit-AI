using RogueSharp;
using SimArena.Entities;
using SimArena.Serialization.Configuration;

namespace SimArena.Core
{
    public class SimulationWithConfiguration: Simulation
    {
        protected GameConfiguration _config;
    
        public SimulationWithConfiguration(GameConfiguration config)
        {
            _config = config;
        
            _config.Validate(out var errorMessage);
        
            if (errorMessage != null)
            {
                throw new Exception(errorMessage);
            }
        
            // Create the map from the configuration
            if (config.Map != null)
            {
                Map = config.Map.CreateMap();
            }
            else
            {
                // Default fallback map if no configuration is provided
                Map = new Map(10, 10);
            }
        
            // Create the objective tracker from the configuration
            var objectiveTracker = config.Objective.CreateTracker();
            SetObjectiveTracker(objectiveTracker);
        
            // Create the agents from the configuration
            CreateAgentsFromConfiguration();
        }

        private void CreateAgentsFromConfiguration()
        {
            TemplateManager templateManager = new TemplateManager();
            templateManager.LoadTemplates();
            _config.ResolveTemplates(templateManager);
        
            foreach (var agentConfig in _config.Agents)
            {
                CreateAgentFromConfiguration(agentConfig);
            }
        }

        private void CreateAgentFromConfiguration(AgentConfiguration agentConfig)
        {
            Agent agent;
        
            Brain brain = agentConfig.Brain.CreateBrain(null, Map, this);
        
            // Create the agent
            if (agentConfig.RandomStart)
            {
                agent = new Agent(
                    Brain.GetRandomWalkableLocation(Map).x,
                    Brain.GetRandomWalkableLocation(Map).y,
                    brain,
                    agentConfig.Name
                );
            }
            else
            {
                agent = new Agent(
                    agentConfig.StartX,
                    agentConfig.StartY,
                    brain,
                    agentConfig.Name
                );
            }
        
            // Set the agent on the brain
            brain.SetAgent(agent);
        
            // Resolve owned weapons
            foreach (var weaponId in agentConfig.OwnedWeaponIds)
            {
                WeaponConfiguration weaponConfig = _config.Weapons.First(w => w.WeaponId == weaponId);
                var weapon = weaponConfig.CreateWeapon(agent.X, agent.Y, this, agent);
                agent.EquipWeapon(weapon);
            }
        
            // Add the agent to the simulation
            AddAgent(agent);
        }
    }
}