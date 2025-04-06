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