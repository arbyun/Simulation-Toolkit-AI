---
title: Custom Map Analysis
---

# Custom Map Analysis

The Map Analysis System is designed to be easily extended to track other events on the map and provide 
analysis tools for those events as well.

This guide will walk you through the process of creating a custom [map analyzer](map-analysis.md) for tracking 
and analyzing custom events on the map.

# Index
- [Creating Custom Analyzers](#creating-custom-analyzers)
- [Registering Custom Analyzers](#registering-custom-analyzers)
- [Accessing Custom Analyzers](#accessing-custom-analyzers)
- [Example: Tracking Agent Sightings](#example-tracking-agent-sightings)

## Creating Custom Analyzers

To create a custom analyzer, implement the `IMapAnalysis<TData>` interface. The `TData` type represents the 
type of data you want to track.

```csharp
public class CustomAnalysis : IMapAnalysis<CustomData>
{
    // Implement interface methods
    public override string AnalysisType => "Custom";
}
```

The `CustomData` type should implement the `IPositionalData` interface to provide position and timestamp information.

```csharp
public class CustomData : IPositionalData
{
    public int X { get; set; }
    public int Y { get; set; }
    public DateTime Timestamp { get; set; }
    public int Step { get; set; }
}
```

## Registering Custom Analyzers

To have your custom analyzer automatically created and initialized when a simulation starts, register it with the `Simulation` class.

```csharp
public class MySimulation : Simulation
{
    public MySimulation()
    {
        // Register the custom analyzer
        
        if (Map != null)
        {
            var customAnalysis = new CustomAnalysis();
            customAnalysis.Initialize(Map.Width, Map.Height);
            MapAnalyzers.Add(customAnalysis);
            
            // Subscribe to relevant events
            // For example: (CustomEvent does not exist)
            Events.CustomEvent += OnCustomEvent;
        }
    }
    
    private void OnCustomEvent(object sender, CustomEventArgs e)
    {
        var customAnalysis = GetMapAnalyzer<CustomAnalysis>();
        if (customAnalysis != null)
        {
            var customData = new CustomData { X = e.X, Y = e.Y, Timestamp = DateTime.Now, Step = CurrentStep };
            customAnalysis.RecordData(customData);
        }
    }
}
```

## Accessing Custom Analyzers

You can access your custom analyzer in the same way as the built-in analyzers.

```csharp
var customAnalysis = simulation.GetMapAnalyzer<CustomAnalysis>();
```

or by overriding the `AnalysisType` property and giving it a unique name:

```csharp
var customAnalysis = simulation.GetMapAnalyzer("Custom");
```

## Example: Tracking Agent Sightings

Let's say you want to track where agents have been sighted on the map.

```csharp
public class SightingData : IPositionalData
{
    public int X { get; set; }
    public int Y { get; set; }
    public DateTime Timestamp { get; set; }
    public int Step { get; set; }
    public string AgentName { get; set; }
}

public class SightingAnalysis : IMapAnalysis<SightingData>
{
    // Implement interface methods
    public override string AnalysisType => "Sightings";
}
```

Register it with the simulation:

```csharp
public class MySimulation : Simulation
{
    public MySimulation()
    {
        // Register the sighting analyzer
        if (Map != null)
        {
            var sightingAnalysis = new SightingAnalysis();
            sightingAnalysis.Initialize(Map.Width, Map.Height);
            MapAnalyzers.Add(sightingAnalysis);
            
            // Subscribe to relevant events
            // For example: (OnAgentSighted does not exist)
            Events.OnAgentSighted += OnAgentSighted;
        }
    }
    
    private void OnAgentSighted(object sender, AgentSightedEventArgs e)
    {
        var sightingAnalysis = GetMapAnalyzer<SightingAnalysis>();
        if (sightingAnalysis != null)
        {
            var sightingData = new SightingData { X = e.X, Y = e.Y, Timestamp = DateTime.Now, Step = CurrentStep, AgentName = e.AgentName };
            sightingAnalysis.RecordData(sightingData);
        }
    }
}
```

And access it in your code:

```csharp
var sightingAnalysis = simulation.GetMapAnalyzer<SightingAnalysis>();
```
