using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject Prefab;
        public int Size;
    }
    [System.Serializable]
    public class Group
    {
        public string Tag;
        public List<Pool> Pools;
    }
    #region Singleton
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    //public GameObject Player;
    public List<Group> Groups;
    public Dictionary<string, Queue<GameObject>> PoolDictionary;


    // Start is called before the first frame update
    private void Start()
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Group g in Groups)
        {
            foreach (Pool pool in g.Pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int i = 0; i < pool.Size; i++)
                {
                    GameObject obj = Instantiate(pool.Prefab, gameObject.transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                PoolDictionary.Add(pool.Tag, objectPool);
            }
        }

    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, bool DontSpawnIfActive = false)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't excist.");
            return null;
        }
        GameObject objectToSpawn = PoolDictionary[tag].Dequeue();
        if (DontSpawnIfActive)
        {
            int Count = 0;
            while (objectToSpawn.activeSelf == true && Count < 200)
            {
                PoolDictionary[tag].Enqueue(objectToSpawn);
                objectToSpawn = PoolDictionary[tag].Dequeue();
                Count++;
            }
            if (Count == 200) { PoolDictionary[tag].Enqueue(objectToSpawn); return objectToSpawn; }
        }
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IpooledObject pooledObj = objectToSpawn.GetComponent<IpooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        PoolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Vector3 Scale, bool DontSpawnIfActive = false)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't excist.");
            return null;
        }
        GameObject objectToSpawn = PoolDictionary[tag].Dequeue();
        if (DontSpawnIfActive)
        {
            int Count = 0;
            while (objectToSpawn.activeSelf == true && Count < 200)
            {
                PoolDictionary[tag].Enqueue(objectToSpawn);
                objectToSpawn = PoolDictionary[tag].Dequeue();
                Count++;
            }
        }
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.localScale = Scale;

        IpooledObject pooledObj = objectToSpawn.GetComponent<IpooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        PoolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}

public interface IpooledObject
{

    public void OnObjectSpawn();

}