using System;
using System.Collections.Generic;
using SimArena.Core;
using SimArena.Core.Results;
using SimArena.Entities.Weapons;

namespace SimArena.Entities
{
    public class Agent : Entity, IDamageable
    {
        public Brain Brain { get; private set; }
        public List<Weapon> Weapons { get; private set; } = new();
        public int Team 
        {
            get
            {
                if (Brain != null)
                {
                    return Brain.Team;
                }

                return -1;
            }
        }
        
        public bool IsAlive { get; private set; } = true;
        public string Name { get; }
        
        public Kda Kda { get; set; } = new(0, 0, 0);
        
        public int Health { get; set; } = 100;

        public List<Agent> RecentAttackers { get; private set; } = new();

        public Agent(int x, int y, Brain brain, string name) : base(x, y)
        {
            Brain = brain;
            Name = name;
        }
        
        public Agent(int x, int y, Brain brain) : base(x, y)
        {
            Brain = brain;
            Name = "Agent_Unknown";
        }

        public void SetBrain(Brain brain)
        {
            Brain = brain;
        }
        
        public void EquipWeapon(Weapon weapon)
        {
            Weapons.Add(weapon);
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
