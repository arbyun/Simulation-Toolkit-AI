using SimArena.Core.Entities;

namespace SimArena.Core.SimulationElements.Map
{
    public class ContinuousMap: Map, IMap
    {
        public int Width { get; }
        public int Height { get; }

        public bool IsInBounds(int x, int y)
        {
            throw new NotImplementedException();
        }

        public bool IsWalkable(int x, int y)
        {
            throw new NotImplementedException();
        }

        public Entity IsOccupiedBy(int x, int y)
        {
            throw new NotImplementedException();
        }

        public bool IsTransparent(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void SetWalkable(int x, int y, bool isWalkable, Entity entity = null)
        {
            throw new NotImplementedException();
        }

        public void SetWalkable(int x, int y, bool isWalkable)
        {
            throw new NotImplementedException();
        }

        public void SetTransparent(int x, int y, bool isTransparent)
        {
            throw new NotImplementedException();
        }

        public void ComputeFov(Character entity, bool lightWalls = true)
        {
            throw new NotImplementedException();
        }

        public void ToggleFieldOfView(Character entity, bool enabled = true)
        {
            throw new NotImplementedException();
        }

        public bool IsInFov(int x, int y)
        {
            throw new NotImplementedException();
        }

        public bool SetEntityPosition(Entity entity, int x, int y)
        {
            throw new NotImplementedException();
        }

        public (int x, int y)? GetRandomWalkableLocation(int minX, int maxX, int minY, int maxY)
        {
            throw new NotImplementedException();
        }

        public (int x, int y)? GetRandomWalkableLocation()
        {
            throw new NotImplementedException();
        }

        public float GetDistance(int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
        }

        public bool IsInLineOfSight(int ownerX, int ownerY, int argX, int argY)
        {
            throw new NotImplementedException();
        }
    }
}