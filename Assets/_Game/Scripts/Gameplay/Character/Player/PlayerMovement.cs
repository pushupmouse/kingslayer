using MEC;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Vector2Variable _inputs;
    [SerializeField] private FloatReference _finalSpeed;
    [SerializeField] private TransformVariable _playerTransform;

    private void Awake()
    {
        _playerTransform.Value = transform;
    }

    private void Start()
    {
        Timing.RunCoroutine(Utility.EmulateUpdate(MyUpdate, this).CancelWith(gameObject));
    }

    private void MyUpdate()
    {
        transform.position += new Vector3(_inputs.Value.x, _inputs.Value.y, 0) * (_finalSpeed * Time.deltaTime);

    }
}
