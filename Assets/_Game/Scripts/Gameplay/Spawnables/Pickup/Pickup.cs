using System;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public event Action OnPickedUp;
    
    public void Init(Vector3 position)
    {
        transform.position = position;
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        OnPickedUp?.Invoke();
        ObjectPool.Instance.ReturnObject(this);
    }
}
