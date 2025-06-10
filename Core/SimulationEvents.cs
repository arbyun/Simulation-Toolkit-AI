using System;
using SimArena.Core.Results;
using SimArena.Core.Results.Objective_Results;
using SimArena.Entities;

namespace SimArena.Core
{
    public class SimulationEvents
    {
        public event EventHandler Initialized;
        public event EventHandler Started;
        public event EventHandler Paused;
        public event EventHandler Resumed;
        public event EventHandler<ISimulationResult> Stopped;
        public event EventHandler<int> StepCompleted;
        public event EventHandler<Entity> OnCreate;
        public event EventHandler<Entity> OnMove;
        public event EventHandler<(Entity, Entity)> OnDamage;
        public event EventHandler<(Entity, Entity)> OnHeal;
        public event EventHandler<Agent> OnAgentKilled;
        public event EventHandler<int> OnTeamWon;
        
        public event EventHandler<(Entity, Entity)> OnKill;
        public event EventHandler<Entity> OnDestroy;
        
        // Debug event for verbose logging
        public event EventHandler<string> OnDebugMessage;

        internal void RaiseDamage(object sender, (Entity attacker, Entity victim) combat)
            => OnDamage?.Invoke(sender, combat);
        internal void RaiseHeal(object sender, (Entity healer, Entity healed) combat)
            => OnHeal?.Invoke(sender, combat);
        internal void RaiseKill(object sender, (Entity killer, Entity killed) combat)
            => OnKill?.Invoke(sender, combat);
        internal void RaiseInitialized(object sender) => Initialized?.Invoke(sender, EventArgs.Empty);
        internal void RaiseStarted(object sender) => Started?.Invoke(sender, EventArgs.Empty);
        internal void RaisePaused(object sender) => Paused?.Invoke(sender, EventArgs.Empty);
        internal void RaiseResumed(object sender) => Resumed?.Invoke(sender, EventArgs.Empty);
        internal void RaiseStopped(object sender, ISimulationResult result) => Stopped?.Invoke(sender, result);
        internal void RaiseStepCompleted(object sender, int step) => StepCompleted?.Invoke(sender, step);
        internal void RaiseOnCreate(object sender, Entity entity) => OnCreate?.Invoke(sender, entity);
        internal void RaiseOnMove(object sender, Entity entity) => OnMove?.Invoke(sender, entity);
        internal void RaiseOnDestroy(object sender, Entity entity) => OnDestroy?.Invoke(sender, entity);
        
        internal void RaiseOnAgentKilled(object sender, Agent entity) => OnAgentKilled?.Invoke(sender, entity);
        internal void RaiseOnTeamWon(object sender, int team) => OnTeamWon?.Invoke(sender, team);
        internal void RaiseDebugMessage(object sender, string message) => OnDebugMessage?.Invoke(sender, message);
    }
}
