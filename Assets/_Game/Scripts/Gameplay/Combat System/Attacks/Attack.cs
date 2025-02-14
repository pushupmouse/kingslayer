using Obvious.Soap;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private FloatReference _lifeTime;

    protected float _damage;
    protected float _penetration;
    private bool _hasHit = false;

    public void Init(float damage, float penetration, Vector3 direction, Vector3 spawnPosition)
    {
        _damage = damage;
        _penetration = penetration;
        _hasHit = false; // Reset so the projectile can hit again
        
        transform.position = spawnPosition;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        Invoke(nameof(Deactivate), _lifeTime);
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            _hasHit = true;
            damageable.TakeDamage(_damage, _penetration);
            Deactivate();
        }
    }

    protected virtual void Deactivate()
    {
        CancelInvoke(nameof(Deactivate));
        ObjectPool.Instance.ReturnObject(this);
    }
}
