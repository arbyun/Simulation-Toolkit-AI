# SimToolAI (Temp Name) - Simulation Toolkit

A flexible and extensible toolkit for creating simulations with AI-driven entities. Right now, this project only provides a 
foundation for building grid-based simulations. It aims to provide a solid foundation for developing complex simulations with customizable entities, maps, and rendering systems.

## Overview

SimToolAI is designed as a modular framework. The current implementation includes:

- Grid-based map system with support for walls, doors, and other terrain features
- Entity system with (some) customizable properties and behaviors
- Rendering system that supports both console and Unity-based visualization
- Field of view calculations for entities (WIP)
- Command system for handling user input and entity actions

## Core Components

### Map System
- `GridMap`: A grid-based map implementation using RogueSharp
- `ContinuousMap`: Not yet implemented but planned for future enhancements
- `GridMapParser`: A parser for loading grid-based maps from files or text
- Grid-based map types: Support for walkable/non-walkable cells, transparent/non-transparent cells for line of sight

### Entity System
- Base `Entity` class with position, awareness, and other common properties
- `Player` class for user-controlled entities
- `Bullet` class for projectiles launched by entities
- Support for entity movement, collision detection, and field of view

### Rendering System
- `IRenderable` interface for rendering entities and maps
- `Scene` class for managing scenes and their renderables
- `ConsoleScene` for console-based rendering
- `UnityScene` for Unity-based rendering
- Rendering strategies for different visualization approaches

## Demo

The project includes two simple demos (one console-based, the other unity-based) that demonstrate the core functionality:
- Map loading and rendering
- Player movement with WASD/arrow keys
- Simple projectile system (console demo uses spacebar to fire, unity demo uses left click)

## Future Development

- Complete implementation of continuous maps for more realistic simulations
- Full field of view calculation for entities
- Enhanced AI behaviors for non-player entities
- More sophisticated rendering options
- Additional map generation algorithms
- Expanded entity interaction systems
- Independence from RogueSharp (probably)
- Better architecture for extensibility and maintainability

## Project Structure

The project follows SOLID principles and is organized into the following main directories:
- `Core`: Contains the core simulation components
  - `Entities`: Entity-related classes
  - `Map`: Map-related classes
  - `Rendering`: Rendering-related classes
    - `RenderStrategies`: Strategies for different rendering methods
- `Utilities`: Helper classes and extensions

## Technologies Used

- C# (.NET)
- [RogueSharp](https://github.com/FaronBracy/RogueSharp/tree/main)

## Changelog

<details>
<summary> 
<b>22/04/2025</b>
</summary>

#### Core Architecture Changes

##### 1. Brain System
- Added a `Brain` abstract class as the base for all decision-making components
- Implemented `HumanBrain` for player-controlled entities
- Implemented `AIBrain` for AI-controlled entities
- Moved awareness from Entity to Brain for better encapsulation

##### 2. Entity Hierarchy
- Maintained `Entity` as the base class for all entities
- Added `Character` as an intermediate class with health, combat abilities
- Updated `Player` to inherit from `Character` instead of directly from `Entity`
- Characters now have a Brain that controls their decision-making

##### 3. Configuration System
- Added `MatchConfig` for loading simulation settings from JSON or XML
- Added `AgentConfig` for configuring individual agents
- Support for different brain types (Human/AI)
- Support for different simulation modes (Realtime/Offline)

##### 4. Simulation System
- Added `Simulation` class to manage the simulation lifecycle
- Support for Realtime and Offline simulation modes
- Event-based architecture for simulation events
- Proper separation of simulation logic from visualization

##### 5. Unity Integration
- Added `AIToolkitRunner` MonoBehaviour for Unity integration
- Support for loading configuration files in Unity
- Visualization of entities in Unity
- Input handling for human-controlled entities

##### 6. Console Application
- Added `ConsoleRunner` for running simulations from the command line
- Support for loading configuration files
- Proper error handling and validation
- Output of simulation results

#### Usage Examples

##### Console Application
```csharp
// Load a configuration file
var config = MatchConfig.LoadFromFile("sample_config.json");

// Create a simulation in offline mode
var simulation = new Simulation(config, SimulationMode.Offline);

// Initialize and run the simulation
simulation.Initialize();
simulation.Start();
```

##### Unity Integration
```csharp
// In a MonoBehaviour
public void StartSimulation(string configPath)
{
    // Load the configuration
    var config = MatchConfig.LoadFromFile(configPath);
    
    // Create the simulation
    var simulation = new Simulation(config, SimulationMode.Realtime);
    
    // Initialize and start the simulation
    simulation.Initialize();
    simulation.Start();
    
    // Process input for a human-controlled player
    simulation.ProcessPlayerInput(playerId, Direction.Up, true);
}
```

##### Configuration Example
```json
{
  "Name": "Sample Match",
  "MapPath": "maps/default.txt",
  "RealtimeMode": true,
  "MaxSteps": 1000,
  "Agents": [
    {
      "Name": "Player",
      "BrainType": "Human",
      "RandomStart": true,
      "Awareness": 15,
      "MaxHealth": 100,
      "AttackPower": 10,
      "Defense": 5,
      "Speed": 1.0
    },
    {
      "Name": "Enemy1",
      "BrainType": "AI",
      "RandomStart": true,
      "Awareness": 8,
      "MaxHealth": 80,
      "AttackPower": 12,
      "Defense": 3,
      "Speed": 1.2
    }
  ]
}
```
</details>
