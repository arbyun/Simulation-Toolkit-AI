using System;
using RogueSharp;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Rendering;

namespace SimToolAI.Core.Map
{
    public class GridMap: RogueSharp.Map, ISimMap
    {
        int ISimMap.Width => base.Width;
        int ISimMap.Height => base.Height;
        
        public IRenderable Renderable { get; private set; }

        private FieldOfView _fieldOfView;
        private Entity _currentFovEntity;

        public GridMap(int width, int height, IRenderable r) : base(width, height)
        {
            _fieldOfView = new FieldOfView(this);
            Renderable = r;
        }

        public GridMap()
        {
        }
        
        public void Initialize(IRenderable r)
        {
            Renderable = r;
        }
        
        public void ToggleFieldOfView(Entity entity, bool enabled = true)
        {
            if (_fieldOfView == null)
            {
                _fieldOfView = new FieldOfView(this);
            }
            
            if (enabled == false)
            {
                _currentFovEntity = null;
            }
            
            _currentFovEntity = entity;
        }

        public void SetWalkable(int x, int y, bool isWalkable)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;
            
            GetCell(x, y).IsWalkable = isWalkable;
        }
        
        public void SetTransparent(int x, int y, bool isTransparent)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;
            
            GetCell(x, y).IsTransparent = isTransparent;
        }

        public void ComputeFov(Entity entity, bool lightWalls = true)
        {
            _fieldOfView.ComputeFov(entity.X, entity.Y, entity.Awareness, lightWalls);
        }

        public bool IsInFov(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return false;
            
            return _fieldOfView.IsInFov(x, y);
        }
        
        public (int x, int y)? GetRandomWalkableLocation(int minX, int maxX, int minY, int maxY)
        {
            // Ensure bounds are within map limits
            minX = Math.Max(0, minX);
            maxX = Math.Min(Width - 1, maxX);
            minY = Math.Max(0, minY);
            maxY = Math.Min(Height - 1, maxY);
            
            // Check if there's any walkable space in the area
            bool hasWalkableSpace = false;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (IsWalkable(x, y))
                    {
                        hasWalkableSpace = true;
                        break;
                    }
                }
                if (hasWalkableSpace) break;
            }
            
            if (!hasWalkableSpace)
                return null;
            
            // Try to find a random walkable location
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(minX, maxX + 1);
                int y = random.Next(minY, maxY + 1);
                
                if (IsWalkable(x, y))
                    return (x, y);
            }
            
            return null;
        }
        
        public bool SetEntityPosition(Entity entity, int x, int y)
        {
            if (!IsWalkable(x, y))
                return false;
            
            // Make the entity's current position walkable again
            SetWalkable(entity.X, entity.Y, true);
            
            // Update entity position
            entity.X = x;
            entity.Y = y;
            
            // Make the entity's new position not walkable
            SetWalkable(x, y, false);
            
            // Compute FOV with the entity as the center
            if (_currentFovEntity == entity)
            {
                ComputeFov(entity);
            }
            
            return true;
        }
    }
}