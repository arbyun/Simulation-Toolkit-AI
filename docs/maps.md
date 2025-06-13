---
title: Maps
---

# Maps

SimArena's map system is powered by the [RogueSharp](https://github.com/FaronBracy/RogueSharp) library. It provides a flexible map system that supports 
different types of map configurations and templates for creating diverse combat environments.

# Index

- [Map Types](#map-types)
  - [Simple Maps](#simple-maps)
  - [Procedurally Generated Maps](#procedurally-generated-maps)
    - [Random Rooms](#random-rooms)
    - [Border Only](#border-only)
    - [Cave Map](#cave-map)
    - [String](#string)
  - [File-Based Maps](#file-based-maps)
  - [Map Templates](#map-templates)
    - [Built-in Map Templates](#built-in-map-templates)

## Map Types

### Simple Maps
Simple maps are basic rectangular areas with all cells walkable and transparent. They're perfect for open combat scenarios
and only need their width and height specified for them to immediately work out of the box.

### Procedurally Generated Maps
Procedural maps use algorithms to generate complex layouts with rooms, corridors, and obstacles.

#### Random Rooms
Creates a map with randomly placed rooms connected by corridors. See 
[RandomRoomsMapCreationStrategy](https://faronbracy.github.io/RogueSharp/api/RogueSharp.MapCreation.RandomRoomsMapCreationStrategy-1.html) 
for more information on how these are generated.

#### Border Only
Creates a map with walls only around the border, creating a fortress-like environment. See [BorderOnlyMapCreationStrategy](https://faronbracy.github.io/RogueSharp/api/RogueSharp.MapCreation.BorderOnlyMapCreationStrategy-1.html) 
for more information on how these are generated.

#### Cave Map
Creates a map using cellular automata. See
[CaveMapCreationStrategy](https://faronbracy.github.io/RogueSharp/api/RogueSharp.MapCreation.CaveMapCreationStrategy-1.html) 
for more information on how these are generated.

#### String
Creates a map from a string representation. 

The following symbols represent cells on the map:

- `.`: Cell is transparent and walkable
- `s`: Cell is walkable (but not transparent)
- `o`: Cell is transparent (but not walkable)
- `#`: Cell is not transparent or walkable

See
[StringMapCreationStrategy](https://faronbracy.github.io/RogueSharp/api/RogueSharp.MapCreation.StringMapCreationStrategy-1.html) 
for more information on how these are generated.

### File-Based Maps

Maps loaded from text files with custom layouts. The format is described in detail in [File Based Maps](custom-maps.md#creating-file-based-maps).

## Map Templates

Map templates allow you to create reusable map configurations that can be shared across different game scenarios. 
See [Custom Maps](custom-maps.md) for more information and examples on how to implement your own.

### Built-in Map Templates

SimArena comes with several pre-built map templates:

#### Small Arena (`small_arena`)
- **Size**: 20x20
- **Type**: Simple
- **Best for**: Quick skirmishes, close-quarters combat
- **Tags**: small, arena, combat, quick

#### Medium Battlefield (`medium_battlefield`)
- **Size**: 30x30
- **Type**: Simple
- **Best for**: Team-based combat, tactical positioning
- **Tags**: medium, battlefield, tactical, team

#### Large Warzone (`large_warzone`)
- **Size**: 50x50
- **Type**: Simple
- **Best for**: Long-range combat
- **Tags**: large, warzone, epic, long-range

#### Urban Complex (`urban_complex`)
- **Size**: 40x40
- **Type**: Procedural (Random Rooms)
- **Best for**: Tactical combat, room clearing
- **Tags**: urban, complex, rooms, tactical, procedural

#### Fortress Siege (`fortress_siege`)
- **Size**: 35x35
- **Type**: Procedural (Border Only)
- **Best for**: Siege scenarios, defensive combat
- **Tags**: fortress, siege, border, defensive

#### Training Ground (`training_ground`)
- **Size**: 15x15
- **Type**: Simple
- **Best for**: AI testing, weapon effectiveness testing
- **Tags**: training, testing, compact, ai

