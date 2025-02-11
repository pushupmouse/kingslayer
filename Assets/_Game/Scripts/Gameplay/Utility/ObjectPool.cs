using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    private static List<PooledObjectInfo> objectPools = new List<PooledObjectInfo>();
 
    private GameObject objectPoolEmptyHolder;
    private GameObject gameObjectsEmpty;
    private GameObject particleSystemEmpty;
    
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        SetUpEmpties();
    }

    private void SetUpEmpties()
    {
        objectPoolEmptyHolder = new GameObject("Pooled Objects");

        gameObjectsEmpty = new GameObject("Game Objects");
        gameObjectsEmpty.transform.SetParent(objectPoolEmptyHolder.transform); 
        
        particleSystemEmpty = new GameObject("Particle System");
        particleSystemEmpty.transform.SetParent(objectPoolEmptyHolder.transform); 
    }

    public T SpawnObject<T>(GameObject objectToSpawn, Vector2 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None) where T : Component
    {
        // Find an existing pool for the object
        PooledObjectInfo pool = objectPools.Find(p => p.LookupString == objectToSpawn.name);

        // Create a new pool if it doesn't exist
        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            objectPools.Add(pool);
        }

        // Check for an inactive object in the pool
        GameObject spawnableObject = pool.InactiveObjects.FirstOrDefault();

        // Create a new object if no inactive object is available
        if (spawnableObject == null)
        {
            //decide what type of parent will be used to contain this object
            GameObject parentObject = SetParentObject(poolType);

            //spawn object
            spawnableObject = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

            if(parentObject != null)
            {
                spawnableObject.transform.SetParent(parentObject.transform);
            }
        }
        else
        {
            // Reuse the inactive object
            spawnableObject.transform.position = spawnPosition;
            spawnableObject.transform.rotation = spawnRotation;

            // Remove it from the inactive list since it's now active
            pool.InactiveObjects.Remove(spawnableObject);
            spawnableObject.SetActive(true);
        }

        // return the object as its type
        return spawnableObject.GetComponent<T>();
    }

    public void ReturnObjectToPool(GameObject objectToReturn)
    { 
        //the object name has (clone), and to match the lookup string we have to remove it
        string modifiedObjectName = objectToReturn.name.Substring(0, objectToReturn.name.Length - 7);

        PooledObjectInfo pool = objectPools.Find(p => p.LookupString == modifiedObjectName);

        if( pool == null)
        {
            Debug.LogWarning("This object does not belong to the pool (" + objectToReturn.name + ")");
        }
        else
        {
            objectToReturn.SetActive(false);
            pool.InactiveObjects.Add(objectToReturn);
        }
    }

    private GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.None:
                return null;

            case PoolType.GameObject:
                return gameObjectsEmpty;

            case PoolType.ParticleSystem:
                return particleSystemEmpty;
            
            default:
                return null;
        }
    }
}

public class PooledObjectInfo
{
    public string LookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}

public enum PoolType
{
    None = 0,
    GameObject = 1,
    ParticleSystem = 2,
}

