---
title: Usage
---

# Usage Guide

Here are some common ways to use **SimArena**:

## Quick Start

For detailed setup instructions and code examples, check out the specific documentation pages related to each component.

For configuration examples, check out our [README](https://github.com/arbyun/Simulation-Toolkit-AI/blob/master/README.md#configurations) 
file on GitHub. Alternatively, explore the `configurations` folder in the project for more examples.

## Common Use Cases

### 1. Basic AI Testing
Create simple simulations to test different AI brain behaviors:
- Compare BrainA vs BrainB performance
- Test agent decision-making in different map layouts
- Evaluate weapon effectiveness across different scenarios

### 2. Game Balancing
Use SimArena to balance game mechanics:
- Run multiple simulations with different weapon configurations
- Test team compositions and map layouts
- Analyze combat effectiveness through the Map Analysis System

### 3. AI Development
Develop and test new AI behaviors:
- Create custom Brain classes that extend the base Brain
- Use Map Analysis data to make strategic decisions
- Test AI performance across different objectives (Deathmatch, Capture Point, etc.)

### 4. Research and Education
For academic projects and learning:
- Study emergent behaviors in multi-agent systems
- Analyze tactical decision-making patterns
- Generate data for research projects

## Getting Started

1. **Simple Approach**: Create simulations directly in code (ok for quick tests)
2. **Configuration Approach**: Use JSON configuration files with templates (recommended for reproducibility)

### Simple Approach Example

```csharp
// Create a simulation with a randomly generated map
var simulation = new Simulation(width: 20, height: 20);

// Create the objective and tracker
var objective = new DeathmatchObjective(SimulationObjective.TeamDeathmatch, 2, 10);
var tracker = new DeathmatchTracker(objective);
simulation.SetObjectiveTracker(tracker);

// Create agents with brains
var (x1, y1) = Brain.GetRandomWalkableLocation(simulation.Map);
var brain1 = new RandomBrain(simulation.Map, team: 0);
var agent1 = new Agent(x1, y1, brain1, "Agent Red");
brain1.SetAgent(agent1);

var (x2, y2) = Brain.GetRandomWalkableLocation(simulation.Map);
var brain2 = new TacticalBrain(simulation.Map, team: 1);
var agent2 = new Agent(x2, y2, brain2, "Agent Blue");
brain2.SetAgent(agent2);

// Add agents to the simulation
simulation.AddAgent(agent1);
simulation.AddAgent(agent2);

// Run the simulation
while (!simulation.IsGameOver)
{
    simulation.Update(deltaTime: 1.0f);
}

Console.WriteLine($"Game Over! Winner: Team {simulation.WinningTeam}");
```

### Configuration Approach Example

```csharp
// Load the configuration
var config = GameConfiguration.LoadFromJson("configurations/team_deathmatch.json");

// Create the simulation
var simulation = new SimulationWithConfiguration(config);

// Run the simulation
while (!simulation.IsGameOver)
{
    simulation.Update(deltaTime: 1.0f);
}

Console.WriteLine($"Simulation '{config.Name}' completed! Winner: Team {simulation.WinningTeam}");
```

## What's Next?

Once you've got the basics working, explore further documentation:
- [Custom Brains](custom-brains.md) - Create your own AI behaviors
- [Configuration](configuration.md) - Learn about the template and configuration system
- [Objectives](objectives.md) - Set up different win conditions
- [Weapons](weapons.md) - Configure combat mechanics
- [Maps](maps.md) - Create and customize maps
- [Map Analysis](map-analysis.md) - Analyze simulation data

