---
title: Weapons
---

# Weapons

Weapons are the tools agents use to attack each other. They are attached to [agents](agents.md) and can be used to deal damage to other agents.

# Index

- [Weapon Types](#weapon-types)
  - [Melee Weapons](#melee-weapons)
  - [Ranged Weapons](#ranged-weapons)
- [Weapon Creation](#weapon-creation)
- [Using Weapons](#using-weapons)
- [Weapon Configuration](#weapon-configuration)

## Weapon Types

Weapons can be of different types, each with its own set of parameters and behavior. We provide the following base 
weapon examples, which are meant to be extended:

- `MeleeWeapon`: A melee weapon that deals damage in close range.
- `RangedWeapon`: A ranged weapon that deals damage at a distance.

### Melee Weapons

The simplest weapon type is the melee weapon. It has no additional properties beyond the base `Weapon` class.

In the simulation, melee weapons are used to attack agents in close range. The attack is resolved by the weapon's 
`Attack` method, which takes a direction as a parameter. The direction is used to determine the target cell of the attack.

Melee weapons, by default, only have one cell of range, meaning that the agent must be adjacent to the target to attack it.

### Ranged Weapons

Ranged weapons have additional properties beyond the base `Weapon` class. They have a range, a projectile speed, and a fire rate.

In the simulation, ranged weapons are used to attack agents at a distance. 
The attack is resolved by the weapon's `Attack` method, which takes a direction as a parameter. 
It then creates a `Bullet` entity that moves in that given direction. The bullet deals damage to first agent it 
collides with within its range.

Ranged weapons have a cooldown, which is the time between shots. The cooldown is based on the fire rate, 
which is the number of shots per second. The cooldown is calculated as `1 / fireRate`.

## Weapon Creation

To create a new weapon instance, your weapon class must inherit from `Weapon` (or, preferentially, by one of its 
subclasses, either `MeleeWeapon` or `RangedWeapon` based on the intended behaviour) and implement the correct 
constructor signatures.

In the case of inheriting directly from `Weapon`, you must implement the `Attack` method:

```csharp
public class MyCustomWeapon : Weapon
{
    public MyCustomWeapon(int x, int y, Simulation simulation) : base(x, y, simulation)
    {
    }
    
    public MyCustomWeapon(int x, int y, Simulation simulation, Agent owner) : base(x, y, simulation, owner)
    {
    }
    
    public override bool Attack(Vector3 direction)
    {
        // Your attack logic goes here!
    }
}
```

In the case of inheriting from `MeleeWeapon` or `RangedWeapon`, you don't need to implement the `Attack` method, 
but you do need to call the base constructor with the appropriate parameters:

```csharp
public class MyCustomRangedWeapon: RangedWeapon
{
    public MyCustomRangedWeapon(int x, int y, Simulation simulation, float range, float projectileSpeed, float fireRate) : base(x, y, simulation, range, projectileSpeed, fireRate)
    {
    }
   
}
```

Of course, you can override the `Attack` method if you want to change the behavior of the weapon.

```csharp
public override bool Attack(Vector3 direction)
{
    // Your attack logic goes here!
    return true;
}
```

## Using Weapons

Weapons can be created directly at any time during the simulation:

```csharp
var weapon = new MeleeWeapon(0, 0, simulation);
```

Once you've created a weapon, you can attach it to an agent using the `Equip` method on the weapon object.

```csharp
weapon.Equip(agent);
```

You must also notify the agent that it has a new weapon by calling the `EquipWeapon` method on the agent object.

```csharp
agent.EquipWeapon(weapon);
``````

As weapons are entities, they can just be a static object in the simulation like any other entity. This is useful if 
you want to allow agents to pick up weapons from the environment in a custom brain.

## Weapon Configuration

Weapons can be configured via JSON files. To set up configuration for your custom weapon, see the 
[weapon configuration documentation](custom-weapons.md).
