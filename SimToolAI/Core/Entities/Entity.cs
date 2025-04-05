using SimToolAI.Core.Rendering;

namespace SimToolAI.Core.Entities
{
    public abstract class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Awareness { get; set; }
        public string Name { get; set; }
        public IRenderable Avatar { get; set; }
    
        protected Entity(string name, int x, int y, int awareness)
        {
            Name = name;
            X = x;
            Y = y;
            Awareness = awareness;
        }

        protected Entity(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
            Awareness = 0;
        }
        
        protected Entity(int x, int y)
        {
            Name = "null";
            X = x;
            Y = y;
            Awareness = 0;
        }

        /// <summary>
        /// Updates the entity state
        /// </summary>
        public virtual void Update(float deltaTime) {}
    }
}