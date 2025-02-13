using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    [SerializeField] private ScriptableListEnemy _scriptableListEnemy;
    [SerializeField] private ParticleSystem _spawnVfxPrefab;
    [SerializeField] private ParticleSystem _destroyVfxPrefab;

    private void Awake()
    {
        _scriptableListEnemy.OnItemAdded += OnEnemySpawned;
        _scriptableListEnemy.OnItemRemoved += OnEnemyDied;
    }

    private void OnDestroy()
    {
        _scriptableListEnemy.OnItemAdded -= OnEnemySpawned;
        _scriptableListEnemy.OnItemRemoved -= OnEnemyDied;
    }
    
    private void OnEnemySpawned(Enemy obj)
    {
        Instantiate(_spawnVfxPrefab.gameObject, obj.transform.position, Quaternion.identity, transform);
    }
    
    private void OnEnemyDied(Enemy obj)
    {
        Instantiate(_destroyVfxPrefab.gameObject, obj.transform.position, Quaternion.identity, transform);
    }
}
