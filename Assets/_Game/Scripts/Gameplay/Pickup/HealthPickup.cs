using Obvious.Soap;
using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] private FloatVariable _playerMaxHealth;
    [SerializeField] private FloatVariable _playerCurrentHealth;
    [SerializeField] private FloatReference _valueMult;
    protected override void OnTriggerEnter2D(Collider2D  other)
    {
        _playerCurrentHealth.Add(_playerMaxHealth * _valueMult);
        base.OnTriggerEnter2D(other);
    }
}
