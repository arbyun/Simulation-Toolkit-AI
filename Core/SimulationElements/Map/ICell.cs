using System;
using SimArena.Core.Entities;

namespace SimArena.Core.SimulationElements.Map
{
    public interface ICell : IEquatable<ICell>
    {
        int X { get; set; }

        int Y { get; set; }

        bool IsTransparent { get; set; }

        bool IsWalkable { get; set; }
        
        Entity Entity { get; set; }
    }
}