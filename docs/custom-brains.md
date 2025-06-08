---
title: Custom Brains
---

# Creating Custom Brains

This page explains how to create new Brain types in SimArena that are compatible with JSON serialization.

## Overview

The Brain system in SimArena uses a combination of:
1. The base `Brain` abstract class for runtime logic
2. Concrete brain implementations (e.g., `RandomBrain`, `ChaserBrain`)
3. Serializable configuration classes (`BrainConfiguration` hierarchy)

This architecture separates the runtime behavior from serialization concerns, making it easier to create new brain types.

## Step 1: Create a Brain implementation

Create a new class that inherits from the `Brain` abstract class:

```csharp
namespace SimArena.Brains
{
    public class YourNewBrain : Brain
    {
        // Add specific properties for your brain type
        private bool _someProperty;
        
        // Constructor that takes an agent
        public YourNewBrain(Agent agent, IMap map, int team, bool someProperty, int tickIntervalMs = 500) 
            : base(agent, map, team, tickIntervalMs)
        {
            _someProperty = someProperty;
        }
        
        // Constructor without agent (used for deferred initialization)
        public YourNewBrain(IMap map, int team, bool someProperty, int tickIntervalMs = 500) 
            : base(map, team, tickIntervalMs)
        {
            _someProperty = someProperty;
        }

        // Implement the abstract method to define brain behavior
        protected override void ExecuteThink()
        {
            // Implement your brain's decision-making logic here
            // You can use:
            // - MoveTo(x, y) to move the agent
            // - Agent to access the current agent
            // - _map to access the game map
        }
        
        // Add any additional methods needed for your brain's behavior
    }
}
```

## Step 2: Create a BrainConfiguration for your brain

Add a new configuration class that inherits from `BrainConfiguration`.
Don't forget to add the necessary JSON serialization attributes, usually given by the template 
`[JsonDerivedType(typeof(YourBrainConfiguration), "YourBrainTypeName")]`.

```csharp
namespace SimArena.Serialization.Configuration
{
    [Serializable]
    [JsonDerivedType(typeof(YourNewBrainConfiguration), "YourNewBrain")]
    public class YourNewBrainConfiguration : BrainConfiguration, IJsonOnDeserialized // Implement IJsonOnDeserialized if needed
    {
        // Add serializable properties specific to your brain
        public bool SomeProperty { get; set; }
        
        // Constructor with parameters
        public YourNewBrainConfiguration(int team = 0, bool someProperty = false, int tickIntervalMs = 500, int awareness = 10) 
            : base("YourNewBrain", team, tickIntervalMs, awareness)
        {
            SomeProperty = someProperty;
        }
        
        // Parameter-less constructor for JSON deserialization
        public YourNewBrainConfiguration() : base("YourNewBrain") { }
        
        // Implement if using IJsonOnDeserialized
        public void OnDeserialized()
        {
            // Validate deserialized properties or apply business rules
            // e.g., ensure values are within valid ranges
        }
        
        // Factory method to create the actual brain instance
        public override Brain CreateBrain(Agent agent, IMap map, Simulation simulation = null)
        {
            var brain = new YourNewBrain(map, Team, SomeProperty, TickIntervalMs);
            if (agent != null)
            {
                brain.SetAgent(agent);
            }
            return brain;
        }
    }
}
```

## Step 3: Register your brain type for polymorphic serialization

Update the `BrainConfiguration` base class to include your new type:

```csharp
// in BrainConfiguration.cs
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(RandomBrainConfiguration), "RandomBrain")]
[JsonDerivedType(typeof(ChaserBrainConfiguration), "ChaserBrain")]
[JsonDerivedType(typeof(YourNewBrainConfiguration), "YourNewBrain")] // Add this line
public class BrainConfiguration
{
    // ... existing code ...
}
```

## Usage Examples

### Creating a Brain Configuration

```csharp
// Create a configuration for your new brain
var brainConfig = new YourNewBrainConfiguration(
    team: 1, 
    someProperty: true, 
    tickIntervalMs: 300
);
```

### Serializing a Brain Configuration

```csharp
// Serialize to JSON
string json = JsonSerializer.Serialize(brainConfig);
```

Example JSON output:
```json
{
  "Type": "YourNewBrain", 
  "SomeProperty": true,
  "BrainTypeName": "YourNewBrain", 
  "Awareness": 10,
  "TickIntervalMs": 300,
  "Team": 1
}
```

### Deserializing a Brain Configuration

```csharp
// Deserialize from JSON
var loadedConfig = JsonSerializer.Deserialize<BrainConfiguration>(json);

// You can also check the type and cast if needed
if (loadedConfig is YourNewBrainConfiguration yourConfig)
{
    // Access specific properties
    bool someProp = yourConfig.SomeProperty;
}
```

### Creating a Brain from Configuration

```csharp
// Create an actual brain instance from configuration
var agent = /* get or create agent */;
var map = /* get or create map */;
var simulation = /* get simulation instance */;

Brain brain = brainConfig.CreateBrain(agent, map, simulation);
```

## Best Practices

2. **Provide Default Values**: Always provide sensible defaults for properties to ensure configurations work even with minimal initialization.
3. **Validate on Deserialization**: Implement `IJsonOnDeserialized` and its `OnDeserialized` method to validate values after deserialization.
4. **Document Properties**: Use XML comments to document what each property does and any valid value ranges.
5. **Handle Nulls**: Check for null references, especially in the `CreateBrain` method.
6. **Keep Constructors Consistent**: Follow the pattern of providing both agent and non-agent constructors for flexibility.
7. **Avoid Serializing Complex Types**: Properties like `Agent` or `IMap` should not be part of the serialization; they should be provided at runtime.

## Fun Fact: How the JSON Polymorphism Works

The System.Text.Json serialization uses a "type discriminator" property to determine which concrete class to deserialize JSON into. In our case:

1. `[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]` on the base class specifies the property name that will contain the type information.
2. `[JsonDerivedType(typeof(YourNewBrainConfiguration), "YourNewBrain")]` registers a derived class with a specific type name.
3. When serializing, the system automatically adds the "Type" property with the appropriate value.
4. When deserializing, the system uses the "Type" property to determine which class to instantiate.

This approach allows for easy extensibility without needing to modify enums (like in the objective example) or other central code.