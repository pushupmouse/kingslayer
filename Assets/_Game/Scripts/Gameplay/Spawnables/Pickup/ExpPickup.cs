using System;
using MEC;
using Obvious.Soap;
using UnityEngine;

public class ExpPickup : Pickup
{
    [SerializeField] private FloatVariable _playerCurrentExperience;
    [SerializeField] private Vector2Variable _playerTransform;
    [SerializeField] private FloatVariable _expPickupValue;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private float _attractSpeed = 20f;
    private bool _isAttracted = false;
    private Vector2 _targetPosition;

    private void OnEnable()
    {
        Timing.RunCoroutine(Utility.EmulateUpdate(MyUpdate, this).CancelWith(gameObject));
    }

    private void MyUpdate()
    {
        if (!_isAttracted) return;
        // Move towards player position
        transform.position = Vector2.MoveTowards(
            transform.position, _playerTransform, _attractSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        if (_trail != null)
        {
            _trail.Clear();
        }
        
        _isAttracted = false;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        _playerCurrentExperience.Add(_expPickupValue);
        base.OnTriggerEnter2D(other);
    }

    public void EnableAttracted()
    {
        _isAttracted = true;
    }
}