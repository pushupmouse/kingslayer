using Obvious.Soap;
using UnityEngine;

public class ExpPickup : Pickup
{
    [SerializeField] private FloatVariable _playerCurrentExperience;
    [SerializeField] private FloatVariable _expPickupValue;
    protected override void OnTriggerEnter2D(Collider2D  other)
    {
        _playerCurrentExperience.Add(_expPickupValue);
        base.OnTriggerEnter2D(other);
    }
}
