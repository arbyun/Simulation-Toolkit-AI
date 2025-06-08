---
title: Custom Weapons
---

# Creating Custom Weapons

This page will walk you through the process of creating a complete custom weapon for SimArena, 
including all necessary components, and how to work with weapon configurations in SimArena using 
the polymorphic JSON serialization system.

## Overview

The weapon system in SimArena supports two main types of weapons:

1. **Melee Weapons**: Close-range weapons with attributes like attack speed and area effects
2. **Ranged Weapons**: Distance weapons with attributes like range, projectile speed, and fire rate

Both types share common properties like weapon ID and damage, but have type-specific properties as well.

You may, of course, extend the weapon system to support more types of weapons. For that, you will have to also 
extend the weapon configuration system. Below we will focus on that use case.

## Existing Weapons | Creating Weapon Configurations

### Creating a Melee Weapon

```csharp
var swordConfig = new MeleeWeaponConfiguration(
    weaponId: "sword_01", 
    damage: 15
);
```

### Creating a Ranged Weapon

```csharp
var bowConfig = new RangedWeaponConfiguration(
    weaponId: "bow_01",
    damage: 12,
    range: 8.0f,
    projectileSpeed: 15.0f,
    fireRate: 0.8f
);
```

## Serializing Weapons

You can serialize weapon configurations to JSON for storage.

Example JSON output for a melee weapon:
```json
{
  "Type": "Melee",
  "WeaponId": "sword_01",
  "WeaponType": "Melee",
  "Damage": 15
}
```

Example JSON output for a ranged weapon:
```json
{
  "Type": "Ranged",
  "Range": 8.0,
  "ProjectileSpeed": 15.0,
  "FireRate": 0.8,
  "WeaponId": "bow_01",
  "WeaponType": "Ranged",
  "Damage": 12
}
```

## Step 1: Create a Custom Weapon Type

Create a new class that inherist from the abstract `Weapon` class:

```csharp
public class CustomWeapon : Weapon
{
    // Custom weapon logic here
}
```

## Step 2: Create a Configuration for Your Custom Weapon

Create a new class that inherits from the abstract `WeaponConfiguration` class.
Don't forget to add the necessary JSON serialization attributes, usually given by the template 
`[JsonDerivedType(typeof(YourWeaponConfig), "YourWeaponConfig")]`.

```csharp
[JsonDerivedType(typeof(CustomWeaponConfiguration), "CustomWeapon")]
public class CustomWeaponConfiguration : WeaponConfiguration, IJsonOnDeserialized
{
    public float AreaOfEffect { get; set; }
    
    // Parameter-less constructor for JSON deserialization
    public CustomWeaponConfiguration() : base("CustomWeapon") { }
    
    // Constructor with parameters
    public CustomWeaponConfiguration(string weaponId, int damage, float areaOfEffect) 
        : base("CustomWeapon", weaponId, damage)
    {
        AreaOfEffect = areaOfEffect;
    }
    
    public void OnDeserialized()
    {
        // Any validation or post-deserialization logic can go here
    }
}
```

## Step 3: Register Your Weapon Type for JSON Serialization

Update the `WeaponConfiguration` base class to include your new type:

```csharp
// in WeaponConfiguration.cs
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(MeleeWeaponConfiguration), "Melee")]
[JsonDerivedType(typeof(RangedWeaponConfiguration), "Ranged")]
[JsonDerivedType(typeof(CustomWeaponConfiguration), "CustomWeapon")] // Add this line
public class WeaponConfiguration
{
    // ... existing code ...
}
```

## Common Properties

All weapons share these common properties:

- **WeaponId**: Unique identifier for the weapon
- **Damage**: Base damage value

## Ranged Weapon Properties

Ranged weapons have these specific properties:

- **Range**: Maximum attack range in units
- **ProjectileSpeed**: How fast projectiles travel
- **FireRate**: Rate of fire in shots per second

## Best Practices

1. **Use Descriptive IDs**: Make weapon IDs descriptive and unique (e.g., "fire_sword_01" rather than just "sword")
2. **Validate on Load**: When loading weapons from external sources, validate their properties to ensure they're within acceptable ranges
