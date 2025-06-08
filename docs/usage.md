---
title: Usage
---

# Usage Guide

Here are some common ways to use **SimArena**:

## 1. Running Simulations

To run a simulation, you need to create a `GameConfiguration` object and pass it to the `Simulation` constructor. 
The `GameConfiguration` object contains all the necessary information to set up the simulation, including the objective, agents, and map.

```csharp
GameConfiguration config = new GameConfiguration
{
    Name = "My Simulation",
    MapPath = "path/to/map.json",
    Objective = new StepsObjective(SimulationObjective.Steps, 10)
};

Simulation simulation = new Simulation(config);
```

What this does:

1. Creates a new `GameConfiguration` object with the specified name, map path, and objective.
2. Creates a new `Simulation` object with the specified configuration.
3. Initializes the simulation with the map and agents.
4. Starts the simulation.

## 2. Creating Custom Objectives

Please refer to the [Creating Custom Objectives](custom-objectives.md) documentation for more information.

## 3. Creating Custom Brains

Please refer to the [Creating Custom Brains](custom-brains.md) documentation for more information.

## 4. Creating Custom Weapons

Please refer to the [Creating Custom Weapons](custom-weapons.md) documentation for more information.

## 5. Creating Custom Maps

To create a custom map, you need to implement the `IMap` interface. This interface has several methods that you need to implement, 
including `GetCell`, `GetAllCells`, and `GetBorderCellsInSquare`.

```csharp
public class CustomMap : IMap
{
    public ICell GetCell(int x, int y)
    {
        // Your custom get cell logic here
    }
    
    public IEnumerable<ICell> GetAllCells()
    {
        // Your custom get all cells logic here
    }
    
    public IEnumerable<ICell> GetBorderCellsInSquare(int x, int y, int distance)
    {
        // Your custom get border cells in square logic here
    }
}
```

## 6. Serialization

**SimArena** uses System.Text.Json for serialization. To serialize an object, you can use the `JsonSerializer.Serialize` method. 
To deserialize an object, you can use the `JsonSerializer.Deserialize` method.
