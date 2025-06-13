---
title: Simulation
---

# Simulation

The `Simulation` class is the main entry point for running simulations. It is responsible for managing the game state, 
including the map, agents, and entities. It also handles the game loop and updates the game state over time.

# Index

- [Creating a Simulation](#creating-a-simulation)
- [Running a Simulation](#running-a-simulation)
- [Accessing the Simulation](#accessing-the-simulation)
- [Modifying the Simulation](#modifying-the-simulation)
- [Events](#events)

## Creating a Simulation

To create a simulation, you can use the `Simulation` class.

```csharp
var simulation = new Simulation(map);
```

This will create a new simulation with the specified map. You can then add agents and entities to the simulation.

The simulation class can also easily be extended to add additional functionality.

```csharp
public class MyCustomSimulation : Simulation
{
    public MyCustomSimulation(Map map) : base(map)
    {
    }
    
    // Add custom functionality here
}
```

## Running a Simulation

For this example, let's assume we're running a simulation in an empty Program.cs file, 
and that we have no configuration file.

1. Create a map (or use an [IMapCreationStrategy](https://faronbracy.github.io/RogueSharp/api/RogueSharp.MapCreation.html) to generate one)
2. Create a simulation
3. Set the objective
3. Create agents
4. Add agents to the simulation
5. Run the simulation

```csharp
public static class Program
{
    /// <summary>
    /// Entry point for the application
    /// </summary>
    public static void Main(string[] args)
    {
        // Create a map
        var map = new Map(10, 10);
        
        // Create a simulation
        var simulation = new Simulation(map);
        
        // Decide speed of the simulation
        float updateRate = 0.5f; // Update every 0.5 seconds
        
        // Create the objective and tracker
        var objective = new DeathmatchObjective(SimulationObjective.TeamDeathmatch, 2, 3);
        var tracker = new DeathmatchTracker(objective);
        simulation.SetObjectiveTracker(tracker);
        
        // Create sample agents
        var agent1 = new Agent(0, 0, simulation);
        var agent2 = new Agent(9, 9, simulation);
        
        // Create a brain for the agents (Optional)
        var brain1 = new RandomBrain(agent1, map, 0);
        var brain2 = new RandomBrain(agent2, map, 1);
        agent1.SetBrain(brain1);
        agent2.SetBrain(brain2);
        
        // Add weapons to the agents (Optional)
        var weapon1 = new MeleeWeapon(0, 0, simulation);
        var weapon2 = new MeleeWeapon(9, 9, simulation);
        agent1.EquipWeapon(weapon1);
        agent2.EquipWeapon(weapon2);
        
        // Add agents to the simulation
        simulation.AddAgent(agent1);
        simulation.AddAgent(agent2);
        
        // Run the simulation
        while (!simulation.IsGameOver)
        {
            simulation.Update(updateRate);
        }
    }
}
```

Having a configuration file is the recommended way to run a simulation, as it allows for easy configuration and 
reproducibility. See the [configuration](configuration.md) documentation for more information.

Let us assume that we have a configuration file `path/to/config.json` in this example case:

```csharp
public static class Program
{
    /// <summary>
    /// Entry point for the application
    /// </summary>
    public static void Main(string[] args)
    {
        // Load the configuration
        var config = GameConfiguration.LoadFromJson("path/to/config.json");
        
        // Create the simulation
        var simulation = new SimulationWithConfiguration(config);
        
        // Decide speed of the simulation
        float updateRate = 0.5f; // Update every 0.5 seconds
        
        // Run the simulation
        while (!simulation.IsGameOver)
        {
            simulation.Update(updateRate);
        }
}
```

## Accessing the Simulation

Simulation access is mostly regulated through constructor injection. 
If we notice that static access is needed often, we will add it in the future.

## Modifying the Simulation

To modify the simulation, you can use the various methods provided by the `Simulation` class.

```csharp
simulation.AddAgent(agent);
simulation.RemoveAgent(agent);
simulation.AddEntity(entity);
simulation.RemoveEntity(entity);
```

## Events

The simulation raises events that you can subscribe to in order to be notified of changes to the game state.

```csharp
simulation.Events.OnCreate += (sender, entity) => { /* Your code here */ };
```

For a full list of events, see the `SimulationEvents` class.