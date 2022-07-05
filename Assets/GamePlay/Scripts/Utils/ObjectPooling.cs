using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    #region Singleton
    public static ObjectPooling ins;
    public void Awake()
    {
        ins = this;
    }
    #endregion

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool p in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < p.size; i++)
            {
                GameObject obj = Instantiate(p.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(p.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag)
    {
        GameObject objToSpawn = null;
        if (poolDictionary[tag].Count == 0)
        {
            foreach (Pool p in pools)
            {
                if (p.tag.Equals(tag))
                {
                    objToSpawn = Instantiate(p.prefab);
                    break;
                }
                else continue;
            }
        }
        else
        {
            objToSpawn = poolDictionary[tag].Dequeue();
        }
        objToSpawn.SetActive(true);

        return objToSpawn;
    }

    public void EnQueueObj(string tag, GameObject objToEnqueue)
    {
        poolDictionary[tag].Enqueue(objToEnqueue);
        objToEnqueue.SetActive(false);
    }
}
