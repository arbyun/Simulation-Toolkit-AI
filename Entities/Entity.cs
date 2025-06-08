
using System;

namespace SimArena.Entities
{
    public class Entity 
    {
        private int _x;
        private int _y;
    
        public int X 
        { 
            get => _x;
            set => _x = value;
        }
    
        public int Y 
        { 
            get => _y;
            set => _y = value;
        }
    
        public Guid Id { get; }

        #region Constructors

        public Entity(int x, int y)
        {
            _x = x;
            _y = y;
            Id = Guid.NewGuid();
        }

        #endregion
    }
}
