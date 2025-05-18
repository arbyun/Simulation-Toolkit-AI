using SimArena.Core.Entities;

namespace SimArena.Core.SimulationElements.Map;

public interface IMapBridge
{
    public Simulation Simulation { get; }
    public RogueSharp.Map? Map { get; }
    void GenerateMap(RogueSharp.Map m);
    (int x, int y) GetRandomWalkableLocation(Character owner);
    (int x, int y) GetRandomWalkableLocation();
    bool IsInBounds(int newX, int newY);
    bool SetEntityPosition(Entity entity, int newX, int newY);
    bool IsInLineOfSight(int ownerX, int ownerY, int argX, int argY);
    void SetWalkable(int ownerX, int ownerY, bool p2, Entity entity = null);
    bool IsInFov(int characterX, int characterY, Character ent);
    float GetDistance(int ownerX, int ownerY, int characterX, int characterY);
}