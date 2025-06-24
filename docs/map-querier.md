---
title: MapQuerier
---

# Map Tracking and Analysis System

The MapQuerier is a singleton service that is designed to make queries to the map and the simulation. This can be 
useful for, for example, creating strategic AI behaviors and, possibly, generating heatmaps.

Right now, as an example implementation, the MapQuerier tracks where agents die on the map and provides
analysis tools for creating strategic AI behaviors and, possibly, generating heatmaps.

This system is designed to be easily extended to track other events on the map and provide analysis 
tools for those events as well.

# Index
- [Overview](#overview)
- [Key Features](#key-features)
- [Usage](#usage)
- [API Reference](#api-reference)
- [Data Structures](#data-structures)
- [Integration with Simulation](#integration-with-simulation)
- [Example: TacticalBrain](#example-tacticalbrain)
- [Recommendations for Use](#recommendations-for-use)
- [Performance Considerations](#performance-considerations)
- [Future Enhancements](#future-enhancements)

## Overview

Right now, the MapQuerier automatically tracks every agent death during simulation 
and provides various methods to query this data for:

- **Strategic AI Decision Making**: Brains can avoid dangerous areas or seek opportunities (see `TacticalBrain` for an example)
- **Heatmap Generation**: Visualize death patterns across the map
- **Risk Assessment**: Evaluate the danger level of specific positions
- **Tactical Analysis**: Understand battlefield dynamics, useful for debugging and optimization, or for a game 
designer to see how the agents are performing on a map (see `MapAnalyzer` for an example)

## Key Features

For now, we have:

### Automatic Death Tracking
- Automatically records every agent death with position, timestamp, team, and step information
- Thread-safe singleton pattern for easy access from anywhere in the codebase
- Integrates seamlessly with the simulation event system

### Density Analysis
- Find the most dangerous areas on the map
- Calculate death density within specified radius
- Get risk assessments for specific positions

### Strategic Querying
- Query deaths by coordinate, radius, or team
- Generate heatmaps for visualization
- Analyze patterns over time

## Usage

### Basic Access

```csharp
// Access the singleton instance
var mapQuerier = MapQuerier.Instance;

// Check if it's initialized, for safety
if (mapQuerier.IsInitialized)
{
    // Use the querier
}
```

### In Brain Implementation

```csharp
public class MyTacticalBrain : Brain
{
    protected override void ExecuteThink()
    {
        var mapQuerier = MapQuerier.Instance;
        
        if (!mapQuerier.IsInitialized)
        {
            // Fallback behavior
            MoveRandomly();
            return;
        }

        // For example, assess current position risk
        var deathsNearby = mapQuerier.GetDeathsInRadius(Agent.X, Agent.Y, 3);
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

#### GetMostDenseDeathAreas(int radius = 2, int topCount = 10)
Returns the most dangerous areas on the map based on death density.

```csharp
var dangerousAreas = mapQuerier.GetMostDenseDeathAreas(radius: 3, topCount: 5);
foreach (var area in dangerousAreas)
{
    Console.WriteLine($"Danger zone at ({area.X}, {area.Y}): {area.DeathCount} deaths");
}
```

#### GetDeathsInRadius(int centerX, int centerY, int radius)
Gets all deaths within a specified radius of a coordinate.

```csharp
var nearbyDeaths = mapQuerier.GetDeathsInRadius(agent.X, agent.Y, 2);
var riskLevel = nearbyDeaths.Count > 5 ? "HIGH" : "LOW";
```

#### GetDeathsAtCoordinate(int x, int y)
Gets all deaths at a specific coordinate.

```csharp
var deathsHere = mapQuerier.GetDeathsAtCoordinate(x, y);
if (deathsHere.Count > 0)
{
    Console.WriteLine($"Warning: {deathsHere.Count} agents have died at this location!");
}
```

#### GetDeathsByTeam(int team)
Gets death statistics for a specific team.

```csharp
var teamDeaths = mapQuerier.GetDeathsByTeam(1);
Console.WriteLine($"Team 1 has lost {teamDeaths.Count} agents");
```

#### GetDeathHeatmap(int cellSize = 3)
Generates a 2D array representing death density across the map.

```csharp
var heatmap = mapQuerier.GetDeathHeatmap(cellSize: 5);
// Use heatmap for visualization or analysis
```

### Analysis Tools

The `MapAnalyzer` static class provides high-level analysis tools:

#### AnalyzeDeathPatterns(MapQuerier mapQuerier)
Generates a comprehensive analysis report of death patterns.

```csharp
var analysis = MapAnalyzer.AnalyzeDeathPatterns(MapQuerier.Instance);
Console.WriteLine(analysis);
```

#### GetPositionRiskAssessment(MapQuerier mapQuerier, int x, int y, int radius = 3)
Provides detailed risk assessment for a specific position.

```csharp
var riskReport = MapAnalyzer.GetPositionRiskAssessment(mapQuerier, x, y);
Console.WriteLine(riskReport);
```

#### GenerateTextHeatmap(MapQuerier mapQuerier, int cellSize = 5)
Creates a text-based visualization of the death heatmap.

```csharp
var textHeatmap = MapAnalyzer.GenerateTextHeatmap(mapQuerier);
Console.WriteLine(textHeatmap);
```

## Data Structures

### DeathLocation
Represents a single death event:
```csharp
public class DeathLocation
{
    public int X { get; }           // X coordinate
    public int Y { get; }           // Y coordinate
    public DateTime Timestamp { get; } // When the death occurred
    public string AgentName { get; }   // Name of the agent that died
    public int Team { get; }           // Team of the agent
    public int Step { get; }           // Simulation step when death occurred
}
```

### DeathDensityArea
Represents an area with calculated death density:
```csharp
public class DeathDensityArea
{
    public int X { get; }                    // Center X coordinate
    public int Y { get; }                    // Center Y coordinate
    public int DeathCount { get; }           // Number of deaths in this area
    public double DensityScore { get; }      // Calculated density score
    public List<DeathLocation> Deaths { get; } // Individual death records
}
```

## Integration with Simulation

The MapQuerier is automatically initialized when a Simulation is created and automatically tracks deaths through the event system. No manual setup is required.

```csharp
// Create simulation - MapQuerier is automatically initialized
var simulation = new Simulation(width: 50, height: 50);

// MapQuerier is now ready to use
var mapQuerier = MapQuerier.Instance;
```

If you want to implement your own simulation, you will need to make sure to call `MapQuerier.Initialize` 
and subscribe to the `OnAgentKilled` event to track deaths.

## Example: TacticalBrain

The included `TacticalBrain` class demonstrates how to use the MapQuerier for strategic decision making:

- **Risk Assessment**: Evaluates the danger level of the current position
- **Safe Movement**: Moves to areas with lower death density when in danger
- **Opportunity Seeking**: Seeks areas with moderate activity when safe
- **Danger Avoidance**: Filters out high-risk moves when moving randomly

## Recommendations for Use

1. **Always Check Initialization**: Check `mapQuerier.IsInitialized` before using
2. **Use The Appropriate Radius**: A larger radius gives broader context, a smaller radius gives precise local information
5. **Thread Safety**: The MapQuerier is thread-safe, but consider performance implications of frequent queries 
(if that matters to you)

## Performance Considerations

- The MapQuerier uses locks for thread safety, so avoid excessive querying in tight loops
- Consider caching results of expensive queries like `GetMostDenseDeathAreas`
- The heatmap generation can be expensive for large maps with many deaths

## Future Enhancements

Potential future features that you can implement could include:
- Time-weighted death analysis (recent deaths more important?)
- Team-specific risk analysis
- Weapon-specific death tracking
- Predictive death probability modeling
- Integration with pathfinding for safe route planning
- Other non-death events tracking (e.g., agent sightings, weapon pickups, etc.)