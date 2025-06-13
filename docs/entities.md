---
title: Entities
---

# Entities

Entities are objects that exist within the game world. They can be used to represent anything from a player, 
a building, a tree or even a piece of furniture. They have their own position and orientation on the map.

# Index

- [Agents](#agents)
- [Creating Entities](#creating-entities)
- [Creating Agents](#creating-agents)
- [Extending Entities](#extending-entities)

## Agents

Agents are the main actors in SimArena. The main difference between agents and entities is that agents have an AI 
system (see [brain](brains.md)) which allows them to make decisions based on their surroundings.

## Creating Entities

To create a new entity, you can use the `Entity` class.

```csharp
var entity = new Entity(1, 1);
```

This will create a new entity at position (1, 1). You can then add it to the simulation using the `AddEntity` method.

```csharp
simulation.AddEntity(entity);
```

## Creating Agents

To create a new agent, you can use the `Agent` class.

```csharp
var agent = new Agent(1, 1, new RandomBrain());
```

This will create a new agent at position (1, 1) with a random brain. You can then add it to the simulation using the `AddAgent` method.

```csharp
simulation.AddAgent(agent);
```

## Extending Entities

You can extend the `Entity` class to create your own custom entities. 
For example, you could create a `Tree` entity that has a health value and a method to damage it:

```csharp
public class Tree : Entity
{
    public int Health { get; private set; }
    
    public Tree(int x, int y, int health) : base(x, y)
    {
        Health = health;
    }
    
    public void Damage(int amount)
    {
        Health -= amount;
    }
}
```

You can then add it to the simulation like any other entity.

```csharp
var tree = new Tree(1, 1, 100);
simulation.AddEntity(tree);
```

You can then get the tree from the simulation and damage it in another part of your code.

```csharp
var tree = simulation.GetEntity<Tree>(guid);
tree.Damage(10);
```



