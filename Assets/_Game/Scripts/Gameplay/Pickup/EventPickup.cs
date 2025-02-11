using Obvious.Soap;
using UnityEngine;

public class EventPickup : Pickup
{
    [SerializeField] private ScriptableEventNoParam _onPickedUpEvent;

    protected override void OnTriggerEnter2D(Collider2D  other)
    {
        _onPickedUpEvent.Raise();
        
        base.OnTriggerEnter2D(other);
    }
}
