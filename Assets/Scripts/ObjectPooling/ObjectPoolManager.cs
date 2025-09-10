using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<ObjectPool> ObjectPools = new List<ObjectPool>();
    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnposition, Quaternion spawnRotation)
    {
        ObjectPool pool = ObjectPools.Find(x => x.lookupString == objectToSpawn.name);

        if (pool == null)
        {
            pool = new ObjectPool() { lookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            spawnableObj = Instantiate(objectToSpawn, spawnposition, spawnRotation);
        }
        else
        {
            spawnableObj.transform.position = spawnposition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

}

public class ObjectPool
{
    public string lookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}
