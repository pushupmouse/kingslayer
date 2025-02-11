public interface IDamageable
{
    HealthBar HealthBar {get; set;}
    void OnFinalMaxHealthChanged(float newMaxHealth);
    void OnCurrentHealthChanged(float newCurrentHealth);
    void Die();
    void TakeDamage(float damage, float penetration);
}
