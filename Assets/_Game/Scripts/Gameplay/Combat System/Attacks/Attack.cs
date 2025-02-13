using Obvious.Soap;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private FloatReference _lifeTime;

    private float _damage;
    private float _penetration;
    private bool _hasHit = false;
    
    protected virtual void OnEnable()
    {
        Invoke(nameof(Deactivate), _lifeTime);
    }
    
    public virtual void Init(float damage, float penetration, Vector3 direction, Vector3 spawnPosition)
    {
        _damage = damage;
        _penetration = penetration;
        _hasHit = false; // Reset so the projectile can hit again
        
        transform.position = spawnPosition;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;
    
        _hasHit = true;

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && other.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(_damage, _penetration);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage(_damage, _penetration);
        }

        Deactivate();
    }

    protected virtual void Deactivate()
    {
        CancelInvoke(nameof(Deactivate));
        ObjectPool.Instance.ReturnObject(this);
    }
}
