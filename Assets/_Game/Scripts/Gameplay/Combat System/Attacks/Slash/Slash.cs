using Obvious.Soap;
using UnityEngine;

public class Slash : Attack
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage, _penetration);
        }
    }
}
