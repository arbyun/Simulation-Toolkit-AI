using RogueSharp;
using SimArena.Core.Entities;

namespace SimArena.Core.SimulationElements.Map;

public static class CellExtensions
{
    public static Entity GetEntity(this Cell c, Simulation simulation)
    {
        return simulation.EntityManager.Get(c);
    }

    public static void SetEntity(this Cell c, Entity e, Simulation simulation)
    {
        simulation.EntityManager.Set(e, c);
    }
}