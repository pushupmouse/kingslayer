using UnityEngine;

public class TransformListHandler : MonoBehaviour
{
    [SerializeField] private ScriptableListTransform _scriptableListExperiencePickup;

    private void Start()
    {
        _scriptableListExperiencePickup.Add(transform);
    }

    private void OnDestroy()
    {
        _scriptableListExperiencePickup.Remove(transform);
    }
}
