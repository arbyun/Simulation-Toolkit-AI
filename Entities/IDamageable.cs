namespace SimArena.Entities
{
    public interface IDamageable
    {
        public void TakeDamage(int damage, Agent attacker);
    }
}