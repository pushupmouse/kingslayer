using System;
using Obvious.Soap;
using UnityEngine;
using Yade.Runtime;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyType _type;
    [SerializeField] private YadeSheetData _initEnemyStatsData;
    [SerializeField] private YadeSheetData _enemyGrowthData;
    [SerializeField] private IntVariable _currentRoundPhase;
    [SerializeField] private ScriptableListEnemy _scriptableListEnemy;
    [SerializeField] private FloatVariable _defCoefficient;
    [field: SerializeField] public HealthBar HealthBar { get; set; }

    private float _maxHealth;
    private float _defense;
    private float _maxHealthMult;
    private float _defenseMult;
    private FloatVariable _runtimeMaxHealth;
    private FloatVariable _runtimeCurrentHealth;
        
    private class EnemyStat
    {
        [DataField(2)] public string MaxHealth;
        [DataField(3)] public string Defense;
    }
        
    private class EnemyGrowth
    {
        [DataField(2)] public string MaxHealth;
        [DataField(3)] public string Defense;
    }

    private void Awake()
    {
        if (_runtimeMaxHealth == null)
            _runtimeMaxHealth = SoapUtils.CreateRuntimeInstance<FloatVariable>($"float_{gameObject.name}MaxHealth");
        
        
        if (_runtimeCurrentHealth == null)
            _runtimeCurrentHealth = SoapUtils.CreateRuntimeInstance<FloatVariable>($"float_{gameObject.name}CurrentHealth");
    }

    private void OnEnable()
    {
        _scriptableListEnemy.Add(this);
        
        _runtimeMaxHealth.OnValueChanged += OnFinalMaxHealthChanged;
        _runtimeCurrentHealth.OnValueChanged += OnCurrentHealthChanged;
        
        InitStats();

        if(HealthBar != null)
            HealthBar.Init();
    }

    private void OnDisable()
    {
        _runtimeMaxHealth.Value = _maxHealth;
        
        _runtimeMaxHealth.OnValueChanged -= OnFinalMaxHealthChanged;
        _runtimeCurrentHealth.OnValueChanged -= OnCurrentHealthChanged;
    }

    public void Init(Vector3 spawnPoint)
    {
        transform.position = spawnPoint;
    }
    
    public void InitStats()
    {
        int index = (int)_type;
        var growthList = _enemyGrowthData.AsList<EnemyGrowth>();
        _maxHealthMult = float.Parse(growthList[index].MaxHealth);
        _defenseMult = float.Parse(growthList[index].Defense);
        
        var statList = _initEnemyStatsData.AsList<EnemyStat>();
        
        _maxHealth = float.Parse(statList[index].MaxHealth) * (1 + _maxHealthMult * _currentRoundPhase.Value);
        _defense = float.Parse(statList[index].Defense) * (1 + _defenseMult * _currentRoundPhase.Value);
        
        _runtimeMaxHealth.Value = _maxHealth;
        _runtimeCurrentHealth.Value = _runtimeMaxHealth;
    }

    public void OnFinalMaxHealthChanged(float newMaxHealth)
    {
        _runtimeCurrentHealth.MinMax = new Vector2(0, newMaxHealth);
        var diff = newMaxHealth - _runtimeMaxHealth.PreviousValue;
        _runtimeCurrentHealth.Add(diff);
    }

    public void OnCurrentHealthChanged(float newHealthValue)
    {
        HealthBar.UpdateHealthBar(_runtimeCurrentHealth, _runtimeMaxHealth);
        if (_runtimeCurrentHealth.Value <= 0)
            Die();
    }

    public void Die()
    {
        _scriptableListEnemy.Remove(this);
        ObjectPool.Instance.ReturnObject(this);
    }
    
    public void TakeDamage(float damage, float penetration)
    {
        if (penetration >= 1)
        {
            _runtimeCurrentHealth.Add(-damage);
        }
        else
        {
            var effectiveDefense = _defense - (_defense * penetration) ;

            var damageMultiplier = 1 - (effectiveDefense / (effectiveDefense + _defCoefficient));

            var finalDamage = damage * damageMultiplier;

            _runtimeCurrentHealth.Add(-finalDamage);
        }
    }
    
}