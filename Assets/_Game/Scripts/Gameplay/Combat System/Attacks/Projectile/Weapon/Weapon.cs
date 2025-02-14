using MEC;
using Obvious.Soap;
using UnityEngine;

public class Weapon : MonoBehaviour, IAttack
{
    [SerializeField] private ScriptableListEnemy _scriptableListEnemy;
    [SerializeField] private Attack _attackPrefab;
    [SerializeField] private Attack _critAttackPrefab;
    [SerializeField] private FloatVariable _spawnOffset;
    [Header("Weapon Stats")]
    [SerializeField] private FloatVariable _playerAttack;
    [SerializeField] private FloatVariable _playerAttackSpeed;
    [SerializeField] private FloatVariable _playerCritRate;
    [SerializeField] private FloatVariable _playerCritMult;
    [SerializeField] private FloatVariable _playerAmp;
    [SerializeField] private FloatVariable _playerPenetration;
    [SerializeField] private FloatVariable _playerMaxRange;
    private Transform _ownerTransform;
    private float _timer;
    
    private void Awake()
    {
        _ownerTransform = transform.parent == null ? transform : transform.parent;
    }

    private void Start()
    {
        Timing.RunCoroutine(Utility.EmulateUpdate(MyUpdate, this).CancelWith(gameObject));
    }

    private void MyUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer < 1f / _playerAttackSpeed) return;
        ShootAtClosestEnemy();
        _timer = 0;
    }

    private void ShootAtClosestEnemy()
    {
        var closestEnemy = _scriptableListEnemy.GetClosest(transform.position,_playerMaxRange);
        
        if (closestEnemy == null) return;
        var direction = closestEnemy.transform.position - _ownerTransform.position;
        SpawnAttack(direction.normalized);  
    }

    public void SpawnAttack(Vector2 directionNormalized)
    {
        Vector2 spawnPoint = (Vector2)_ownerTransform.position + directionNormalized * _spawnOffset;

        // Determine if the attack is a critical hit
        bool isCriticalHit = Random.value < _playerCritRate.Value;

        // Choose the appropriate projectile prefab
        Attack attackPrefab = isCriticalHit ? _critAttackPrefab : _attackPrefab;

        // Get an object from the pool
        Attack attack = ObjectPool.Instance.GetObject(attackPrefab);

        // Calculate damage
        float damage = GetDamage(isCriticalHit);

        // Initialize the attack
        attack.Init(damage, _playerPenetration, directionNormalized, spawnPoint);
    }

    public float GetDamage(bool isCriticalHit)
    {
        float damage = _playerAttack.Value * (1 + _playerAmp.Value);

        if (isCriticalHit)
        {
            damage *= (1 + _playerCritMult.Value);
        }

        return damage;
    }
}
