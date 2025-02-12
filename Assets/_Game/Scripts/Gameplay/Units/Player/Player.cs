using System;
using MEC;
using Obvious.Soap;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private ScriptableEventInt _onPlayerHealed;
    [SerializeField] private ScriptableEventInt _onPlayerDamaged;
    [SerializeField] private ScriptableEventNoParam _onPlayerDeath;
    [SerializeField] private FloatVariable _defCoefficient;
    [SerializeField] private FloatVariable _regenInterval;
    [Header("Player Stats")]
    [SerializeField] private FloatVariable _currentHealth;
    [SerializeField] private FloatVariable _finalMaxHealth;
    [SerializeField] private FloatVariable _finalDefense;
    [SerializeField] private FloatVariable _finalHealthRegen;
    [field: SerializeField] public HealthBar HealthBar { get; set; }
    
    private float _timer;
    
    private void Awake()
    {
        _currentHealth.Value = _finalMaxHealth.Value;
        _currentHealth.OnValueChanged += OnCurrentHealthChanged;
        _finalMaxHealth.OnValueChanged += OnFinalMaxHealthChanged;
        Timing.RunCoroutine(Utility.EmulateUpdate(MyUpdate, this).CancelWith(gameObject));
    }

    private void MyUpdate()
    {
        if (_finalHealthRegen <= 0) return;
        _timer += Time.deltaTime;
        if (_timer < _regenInterval) return;
        _currentHealth.Value += _finalMaxHealth * _finalHealthRegen/100f;
        _timer = 0;
    }

    private void OnDestroy()
    {
        _currentHealth.OnValueChanged -= OnCurrentHealthChanged;
        _finalMaxHealth.OnValueChanged -= OnFinalMaxHealthChanged;
    }

    public void OnFinalMaxHealthChanged(float newMaxHealth)
    {
        var diff = newMaxHealth - _finalMaxHealth.PreviousValue;
        _currentHealth.MinMax = new Vector2(0, newMaxHealth);
        _currentHealth.Add(diff);
    }
    
    public void OnCurrentHealthChanged(float newHealthValue)
    {
        if (HealthBar != null)
            HealthBar.UpdateHealthBar(_currentHealth, _finalMaxHealth);
        
        var diff = newHealthValue - _currentHealth.PreviousValue;

        if (diff < 0)
        {
            if(_currentHealth.Value <= 0)
                Die();
            else
            {
                _onPlayerDamaged.Raise(Math.Abs(Mathf.RoundToInt(diff)));
            }
        }
        else
        {
            _onPlayerHealed.Raise(Mathf.RoundToInt(diff));
        }
    }

    public void TakeDamage(float damage, float penetration)
    {
        if (penetration >= 1)
        {
            _currentHealth.Add(-damage);
        }
        else
        {
            var effectiveDefense = _finalDefense - (_finalDefense * penetration) ;

            var damageMultiplier = 1 - (effectiveDefense / (effectiveDefense + _defCoefficient));

            var finalDamage = damage * damageMultiplier;

            _currentHealth.Add(-finalDamage);
        }
    }

    public void Die()
    {
        _onPlayerDeath.Raise();
    }
}
