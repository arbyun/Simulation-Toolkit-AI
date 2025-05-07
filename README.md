# Simulation Toolkit AI

A flexible and extensible simulation framework for AI-driven agent-based simulations. This toolkit provides a robust foundation for creating, configuring, and running simulations with multiple agents in various scenarios.

## Features

- **Flexible Simulation Engine**: Run simulations in real-time or offline mode
- **Agent-Based Architecture**: Create and manage multiple AI or human-controlled agents
- **Map System**: Grid-based map system with field of view calculations (based on RogueSharp)
- **Multiple Objective Types**: Support for various simulation objectives:
  - Team Deathmatch
  - Capture Point
  - Defense
  - Step-based simulations
- **JSON Configuration**: Easy setup through JSON configuration files
- **Event System**: Comprehensive event system for monitoring simulation state changes

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)
  - [Game Configuration](#game-configuration)
  - [Agent Configuration](#agent-configuration)
  - [Objective Configuration](#objective-configuration)
- [Architecture](#architecture)
- [Credits](#credits)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/arbyun/Simulation-Toolkit-AI.git
```

2. Open the solution in your preferred IDE (Visual Studio, Rider, etc.)

3. Build the solution:
```bash
dotnet build
```

## Usage

### Basic Simulation Setup

```csharp
// Create a game configuration or load existing one from a path
GameConfig config = new GameConfig
{
    Name = "My Simulation",
    MapPath = "path/to/map.txt",
    RealtimeMode = true,
    Objective = new StepsObjective(SimulationObjective.Steps, 1000),
    Agents = new List<AgentConfig>
    {
        new AgentConfig
        {
            Name = "Agent1",
            BrainType = BrainType.AI,
            RandomStart = true,
            Awareness = 10
        }
    }
};

// Create a simulation with the configuration
Simulation simulation = new Simulation(config, SimulationMode.Realtime);

// Initialize the simulation with a map and scene
IMap map = MapLoader.LoadMap(config.MapPath);
Scene scene = new Scene();
simulation.Initialize(map, scene);

// Start the simulation
simulation.Start();

// Update the simulation in a game loop (if in realtime mode)
while (simulation.IsRunning)
{
    simulation.Update(0.016f); // ~60 FPS
    // Render or process simulation state as needed
}
```

### Running an Offline Simulation

```csharp
// Create a configuration for offline mode
GameConfig config = new GameConfig
{
    Name = "Offline Simulation",
    MapPath = "path/to/map.txt",
    RealtimeMode = false,
    Objective = new StepsObjective(SimulationObjective.Steps, 1000),
    Agents = new List<AgentConfig>
    {
        // Only AI agents are supported in offline mode
        new AgentConfig { Name = "AI1", BrainType = BrainType.AI },
        new AgentConfig { Name = "AI2", BrainType = BrainType.AI }
    }
};

// Create and run the simulation
Simulation simulation = new Simulation(config, SimulationMode.Offline);
IMap map = MapLoader.LoadMap(config.MapPath);
Scene scene = new Scene();
simulation.Initialize(map, scene);

// Access the results
simulation.Events.OnStopped += (sim, result) => {
    Console.WriteLine($"Simulation completed with result: {result}");
};

// In offline mode, Start() will run the simulation to completion
simulation.Start();
```

## Configurations

### Game Configuration

The `GameConfig` class is the main configuration for a simulation:

```json
{
  "Name": "Example Simulation",
  "MapPath": "maps/arena.txt",
  "RealtimeMode": true,
  "Objective": {
    "Type": "Steps",
    "TypeEnum": "Steps",
    "MaxSteps": 10000
  },
  "Agents": [
    {
      "Name": "AI Agent 1",
      "BrainType": "AI",
      "RandomStart": true,
      "Awareness": 10,
      "MaxHealth": 100,
      "AttackPower": 10,
      "Defense": 5,
      "Speed": 1.0
    },
    {
      "Name": "Human Player",
      "BrainType": "Human",
      "RandomStart": true,
      "Awareness": 15,
      "MaxHealth": 120,
      "AttackPower": 15,
      "Defense": 3,
      "Speed": 1.2
    }
  ]
}
```

### Agent Configuration

The `AgentConfig` class configures individual agents:

```json
{
  "Name": "Agent Name",
  "BrainType": "AI",  // "AI" or "Human"
  "StartX": 0,        // Starting X position (ignored if RandomStart is true)
  "StartY": 0,        // Starting Y position (ignored if RandomStart is true)
  "RandomStart": true, // Whether to use a random starting position
  "Awareness": 10,    // Field of view radius
  "MaxHealth": 100,   // Maximum health points
  "AttackPower": 10,  // Base attack power
  "Defense": 5,       // Base defense value
  "Speed": 1.0        // Movement speed multiplier
}
```

### Objective Configuration

The toolkit supports several objective types:

#### Steps Objective

Runs the simulation for a specified number of steps:

```json
{
  "Type": "StepsObjective",
  "MaxSteps": 10000
}
```

#### Team Deathmatch Objective

Teams compete until only one team has surviving members:

```json
{
  "Type": "DeathmatchObjective",
  "Teams": 2,
  "PlayersPerTeam": 5
}
```

#### Capture Point Objective

Teams compete to capture and hold a point for a specified time:

```json
{
  "Type": "CapturePointObjective",
  "Teams": 2,
  "PlayersPerTeam": 5,
  "CaptureRadius": 5.0,
  "CaptureTime": 60.0
}
```

#### Defense Objective

One team defends an objective while another team attacks:

```json
{
  "Type": "DefendObjective",
  "Teams": 2,
  "PlayersPerTeam": 5,
  "MaxMatchTime": 120.0,
  "ObjectiveThreshold": 0
}
```

## Architecture

The toolkit is built around several core components:

```mermaid
classDiagram
    class Simulation {
        +GameConfig Config
        +SimulationMode Mode
        +IMap Map
        +List~Character~ Agents
        +bool IsRunning
        +SimulationEvents Events
        +Initialize(IMap map, Scene scene)
        +Start()
        +Pause()
        +Resume()
        +Stop()
        +Update(float deltaTime)
    }
    
    class GameConfig {
        +string Name
        +string MapPath
        +bool RealtimeMode
        +ObjectiveConfig Objective
        +List~AgentConfig~ Agents
        +LoadFromJson(string path)
        +SaveToJson(string path)
    }
    
    class AgentConfig {
        +string Name
        +BrainType BrainType
        +int StartX
        +int StartY
        +bool RandomStart
        +int Awareness
        +int MaxHealth
        +int AttackPower
        +int Defense
        +float Speed
    }
    
    class ObjectiveConfig {
        +SimulationObjective TypeEnum
        +CreateTracker()
    }
    
    class StepsObjective {
        +int MaxSteps
    }
    
    class DeathmatchObjective {
        +int Teams
        +int PlayersPerTeam
    }
    
    class CapturePointObjective {
        +float CaptureRadius
        +float CaptureTime
    }
    
    class DefendObjective {
        +float ObjectiveThreshold
        +float MaxMatchTime
    }
    
    class IMap {
        +int Width
        +int Height
        +bool IsWalkable(int x, int y)
        +bool IsTransparent(int x, int y)
        +SetEntityPosition(Entity entity, int x, int y)
        +ToggleFieldOfView(Character character)
    }
    
    class Entity {
        +Guid Id
        +int X
        +int Y
        +Update(float deltaTime)
    }
    
    class Character {
        +string Name
        +int Health
        +int MaxHealth
        +IBrain Brain
        +List~Weapon~ Weapons
    }
    
    Simulation --> GameConfig
    Simulation --> IMap
    GameConfig --> ObjectiveConfig
    GameConfig --> AgentConfig
    ObjectiveConfig <|-- StepsObjective
    ObjectiveConfig <|-- DeathmatchObjective
    DeathmatchObjective <|-- CapturePointObjective
    DeathmatchObjective <|-- DefendObjective
    Simulation --> Character
    Entity <|-- Character
```

## Simulation Flow

```mermaid
flowchart TD
    A[Create GameConfig] --> B[Create Simulation]
    B --> C[Initialize with Map & Scene]
    C --> D[Start Simulation]
    D --> E{Simulation Mode?}
    E -->|Offline| F[Run to completion]
    E -->|Realtime| G[Update in game loop]
    G --> H{Is Running?}
    H -->|Yes| G
    H -->|No| I[Process Results]
    F --> I
```

## Credits

This project makes extensive use of the RogueSharp library for map handling, field of view calculations, and other grid-based functionality. Special thanks to:

- [RogueSharp](https://github.com/FaronBracy/RogueSharp) by Faron Bracy - A .NET Standard library that provides map generation, field-of-view calculations, and other roguelike game utilities.
