---
title: Custom Maps
---

# Creating Custom Maps

This guide covers how to create custom [maps](maps.md) and map templates for SimArena.

# Index

- [Creating Map Templates](#creating-map-templates)
  - [Basic Template Structure](#basic-template-structure)
  - [Template Properties](#template-properties)
- [Map Configuration Types](#map-configuration-types)
  - [1. Simple Maps](#1-simple-maps)
  - [2. Procedural Maps with Random Rooms](#2-procedural-maps-with-random-rooms)
  - [3. Border-Only Maps](#3-border-only-maps)
  - [4. File-Based Maps](#4-file-based-maps)
    - [Supported Directives](#supported-directives)
    - [Example Map File Content](#example-map-file-content)
    - [Notes](#notes)
- [Creating File-Based Maps](#creating-file-based-maps)
- [Using Custom Maps](#using-custom-maps)
  - [In Game Configurations](#in-game-configurations)
  - [With Overrides](#with-overrides)
- [Design Guidelines](#design-guidelines)

## Creating Map Templates

Map templates are the recommended way to create reusable map configurations. They're stored as JSON files in the `templates/maps/` directory.

### Basic Template Structure

```json
{
  "TemplateId": "my_custom_map",
  "Name": "My Custom Map",
  "Description": "A custom map for specific scenarios",
  "Version": "1.0",
  "Tags": ["custom", "scenario"],
  "MapConfiguration": {
    // Map configuration goes here
  }
}
```

### Template Properties

- **TemplateId**: Unique identifier for the template (used in references)
- **Name**: Human-readable name for the template
- **Description**: Detailed description of the map's purpose and characteristics
- **Version**: Template version for backwards compatibility
- **Tags**: Array of tags for categorization and searching
- **MapConfiguration**: The actual map configuration object

## Map Configuration Types

### 1. Simple Maps

Perfect for open combat scenarios:

```json
{
  "MapConfiguration": 
  {
    "Type": "Simple",
    "Width": 40,
    "Height": 40
  }
}
```

### 2. Procedural Maps with Random Rooms

For dynamic, room-based combat:

```json
{
  "MapConfiguration": 
  {
    "Type": "ProcGen",
    "StrategyType": "RandomRooms",
    "Width": 50,
    "Height": 30,
    "MaxRooms": 20,
    "RoomMaxWidth": 6,
    "RoomMaxHeight": 4
  }
}
```

The available strategy types are:
- **Random Rooms**: Creates a map with randomly placed rooms connected by corridors
- **Border Only**: Creates a map with walls only around the border
- **Cave Map**: Creates a map using cellular automata
- **String**: Creates a map from a string representation

More can be implemented by implementing the `IMapCreationStrategy<Map>` interface and registering it with the 
polymorphic JSON serialization system.

### 3. File-Based Maps

For completely custom layouts:

```json
{
  "MapConfiguration": 
  {
    "Type": "File",
    "FilePath": "maps/my_custom_map.txt",
    "Width": 25,
    "Height": 25
  }
}
```

## Creating File-Based Maps

File-based maps use text files to define custom layouts. They use structured directives in plain text 
files to define walls, doors, spacing, and offsets. These maps will then be parsed and translated into playable 
maps through an instance of `FileBasedMapCreationStrategy<TMap>`.

### Supported Directives

Each line in the map file must begin with a `#` followed by a keyword. Empty lines and comments (`//`) are ignored.

#### Layout & Initialization
- `#width [int]`
  - Explicitly sets the map's width (optional if dimensions can be inferred).

- `#height [int]`
  - Explicitly sets the map's height (optional if dimensions can be inferred).

#### Map Features
- `#wall ([x1],[y1])`
  - Places a wall at a single coordinate.

- `#wall ([x1],[y1]) ([x2],[y2])`
  - Draws a straight wall (horizontal or vertical only) between two coordinates.

- `#wallrect ([x1],[y1]) ([x2],[y2])`
  - Draws a rectangle of walls defined by the top-left and bottom-right corners.

- `#door ([x],[y])`
  - Places a walkable, impassable door tile at the specified coordinate.

- `#window ([x],[y])`
  - Places a transparent, unwalkable tile.

### Example Map File Content

```
#wallrect (0,0) (35,4)
#door (35,3)
#wallrect (34,0) (39,6)
#door (34,3)
```

### Notes
- If `#width` and `#height` are omitted, they are inferred based on the furthest `#wallrect`, `#wall`, or `#door` coordinates.
- Coordinates must be in the format (x,y) with no spaces inside the parentheses.
- `#wallrect` draws the four outer edges of a rectangle.

## Using Custom Maps

### In Game Configurations

Reference your custom map template:

```json
{
  "Name": "Custom Battle",
  "MapTemplate": {
    "TemplatePath": "my_custom_map"
  },
  "RealtimeMode": false,
  "Objective": {
    "Type": "DeathmatchObjective",
    "ObjectiveType": "TeamDeathmatch",
    "TeamCount": 2,
    "KillsToWin": 5
  }
}
```

### With Overrides

Customize template properties:

```json
{
  "Name": "Large Custom Battle",
  "MapTemplate": {
    "TemplatePath": "office_building",
    "Overrides": {
      "Width": 80,
      "Height": 60,
      "MaxRooms": 40,
      "RoomMaxWidth": 10,
      "RoomMaxHeight": 8
    }
  }
}
```

## Helpful Guidelines

### Map Size Recommendations

- **Small (15-25)**: 2-4 agents, quick battles
- **Medium (25-40)**: 4-8 agents, tactical combat
- **Large (40-60)**: 8+ agents, strategic battles
- **Extra Large (60+)**: Large-scale warfare

### Room Configuration Guidelines

For RandomRooms maps:
- **MaxRooms**: 0.5-1.0 rooms per 100 square units
- **RoomMaxWidth/Height**: 15-25% of map dimensions
- **Minimum room size**: 3x3 for meaningful combat

## Advanced Techniques

### Conditional Map Selection

Use tags to select appropriate maps:

```csharp
var urbanMaps = templateManager.GetMapTemplatesByTag("urban");
var selectedMap = urbanMaps.First();
```

### Dynamic Map Modification

Modify maps at runtime:

```csharp
var map = mapConfiguration.CreateMap();
// Add dynamic obstacles
map.SetCellProperties(x, y, false, false); // Make cell impassable
```

### Custom Map Strategies

Implement `IMapCreationStrategy<Map>` for custom generation algorithms:

```csharp
public class CustomMapStrategy : IMapCreationStrategy<Map>
{
    public Map CreateMap()
    {
        // Custom map generation logic
        return new Map(width, height);
    }
}
```
