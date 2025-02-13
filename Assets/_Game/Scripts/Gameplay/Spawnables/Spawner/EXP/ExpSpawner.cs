using UnityEngine;

public class ExpSpawner : MonoBehaviour
{
    [SerializeField] private ScriptableListEnemy _scriptableListEnemy;
    [SerializeField] private ExpPickup _expPickupPrefab;
    
    private void Awake()
    {
        _scriptableListEnemy.OnItemRemoved += OnEnemyDied;
    }

    private void OnDestroy()
    {
        _scriptableListEnemy.OnItemRemoved -= OnEnemyDied;
    }
    
    private void OnEnemyDied(Enemy enemy)
    {
        ExpPickup pickup = ObjectPool.Instance.GetObject(_expPickupPrefab);
        pickup.Init(enemy.transform.position);
    }
}
