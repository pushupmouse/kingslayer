using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    private Dictionary<string, Queue<Component>> pools = new Dictionary<string, Queue<Component>>();
    private Dictionary<string, Transform> poolParents = new Dictionary<string, Transform>(); // Track parent objects

    private Transform _parentTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _parentTransform = transform; // Root parent for organization
    }

    public T GetObject<T>(T prefab) where T : Component
    {
        string key = prefab.gameObject.name; // Use prefab name as key

        if (!pools.ContainsKey(key) || pools[key].Count == 0)
        {
            return CreateNewObject(prefab, true);
        }

        T obj = (T)pools[key].Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnObject<T>(T obj) where T : Component
    {
        string key = obj.gameObject.name.Replace("(Clone)", "").Trim(); // Remove "(Clone)" to match prefab name

        obj.gameObject.SetActive(false);

        if (!pools.ContainsKey(key))
        {
            pools[key] = new Queue<Component>();
        }

        pools[key].Enqueue(obj);
        
        // Move the object under its designated parent
        if (poolParents.ContainsKey(key))
        {
            obj.transform.SetParent(poolParents[key]);
        }
    }

    private T CreateNewObject<T>(T prefab, bool isActive) where T : Component
    {
        string key = prefab.gameObject.name;

        // Ensure a parent exists for this prefab type
        if (!poolParents.ContainsKey(key))
        {
            GameObject parentObject = new GameObject($"{key} Pool");
            parentObject.transform.SetParent(_parentTransform);
            poolParents[key] = parentObject.transform;
        }

        T newObj = Instantiate(prefab, poolParents[key]); // Instantiate under the correct parent
        newObj.gameObject.name = key; // Ensure name consistency
        newObj.gameObject.SetActive(isActive);
        return newObj;
    }
}
