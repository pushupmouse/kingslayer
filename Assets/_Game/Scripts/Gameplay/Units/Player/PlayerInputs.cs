using MEC;
using Obvious.Soap;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private Vector2Variable _inputs;

    private void Start()
    {
        Timing.RunCoroutine(Utility.EmulateUpdate(MyUpdate, this).CancelWith(gameObject));
    }

    private void MyUpdate()
    {
        _inputs.Value = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }
}
