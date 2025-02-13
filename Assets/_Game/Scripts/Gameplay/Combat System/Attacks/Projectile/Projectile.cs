using System;
using MEC;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : Attack
{
    [SerializeField] private FloatReference _speed;
    [SerializeField] private TrailRenderer _trail;

    protected override void OnEnable()
    {
        Timing.RunCoroutine(Utility.EmulateUpdate(MyUpdate, this).CancelWith(gameObject));
        
        base.OnEnable();
    }

    private void MyUpdate()
    {
        transform.position += transform.right * _speed * Time.deltaTime; // Move in the correct direction
    }

    protected override void Deactivate()
    {
        if (_trail != null)
        {
            _trail.Clear(); 
        }

        base.Deactivate();
    }
}
