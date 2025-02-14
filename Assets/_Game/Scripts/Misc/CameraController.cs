using Cinemachine;
using Obvious.Soap;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private FloatVariable _playerMaxRange;
    [SerializeField] private float _minimumZoom = 10f;

    private void OnEnable()
    {
        _playerMaxRange.OnValueChanged += UpdateZoom;
    }

    private void OnDisable()
    {
        _playerMaxRange.OnValueChanged -= UpdateZoom;
    }

    private void UpdateZoom(float newRange)
    {
        if (virtualCam != null)
        {
            virtualCam.m_Lens.OrthographicSize = Mathf.Max(newRange, _minimumZoom);
        }
    }
}
