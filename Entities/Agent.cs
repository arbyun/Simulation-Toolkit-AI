using SimArena.Core;
using SimArena.Core.Results;

namespace SimArena.Entities
{
    public class Agent : Entity, IDamageable
    {
        public Brain Brain { get; private set; }
        public int Team { get; }
        public bool IsAlive { get; private set; } = true;
        public string Name { get; }
        
        public Kda Kda { get; set; } = new(0, 0, 0);
        
        public int Health { get; set; } = 100;

        public List<Agent> RecentAttackers { get; private set; } = new();

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
            // KDA stats are updated by the objective tracker, not here
        }

        public void TakeDamage(int damage, Agent attacker)
        {
            Health = Math.Max(0, Health - damage);
            RecentAttackers.Add(attacker);
            
            if (Health == 0)
            {
                Kill();
                // KDA stats are updated by the objective tracker, not here
            }
        }
    }
}
