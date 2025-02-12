using Obvious.Soap;
using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] private FloatVariable _playerMaxHealth;
    [SerializeField] private FloatVariable _playerCurrentHealth;
    [SerializeField] private FloatReference _valueMult;
    [SerializeField] private ScriptableEventInt _onPlayerHealed;
    protected override void OnTriggerEnter2D(Collider2D  other)
    {
        var healAmount = _playerMaxHealth * _valueMult;
        _playerCurrentHealth.Add(healAmount);
        _onPlayerHealed.Raise(0);
        base.OnTriggerEnter2D(other);
    }
}
