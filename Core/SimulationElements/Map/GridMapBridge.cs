using RogueSharp;
using SimArena.Core.Entities;

namespace SimArena.Core.SimulationElements.Map;

public class GridMapBridge: IMapBridge
{
    public Simulation Simulation { get; }
    public RogueSharp.Map Map { get; private set; }

    private readonly Random _random = new();

    public GridMapBridge(Simulation simulation)
    {
        Simulation = simulation;
    }

    public void SetWalkable(int x, int y, bool isWalkable, Entity entity = null)
    {
        if (!IsInBounds(x, y))
            return;

        Map.GetCell(x, y).IsWalkable = isWalkable;

        if (entity != null) Map.GetCell(x, y).SetEntity(entity, Simulation);
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Map.Width && y >= 0 && y < Map.Height;
    }

    public void SetTransparent(int x, int y, bool isTransparent)
    {
        if (!IsInBounds(x, y))
            return;

        Map.GetCell(x, y).IsTransparent = isTransparent;
    }

    public bool IsInFov(int x, int y, Character entity)
    {
        if (!IsInBounds(x, y))
            return false;

        var fov = new FieldOfView(Map);
        fov.ComputeFov(entity.X, entity.Y, entity.Brain.Awareness, false);

        return fov.IsInFov(x, y);
    }
    
    public (int x, int y) GetRandomWalkableLocation(int minX, int maxX, int minY, int maxY, Entity ent = null)
    {
        // Ensure bounds are within map limits
        minX = Math.Clamp(minX, 0, Map.Width - 1);
        maxX = Math.Clamp(maxX, 0, Map.Width - 1);
        minY = Math.Clamp(minY, 0, Map.Height - 1);
        maxY = Math.Clamp(maxY, 0, Map.Height - 1);

        // Check if there's any walkable space in the area
        var hasWalkableSpace = false;
        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                if (ent != null)
                {
                    if (x == ent.X && y == ent.Y)
                    {
                        continue;
                    }
                }

                if (Map.IsWalkable(x, y))
                {
                    hasWalkableSpace = true;
                    break;
                }
            }

            if (hasWalkableSpace) break;
        }

        if (!hasWalkableSpace)
        {
            if (ent != null)
            {
                return (ent.X, ent.Y);
            }
            else
            {
                throw new Exception("No place available to move to.");
            }
        }

        // Try to find a random walkable location
        for (var i = 0; i < 100; i++)
        {
            var x = _random.Next(minX, maxX + 1);
            var y = _random.Next(minY, maxY + 1);

            if (Map.IsWalkable(x, y))
                return (x, y);
        }

        throw new InvalidOperationException("Could not find a walkable location after 100 attempts.");
    }

    public (int x, int y) GetRandomWalkableLocation()
    {
        return GetRandomWalkableLocation(0, Map.Width - 1, 0, Map.Height - 1);
    }

    public (int x, int y) GetRandomWalkableLocation(Character owner)
    {
        return GetRandomWalkableLocation(0, Map.Width - 1, 0, Map.Height - 1, owner);
    }

    public bool SetEntityPosition(Entity entity, int x, int y)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (!IsInBounds(x, y) || !Map.IsWalkable(x, y))
            return false;

        // Make the entity's current position walkable again
        if (IsInBounds(entity.X, entity.Y)) SetWalkable(entity.X, entity.Y, true);

        // Update entity position
        entity.X = x;
        entity.Y = y;

        // Make the entity's new position not walkable if the entity blocks movement
        if (entity.BlocksMovement) SetWalkable(x, y, false, entity);

        return true;
    }

    public Entity? IsOccupiedBy(int x, int y)
    {
        if (Map.IsWalkable(x, y)) return null;

        return Map.GetCell(x, y).GetEntity(Simulation);
    }
    
    public float GetDistance(int x1, int y1, int x2, int y2)
    {
        var dx = x1 - x2;
        var dy = y1 - y2;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }
    
    public bool IsInLineOfSight(int ownerX, int ownerY, int argX, int argY)
    {
        // Check if both positions are in bounds
        if (!IsInBounds(ownerX, ownerY) || !IsInBounds(argX, argY))
            return false;

        // Use Bresenham's line algorithm to check for obstacles
        var dx = Math.Abs(argX - ownerX);
        var dy = Math.Abs(argY - ownerY);
        var sx = ownerX < argX ? 1 : -1;
        var sy = ownerY < argY ? 1 : -1;
        var err = dx - dy;

        var x = ownerX;
        var y = ownerY;

        while (x != argX || y != argY)
        {
            // Skip the starting position
            if (x != ownerX || y != ownerY)
                // If we hit a non-transparent cell, there's no line of sight
                if (!Map.GetCell(x, y).IsTransparent)
                    return false;

            var e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }

        // If we reached the target without hitting obstacles, there's a clear line of sight
        return true;
    }
    
    public void GenerateMap(RogueSharp.Map m)
    {
        Map = m;
    }
}