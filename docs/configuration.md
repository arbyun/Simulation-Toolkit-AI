---
title: Configuration
---

# Configurations

**SimArena** uses JSON for all configuration files. The option to use XML is available, but samples and 
documentation will only be provided for JSON.

# Index

- [Purpose](#purpose)
- [Game Configuration](#game-configuration)
- [Objective Configuration](#objective-configuration)
- [Agent Configuration](#agent-configuration)
- [Brain Configuration](#brain-configuration)
- [Weapon Configuration](#weapon-configuration)
- [Template Configuration](#template-configuration)

## Purpose

Configurations are files that define the settings for a simulation. They include the map, the objective, 
and the agents. Configurations can be created manually through JSON or XML files, 
or at runtime using the `GameConfiguration` class.

## Game Configuration

The `GameConfiguration` class is the root of all configurations. It contains the following properties:

- `Name`: The name of the match
- `MapPath`: The path to the map file
- `RealtimeMode`: Whether to run the simulation in realtime mode
- `Objective`: The objective configuration
- `Agents`: The list of agent configurations
- `AgentTemplates`: The list of agent template references (alternative to direct agent configurations)
- `Weapons`: The list of weapon configurations

## Objective Configuration

Configures an [objective](objectives.md) for the simulation.

The `ObjectiveConfiguration` class contains the following properties:

- `TypeEnum`: The type of objective, used to determine which objective tracker to create

The class is not meant to be used directly, but rather as a base class for specific objective configurations. 
To see how to create a custom objective configuration, see the [Custom Objectives](custom-objectives.md) documentation.

## Agent Configuration

Configures an [agent](entities.md#agents) for the simulation.

The `AgentConfiguration` class contains the following properties:

- `Name`: The name of the agent
- `Brain`: The brain configuration
- `OwnedWeaponIds`: The list of weapon IDs that the agent owns
- `RandomStart`: Whether to randomly generate the agent's starting position; if false, the `StartX` and `StartY` properties are used
- `StartX`: The agent's starting X position
- `StartY`: The agent's starting Y position
- `MaxHealth`: The agent's maximum health
- `AttackPower`: The agent's attack power
- `Defense`: The agent's defense
- `Speed`: The agent's speed
- `Team`: The agent's team number

## Brain Configuration

Configures a [brain](brains.md) for an agent.

The `BrainConfiguration` class contains the following properties:

- `BrainTypeName`: The name of the brain type (for serialization purposes)
- `Awareness`: The brain's awareness
- `TickIntervalMs`: The brain's tick interval in milliseconds
- `Team`: The brain's team number (can be ignored safely to let the simulation randomly assign it)

The class is not meant to be used directly, but rather as a base class for specific brain configurations. 
To see how to create a custom brain configuration, see the [Custom Brains](custom-brains.md) documentation.

## Weapon Configuration

Configures a [weapon](weapons.md) for an agent.

The `WeaponConfiguration` class contains the following properties:

- `WeaponId`: The weapon's ID
- `WeaponType`: The weapon's type (for serialization purposes)
- `Damage`: The weapon's damage

The class is not meant to be used directly, but rather as a base class for specific weapon configurations. For 
example, the `MeleeWeaponConfiguration` class, which has no additional properties.

For an example of a weapon configuration with additional properties, see the 
`RangedWeaponConfiguration` class, which contains the following additional properties:

- `Range`: The weapon's range
- `ProjectileSpeed`: The weapon's projectile speed
- `FireRate`: The weapon's fire rate

To see how to create a custom weapon configuration, see the [Custom Weapons](custom-weapons.md) documentation.

## Template Configuration

Instead of directly configuring agents, you can use templates. Templates are JSON files that define a set of 
default values for an agent. They can be extended and overridden when used in a game configuration.

The `AgentTemplate` class contains the following properties:

- `TemplateId`: The template's ID
- `Description`: The template's description
- `Version`: The template's version
- `Author`: The template's author
- `CreatedDate`: The template's creation date
- `Tags`: The template's tags

All these properties, except `TemplateId`, are optional and are used for documentation/organization purposes.

From there, all the other properties are the same as the `AgentConfiguration` class.

### Using Templates

To use a template in a game configuration, you use the `AgentTemplates` property. Each entry in this list is a 
`TemplateReference` object, which contains the following properties:

- `TemplatePath`: The path to the template file or the template's ID
- `Name`: The name to give the instantiated agent (optional, defaults to the template's name)
- `Overrides`: A dictionary of property paths and values to override in the template

The property paths use dot notation to navigate the object hierarchy. For example, to override the brain's team, 
you would use the path `Brain.Team`.

Here is an example of a template reference:

```json
{
  "TemplatePath": "human_soldier",
  "Name": "Red Team Alpha",
  "Overrides": {
    "Brain.Team": 0
  }
}
```

And here is an example of how to use a template reference in a game configuration:

```json
{
  "Name": "Quick Skirmish - 2v2",
  "RealtimeMode": false,
  "Objective": {
    "Type": "DeathmatchObjective",
    "ObjectiveType": "TeamDeathmatch",
    "TeamCount": 2,
    "KillsToWin": 3
  },
  "Agents": [],
  "AgentTemplates": [
    {
      "TemplatePath": "human_soldier",
      "Name": "Red Team Alpha",
      "Overrides": {
        "Brain.Team": 0
      }
    },
    {
      "TemplatePath": "human_sniper",
      "Name": "Red Team Bravo",
      "Overrides": {
        "Brain.Team": 0
      }
    },
    {
      "TemplatePath": "ai_scout",
      "Name": "Blue Team Charlie",
      "Overrides": {
        "Brain.Team": 1
      }
    },
    {
      "TemplatePath": "human_soldier",
      "Name": "Blue Team Delta",
      "Overrides": {
        "Brain.Team": 1
      }
    }
  ]
}
```

This offers a lot of flexibility in terms of creating complex agent configurations with minimal repetition.

For instance, without templates, the above configuration would have to be written as follows:

```json
{
  "Name": "Quick Skirmish - 2v2",
  "RealtimeMode": false,
  "Objective": {
    "Type": "DeathmatchObjective",
    "ObjectiveType": "TeamDeathmatch",
    "TeamCount": 2,
    "KillsToWin": 3
  },
  "Agents": [
    {
      "Name": "Red Team Alpha",
      "Brain": {
        "Type": "RandomBrain",
        "BrainTypeName": "RandomBrain",
        "Awareness": 12,
        "TickIntervalMs": 400,
        "Team": 0
      },
      "OwnedWeaponIds": [
        "assault_rifle",
        "combat_knife"
      ],
      "RandomStart": true,
      "StartX": 0,
      "StartY": 0,
      "MaxHealth": 100,
      "AttackPower": 15,
      "Defense": 8,
      "Speed": 1.0
    },
    {
      "Name": "Red Team Bravo",
      "Brain": {
        "Type": "ChaserBrain",
        "BrainTypeName": "ChaserBrain",
        "Awareness": 20,
        "TickIntervalMs": 600,
        "Team": 0
      },
      "OwnedWeaponIds": [
        "sniper_rifle",
        "pistol"
      ],
      "RandomStart": true,
      "StartX": 0,
      "StartY": 0,
      "MaxHealth": 75,
      "AttackPower": 35,
      "Defense": 3,
      "Speed": 0.8
    }
    // ... and so on for the other agents ...
  ]
}
```

To see examples of templates, see the templates provided in the `templates` folder of the repository.