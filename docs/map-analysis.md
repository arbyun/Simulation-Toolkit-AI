---
title: Map Analysis System
---

# Map Analysis System

The Map Analysis System is a flexible framework designed to track and analyze various events that occur during 
simulation. This system can be useful for creating strategic AI behaviors, generating heatmaps, and understanding 
battlefield dynamics.

The system uses multiple analyzers that implement the `IMapAnalysis` interface to track different types of events 
such as deaths, damage, and healing (those are examples; the system is meant to be extended). Each analyzer provides 
querying capabilities and can be accessed by AI brains for strategic decision making.

This system is designed to be easily extended to track other events on the map and provide analysis tools for 
those events as well.

# Index
- [Overview](#overview)
- [Key Features](#key-features)
- [Usage](#usage)
- [API Reference](#api-reference)
- [Data Structures](#data-structures)
- [Integration with Simulation](#integration-with-simulation)
- [Example: TacticalBrain](#example-tacticalbrain)
- [Performance Considerations](#performance-considerations)
- [Future Enhancements](#future-enhancements)

## Overview

The Map Analysis System automatically tracks various events during simulation and provides methods to query this data for:

- **Strategic AI Decision Making**: Brains can access analyzers to avoid dangerous areas or seek opportunities (see `TacticalBrain` for an example)
- **Heatmap Generation**: Visualize event patterns across the map (deaths, damage, healing)
- **Risk Assessment**: Evaluate the danger level of specific positions
- **Tactical Analysis**: Understand battlefield dynamics, useful for debugging and optimization, or for a game designer to see how the agents are performing on a map

## Key Features

The system currently includes:

### Multiple Analysis Types
- **Death Analysis**: Tracks agent deaths with position, timestamp, team, and step information
- **Damage Analysis**: Tracks damage events across the map (stub, full implementation planned later)
- **Healing Analysis**: Tracks healing events across the map (stub, full implementation planned later)
- Extensible framework for adding new analysis types

### Density Analysis
- Find the most dangerous/active areas on the map
- Calculate event density within specified radius
- Get risk assessments for specific positions

### Strategic Querying
- Query events by coordinate, radius, or team
- Generate heatmaps for visualization
- Analyze patterns over time
- Access from AI brains for strategic decision making

## Usage

### Basic Access

```csharp
// Access analyzers through the simulation
var deathAnalysis = simulation.GetMapAnalyzer<DeathAnalysis>();

// Check if it's initialized, for safety
if (deathAnalysis != null && deathAnalysis.IsInitialized)
{
    // Use the analyzer
    var totalDeaths = deathAnalysis.GetTotalDataCount();
}

// Or access by analysis type name
var analyzer = simulation.GetMapAnalyzer("Death");
```

### In Brain Implementation

```csharp
public class MyExampleTacticalBrain : Brain
{
    protected override void ExecuteThink()
    {
        var mapQuerier = Simulation.GetMapAnalyzer<DeathAnalysis>();
        
        if (!mapQuerier.IsInitialized)
        {
            // Fallback behavior
            MoveRandomly();
            return;
        }

        // For example, assess current position risk
        var deathsNearby = mapQuerier.GetDataInRadius(Agent.X, Agent.Y, 3);
        var riskLevel = CalculateRisk(deathsNearby.Count);

        if (riskLevel > 0.8)
        {
            // Move to safer area
            MoveToSaferPosition(mapQuerier);
        }
        else if (riskLevel < 0.3)
        {
            // Seek opportunities in moderate-risk areas
            MoveToOpportunityArea(mapQuerier);
        }
    }
}
```

## API Reference

### Core Methods

#### GetDataInRadius(int centerX, int centerY, int radius)
Gets all data points within a specified radius of a coordinate.

```csharp
var nearbyEvents = mapQuerier.GetDataInRadius(agent.X, agent.Y, 3);
```

#### GetDataAtCoordinate(int x, int y)
Gets all data points at a specific coordinate.

```csharp
var eventsHere = mapQuerier.GetDataAtCoordinate(x, y);
```

#### GetTotalDataCount()
Gets the total number of data points recorded.

```csharp
var totalCount = mapQuerier.GetTotalDataCount();
```

#### GetDataByCoordinate()
Gets all data points grouped by coordinate for density analysis.

```csharp
var dataByCoord = mapQuerier.GetDataByCoordinate();
```

##### Death Analysis

###### GetDeathsByTeam(int team)
Gets death statistics for a specific team.

```csharp
var teamDeaths = deathAnalysis.GetDeathsByTeam(1);
Console.WriteLine($"Team 1 has lost {teamDeaths.Count} agents");
```

###### GetMostDenseAreas(int radius = 2, int topCount = 10)
Gets the most densely populated (by death count) areas on the map.

```csharp
var dangerousAreas = deathAnalysis.GetMostDenseAreas(radius: 3, topCount: 5);
foreach (var area in dangerousAreas)
{
    Console.WriteLine($"Danger zone at ({area.X}, {area.Y}): {area.DeathCount} deaths");
}
```

## Data Structures

### DeathData
Represents a death event:
```csharp
public struct DeathData : IPositionalData
{
    public int X { get; }
    public int Y { get; }
    public DateTime Timestamp { get; }
    public int Step { get; }
    public string AgentName { get; }
    public int Team { get; }
}
```

### DamageData
Represents a damage event:
```csharp
public struct DamageData : IPositionalData
{
    public int X { get; }
    public int Y { get; }
    public DateTime Timestamp { get; }
    public int Step { get; }
    public int DamageAmount { get; }
    public int DealerTeam { get; }
    public int ReceiverTeam { get; }
    public string DealerName { get; }
    public string ReceiverName { get; }
}
```

### HealingData
Represents a healing event:
```csharp
public struct HealingData : IPositionalData
{
    public int X { get; }
    public int Y { get; }
    public DateTime Timestamp { get; }
    public int Step { get; }
    public int HealingAmount { get; }
    public int HealerTeam { get; }
    public int PatientTeam { get; }
    public string HealerName { get; }
    public string PatientName { get; }
}
```

### DensityArea
Represents an area with calculated data density:
```csharp
public class DensityArea<TData> where TData : IPositionalData
{
    public int X { get; }
    public int Y { get; }
    public int DataCount { get; }
    public double DensityScore { get; }
    public IReadOnlyList<TData> DataPoints { get; }
}
```

## Integration with Simulation

The Map Analysis System is automatically initialized when a Simulation is created and automatically tracks deaths through the event system. No manual setup is required.

```csharp
// Create simulation - Map Analysis System is automatically initialized
var simulation = new Simulation(width: 50, height: 50);

// Access an analyzer
var deathAnalysis = simulation.GetMapAnalyzer<DeathAnalysis>();
```

If you want to implement your own simulation, you will need to make sure to call `InitializeMapAnalyzers` 
and subscribe to the necessary events to track the data you want to analyze.

## Example: TacticalBrain

The included `TacticalBrain` class demonstrates how to use the Map Analysis System for strategic decision making:

- **Risk Assessment**: Evaluates the danger level of the current position
- **Safe Movement**: Moves to areas with lower death density when in danger
- **Opportunity Seeking**: Seeks areas with moderate activity when safe
- **Danger Avoidance**: Filters out high-risk moves when moving randomly

## Recommendations for Use

1. **Always Check Initialization**: Check `analyzer.IsInitialized` before using any querying methods
2. **Use The Appropriate Radius**: A larger radius gives broader context, a smaller radius gives precise local information

## Future Enhancements

Potential future features that we can implement could include:
- Time-weighted analysis (recent events more important)
- Team-specific analysis
- Weapon-specific analysis
- Predictive modeling