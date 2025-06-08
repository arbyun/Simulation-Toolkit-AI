using SimArena.Core;
using SimArena.Core.Results;

namespace SimArena.Entities
{
    public class Agent : Entity
    {
        public Brain Brain { get; private set; }
        public int Team { get; }
        public bool IsAlive { get; private set; } = true;
        public string Name { get; }
        
        public Kda Kda { get; set; } = new(0, 0, 0);

        public Agent(int x, int y, Brain brain, int team, string name) : base(x, y)
        {
            Brain = brain;
            Team = team;
            Name = name;
        }
        
        public Agent(int x, int y, Brain brain, int team) : base(x, y)
        {
            Brain = brain;
            Team = team;
            Name = "Agent_Unknown";
        }

        public void SetBrain(Brain brain)
        {
            Brain = brain;
        }
    
        public void Kill()
        {
            IsAlive = false;
        }
    }
}
