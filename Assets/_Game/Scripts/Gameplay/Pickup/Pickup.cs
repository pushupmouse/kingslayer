using System;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public event Action OnPickedUp;
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        OnPickedUp?.Invoke();
        Destroy(gameObject);
    }
}
