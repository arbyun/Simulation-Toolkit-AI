---
title: Brains
---

# Brains

Brains are the decision-making entities in SimArena. They are responsible for deciding what 
actions to take in response to the game state. Brains are attached to [agents](agents.md).

# Index

- [Brain Types](#brain-types)
- [Brain Creation](#brain-creation)
- [Using Brains](#using-brains)
- [Brain Configuration](#brain-configuration)

## Brain Types

Brains can be of different types, each with its own set of parameters and behavior. We provide two example brain types, 
`RandomBrain` and `SimpleBrain`. Brains are intended to be extended by users to create custom brains, according to their needs.

## Brain Creation

To create a new brain instance, your brain class must inherit from `Brain` and implement the correct constructor signatures:

```csharp
public class MyCustomBrain : Brain {
    public MyCustomBrain(Agent agent, IMap map, int team, int tickIntervalMs = 500) 
            : base(agent, map, team, tickIntervalMs)
        {}
        
        public MyCustomBrain(IMap map, int team, int tickIntervalMs = 500) 
            : base(map, team, tickIntervalMs)
        {}
}
```

It's recommended to also pass a `Simulation` instance to the constructor, so you have access to any data stored there.

The brain should use its constructor to receive all the information it will need to make its decisions. 

The brain logic listened by the simulation is defined inside the `ExecuteThink` method.

```csharp
protected override void ExecuteThink()
{
    // Your brain logic goes here!
}
```

This method will be called every time the simulation updates, through the method `Think`.

```csharp
public virtual void Think()
{
    if (!_initialized)
    {
        throw new Exception("Brain not initialized.");
    }
    
    // In fast mode, always execute thinking logic without time checks
    if (FastMode)
    {
        ExecuteThink();
        return;
    }
    
    // In normal mode, check if tick interval has passed since last time we made a decision
    if ((DateTime.UtcNow - _lastDecisionTime).TotalMilliseconds < _tickIntervalMs)
        return;
    
    // If so then continue to this
    _lastDecisionTime = DateTime.UtcNow;
    
    // Execute brain-specific logic
    ExecuteThink();
}
```

`Think` can be overridden to add additional functionality before or after calling `ExecuteThink`, such as logging or other 
bookkeeping tasks. Full overrides of `Think` that remove other functionality is not recommended because it may interfere 
with the simulation's internal timing mechanisms.

## Using Brains

Once you've created a brain, you can attach it to an agent using the `SetBrain` method on the agent object.

For example:
```csharp
var myAgent = new Agent(1);
myAgent.SetBrain(new MyCustomBrain(myAgent, map, 1));
```

Or alternatively, when creating the agent itself:

```csharp
var myAgent = new Agent(2, new MyCustomBrain(map, 1));
```

You can also change the brain at runtime by simply setting a new one on the agent.

```csharp
myAgent.SetBrain(new AnotherBrain(myAgent, map, 1));
```

You can also choose to create the agent and brain separately first, and then assign the brain later. In that case, after creating the brain,
you should assign it to the agent like this:

```csharp
var myAgent = new Agent(3);
var myBrain = new MyCustomBrain();
myAgent.SetBrain(myBrain);
myBrain.SetAgent(myAgent);
```

Note that the brain must be assigned to the agent before it can start making decisions.

## Brain Configuration

Brains can be configured via JSON files. To set up configuration for your brain, see the [brain configuration documentation](custom-brains.md).