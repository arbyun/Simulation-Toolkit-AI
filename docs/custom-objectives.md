---
title: Custom Objectives
---

# Creating Custom Objectives

This page will walk you through the process of creating a complete custom objective for SimArena, including all necessary components. 
We'll use the Deathmatch objective as a reference pattern.

## Overview of the Objective System Architecture

An objective in SimArena consists of several interconnected components:

1. **Objective Configuration**: Defines the parameters of the objective
2. **Objective Tracker**: Monitors the simulation and tracks progress toward the objective
3. **Simulation Result**: Stores the outcome of the objective
4. **Result Builder**: Constructs the result from tracker data
5. **Input/Data**: Contains the data needed to evaluate objective completion

Here's how these components relate to each other:

```
ObjectiveConfiguration → creates → ObjectiveTracker
ObjectiveTracker → tracked by → ResultBuilder
ResultBuilder → builds → SimulationResult
```

## Step 1: Define Your Objective Type

First, add your new objective type to the `SimulationObjective` enum:

```csharp
// In SimulationObjective.cs
public enum SimulationObjective
{
    TeamDeathmatch,
    CaptureAllFlags, // Your new objective type
    // Other objective types...
}
```

## Step 2: Create the Objective Configuration

Create a class that inherits from `ObjectiveConfiguration` and implement `IJsonOnDeserialized` if needed (this is only 
needed if you need to change any values after deserialization; see the example below). 
Don't forget to add the necessary JSON serialization attributes, usually given by the template 
`[JsonDerivedType(typeof(YourObjective), nameof(SimulationObjective.YourObjective))]`.

```csharp
namespace SimArena.Serialization.Configuration.Objectives
{
    [Serializable]
    [JsonDerivedType(typeof(CaptureAllFlagsObjective), nameof(SimulationObjective.CaptureAllFlags))]
    public class CaptureAllFlagsObjective : ObjectiveConfiguration, IJsonOnDeserialized
    {
        // Properties specific to this objective type
        public int Teams { get; set; }
        public int FlagsToCapture { get; set; }
        public int PlayersPerTeam { get; set; }

        // Constructor with parameters
        public CaptureTheFlagObjective(int teams, int flagsToCapture, int playersPerTeam) : 
            base(SimulationObjective.CaptureTheFlag)
        {
            // All validation done here should ideally be done in the OnDeserialized method as well
            Teams = teams < 2 ? 2 : teams;
            FlagsToCapture = flagsToCapture < 1 ? 1 : flagsToCapture;
            PlayersPerTeam = playersPerTeam < 1 ? 1 : playersPerTeam;
        }
    
        // Parameterless constructor for deserialization
        public CaptureTheFlagObjective() : base(SimulationObjective.CaptureTheFlag) { }
    
        // Constructor taking only the objective type
        public CaptureTheFlagObjective(SimulationObjective type) : base(type) { }

        // Implement IJsonOnDeserialized if needed for validation; in this case, we're clamping the values so that
        // we don't expect less than 2 teams, 1 flag to capture, or 1 player per team
        public void OnDeserialized()
        {
            Teams = Teams < 2 ? 2 : Teams;
            FlagsToCapture = FlagsToCapture < 1 ? 1 : FlagsToCapture;
            PlayersPerTeam = PlayersPerTeam < 1 ? 1 : PlayersPerTeam;
        }
        
        // Factory method to create the tracker
        public override IObjectiveTracker CreateTracker() => new CaptureTheFlagTracker(this);
    }
}
```

## Step 3: Create the Input Data

Create a class that holds the data needed to track your objective and that implements `IBuildsResult`.
`IBuildsResult` is used to build the result from the tracker data.

Absolutely required components for your input data:
- All the parameters from the objective configuration that are needed to evaluate the objective
- A `CreateBuilder` method that returns a new instance of the result builder

```csharp
namespace SimArena.Core.Objectives.Data
{
    [Serializable]
    public class CaptureAllFlagsInput : IBuildsResult
    {
        // Store data needed to evaluate the objective
        public Dictionary<int, int> TeamFlagCaptures { get; }
        public int FlagsToCapture { get; }
        public int Teams { get; }

        public ISimulationResultBuilder CreateBuilder()
        {
            return new CaptureAllFlagsSimulationResultBuilder();
        }
    }
}
```

## Step 4: Create the Simulation Result

Create a class that holds the result of the objective and implements `ISimulationResult`.
`ISimulationResult` is used to store the outcome of the objective.

Absolutely required components for your result:
- A constructor that takes the necessary parameters
- A `Read` method that returns a formatted string representing the result
- A `ToSerializable` method that returns a POCO that can be serialized

```csharp
using System;
using System.Collections.Generic;
using SimArena.Core.Objectives.Interfaces;

namespace SimArena.Core.Objectives.Results
{
    [Serializable]
    public class CaptureAllFlagsSimulationResult : ISimulationResult
    {
        // Properties to store the outcome
        public Dictionary<int, int> TeamFlagCaptures { get; }
        public int FlagsToCapture { get; }
        public int Teams { get; }
        
        public int WinningTeam { get; }

        public CaptureTheFlagSimulationResult(
            Dictionary<int, int> teamFlagCaptures, 
            int flagsToCapture, 
            int teams)
        {
            TeamFlagCaptures = new Dictionary<int, int>(teamFlagCaptures);
            FlagsToCapture = flagsToCapture;
            Teams = teams;
        }
        
        // Get the result as a formatted string
        public string Read()
        {
            var summary = $"Simulation ran for {TotalSimulationTicks} ticks.\n";
            
            if (IsCompleted)
            {
                summary += $"Team {WinningTeam} won by capturing {TeamFlagCaptures[WinningTeam]} flags!\n";
            }
            else
            {
                summary += "Simulation ended with no winner.\n";
            }
            
            summary += "Flag Captures per Team:\n";
            foreach (var team in TeamFlagCaptures.Keys)
            {
                summary += $"- Team {team}: {TeamFlagCaptures[team]} flags\n";
            }
            
            return summary;
        }
        
        // Get the result as a serializable object
        public object ToSerializable() => new 
        {
            TeamFlagCaptures,
            WinningTeam,
            IsCompleted,
            TotalSimulationTicks
        }
    }
}
```

## Step 5: Create the Result Builder

Create a class that builds the result from the input data and implements `ISimulationResultBuilder`.
`ISimulationResultBuilder` is used to build the result from the tracker data.

Absolutely required components for your result builder:
- Implement `ISimulationResultBuilder<YOUR_INPUT_DATA_TYPE_HERE>`. For our example, that would be `ISimulationResultBuilder<CaptureTheFlagInput>`
- A constructor that takes the input data
- A `Build` method that builds the final result

```csharp
using SimArena.Core.Objectives.Data;
using SimArena.Core.Objectives.Interfaces;
using SimArena.Core.Objectives.Results;

namespace SimArena.Core.Objectives.ResultBuilders
{
    public class CaptureAllFlagsSimulationResultBuilder : ISimulationResultBuilder<CaptureAllFlagsInput>
    {
        public ISimulationResult Build(CaptureAllFlagsInput input)
        {
            return new CaptureAllFlagsSimulationResult(input.TeamFlagCaptures, input.FlagsToCapture, input.Teams);
        }
        
        public ISimulationResult Build(object input)
        {
            return Build((CaptureAllFlagsInput)input);
        }
    }
}
```

## Step 6: Create the Objective Tracker

Create a class that implements `IObjectiveTracker` and any other interfaces needed.

For instance, if you need to interact with events, implement `IEventInteractor`.
Since many objectives will need to interact with agent death, we have implemented a helper interface `IKillTracker` that you can use. In our example, it's not needed.

Absolutely required components for your tracker:
- A constructor that takes the objective configuration
- A `GetInput` method that returns the input data
- An `Update` method that updates the tracker's state
- A `ShouldStop` property that returns true if the objective is complete

```csharp
namespace SimArena.Core.Objectives.Trackers
{
    public class CaptureAllFlagsTracker : IObjectiveTracker, IEventInteractor
    {
        private readonly CaptureAllFlagsObjective _objective;
        private SimulationEvents _events;
        
        // And whatever more properties you'd need to track this objective,
        // for example, a dictionary to keep track of flag captures
        private Dictionary<int, int> _teamFlagCaptures;

        public CaptureTheFlagTracker(CaptureTheFlagObjective objective)
        {
            _objective = objective;
        }

        // Initialize with the simulation instance; not obligatory, but useful
        public void Initialize(SimulationEvents simulationEvents)
        {
            _events = simulationEvents;
            
            // Subscribe to relevant events
            // For example:
            simulation.Events.StepCompleted += OnStepCompleted;
        }

        public IBuildsResult GetInput()
        {
            // Build the input data with whatever data you need
            return new CaptureAllFlagsInput
            {
                TeamFlagCaptures = _teamFlagCaptures,
                ...
            };
        }
    }
}
```

## Step 7: Register Your Objective Type for JSON Serialization

Make sure your objective type is registered in the base `ObjectiveConfiguration` class:

```csharp
// In ObjectiveConfiguration.cs
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(DeathmatchObjective), nameof(SimulationObjective.TeamDeathmatch))]
[JsonDerivedType(typeof(CaptureAllFlagsObjective), nameof(SimulationObjective.CaptureAllFlags))] // Add this line
public class ObjectiveConfiguration
{
    // Existing code...
}
```

## Using Your New Objective

Now you can use your new objective configuration:

```csharp
// Create a capture all flags objective
var ctfObjective = new CaptureAllFlagsObjective(
    teams: 2,
    flagsToCapture: 3,
    playersPerTeam: 3
);

// Example of how to serialize it to JSON
string json = JsonSerializer.Serialize(ctfObjective);

// Example of how to deserialize it from JSON
var loadedObjective = JsonSerializer.Deserialize<ObjectiveConfiguration>(json);

// Create a simulation with this objective
var simulation = new Simulation();
simulation.SetObjective(loadedObjective);
```

## Best Practices

1. **Validation**: Sometimes it's good to validate input parameters in constructors and the `OnDeserialized` method.
2. **Event Handling**: Properly subscribe to and unsubscribe from events in trackers.
3. **Clean Separation**: Keep tracker logic separate from result building and data collection.
4. **Documentation**: Add XML comments to explain the purpose and behavior of your objective components.
5. **Testability**: Design your objective system to be testable in isolation.
6. **Extensibility**: If you're creating a type of objective that might have variants, consider creating abstract base classes.
7. **Error Handling**: Include proper error handling, especially for events and user input.