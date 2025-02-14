using System.Collections.Generic;
using MEC;
using Obvious.Soap;
using UnityEngine;
using Yade.Runtime;
using Random = UnityEngine.Random;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private EnemyType _type;
    [SerializeField] private YadeSheetData _initEnemyStatsData;
    [SerializeField] private YadeSheetData _enemyGrowthData;
    [SerializeField] private IntVariable _currentRoundPhase;
    
    [Header("Attack")]
    [SerializeField] private AttackType _attackType;
    [SerializeField] private Attack _attackPrefab;
    [SerializeField] private Attack _critAttackPrefab;
    [SerializeField] private FloatReference _spawnOffsetProjectile;
    [SerializeField, Range(0f, 1f)] private float _attackDeviaton;
    
    [Header("Other")]
    [SerializeField] private Vector2Variable _playerPosition;
    [SerializeField] private float _idleDuration;


    private StateType _stateType;
    private float _attackTimer = 0;
    private bool _canAttack = true; // Attack is initially ready
    private bool _isIdling = false; // Controls idle delay after attacking
    
    private float _critRate;
    private float _critMult;
    private float _amp;
    private float _penetration;
    private float _speed;
    private float _attackSpeed;
    private float _range;
    private float _attackMult;
    private float _attack;

    private class EnemyStat
    {
        [DataField(1)] public string Attack;
        [DataField(4)] public string Speed;
        [DataField(5)] public string AttackSpeed;
        [DataField(6)] public string Range;
        [DataField(7)] public string CritRate;
        [DataField(8)] public string CritMult;
        [DataField(9)] public string Amp;
        [DataField(10)] public string Penetration;
    }
    
    private class EnemyGrowth
    {
        [DataField(1)] public string Attack;
    }

    private void OnEnable()
    {
        Init();
        Timing.RunCoroutine(Utility.EmulateUpdate(MyUpdate, this).CancelWith(gameObject));
    }

    private void OnDisable()
    {
        SwitchState(StateType.Idle);
        _canAttack = true;
        _isIdling = false;
    }

    private void Init()
    {
        int index = (int)_type;
        var growthList = _enemyGrowthData.AsList<EnemyGrowth>();
        _attackMult = float.Parse(growthList[index].Attack);
        
        var statList = _initEnemyStatsData.AsList<EnemyStat>();
        _attack = float.Parse(statList[index].Attack) * (1 + _attackMult * _currentRoundPhase);
        _critRate = float.Parse(statList[index].CritRate);
        _critMult = float.Parse(statList[index].CritMult);
        _amp = float.Parse(statList[index].Amp);
        _penetration = float.Parse(statList[index].Penetration);
        _attackSpeed = float.Parse(statList[index].AttackSpeed);
        _speed = float.Parse(statList[index].Speed);
        _range = float.Parse(statList[index].Range);
        
        _canAttack = true;
        _attackTimer = 0f;
        _isIdling = false;
        _stateType = StateType.Idle;
    }
    
    private void MyUpdate()
    {
        HandleAttackCooldown(); // Extracted cooldown logic into a method

        if (_isIdling) return; // Stop processing if idling

        float distanceToTarget = GetDistanceToTarget(); // Consistent distance calculation

        switch (_stateType)
        {
            case StateType.Idle:
                if (distanceToTarget > _range) 
                {
                    SwitchState(StateType.Chase);
                }
                break;

            case StateType.Chase:
                if (distanceToTarget > _range)
                {
                    Chase();
                }
                else if (distanceToTarget <= _range && _canAttack)
                {
                    SwitchState(StateType.Attack);
                }
                break;

            case StateType.Attack:
                if (_canAttack)
                {
                    PerformAttack();
                    _canAttack = false;
                    Timing.RunCoroutine(IdleDelay().CancelWith(gameObject)); // Start idle cooldown
                }
                break;
        }
    }
    
    private float GetDistanceToTarget()
    {
        return Vector2.Distance(_playerPosition.Value, transform.position);
    }
    
    private void Chase()
    {
        Vector2 direction = (_playerPosition.Value - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * (_speed * Time.deltaTime));
    }
    
    private void HandleAttackCooldown()
    {
        if (!_canAttack)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= 1f / _attackSpeed)
            {
                _canAttack = true;
                _attackTimer = 0f;

                // Ensure state switches back to Attack when cooldown is up
                float distanceToTarget = GetDistanceToTarget();
                if (_stateType == StateType.Idle && distanceToTarget <= _range)
                {
                    SwitchState(StateType.Attack);
                }
            }
        }
    }

    private void PerformAttack()
    {
        var directionNormalized = (_playerPosition - (Vector2)transform.position).normalized;
        
        Vector2 randomOffset = Random.insideUnitCircle * _attackDeviaton;
        directionNormalized = (directionNormalized + randomOffset).normalized;
        
        SpawnAttack(directionNormalized);
    }

    private void SpawnAttack(Vector2 directionNormalized)
    {
        // Determine spawn position
        Vector2 spawnPosition = (Vector2)transform.position + directionNormalized * 
            (_attackType == AttackType.Melee ? _range : _spawnOffsetProjectile);

        // Determine if the attack is a critical hit
        bool isCriticalHit = Random.value < _critRate;

        // Choose the appropriate attack prefab
        Attack attackPrefab = isCriticalHit ? _critAttackPrefab : _attackPrefab;

        // Get an object from the pool
        Attack attack = ObjectPool.Instance.GetObject(attackPrefab);

        // Calculate damage
        float damage = GetDamage(isCriticalHit);

        attack.Init(damage, _penetration, directionNormalized, spawnPosition);
    }
    
    public float GetDamage(bool isCriticalHit)
    {
        float damage = _attack * (1 + _amp);

        if (isCriticalHit)
        {
            damage *= (1 + _critMult);
        }

        return damage;
    }
    
    private void SwitchState(StateType newState)
    {
        _stateType = newState;
    }
    
    private IEnumerator<float> IdleDelay()
    {
        _isIdling = true;
        SwitchState(StateType.Idle);
        yield return Timing.WaitForSeconds(_idleDuration);
        _isIdling = false;

        float distanceToTarget = Vector2.Distance(_playerPosition.Value, transform.position);

        if (distanceToTarget > _range)
        {
            SwitchState(StateType.Chase);
        }
        else if (_canAttack) // Only attack if cooldown is up
        {
            SwitchState(StateType.Attack);
        }
    }
    
    private enum StateType
    {
        Idle,
        Chase,
        Attack,
    }

    private enum AttackType
    {
        Melee,
        Ranged,
    }
}