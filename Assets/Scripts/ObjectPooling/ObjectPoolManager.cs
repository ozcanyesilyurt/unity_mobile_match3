using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{

    public static List<ObjectPool> ObjectPools = new List<ObjectPool>();

    #region pool object parenting

    public enum PoolType
    {
        Background,
        Obstacle,
        Tile,
        None
    }

    private GameObject _objectPoolEmptyHolder;
    private static GameObject _backgroundsEmpty;
    private static GameObject _obstaclesEmpty;
    private static GameObject _tilesEmpty;

    private void Awake()
    {
        SetupEmpties();
    }

    private void SetupEmpties()
    {
        _objectPoolEmptyHolder = new GameObject("Pooled Objects");

        _backgroundsEmpty = new GameObject("Backgrounds");
        _backgroundsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _obstaclesEmpty = new GameObject("Obstacles");
        _obstaclesEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _tilesEmpty = new GameObject("Tiles");
        _tilesEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
    }

    private static GameObject SetParentObject(PoolType pooltype)
    {
        switch (pooltype)
        {
            case PoolType.Background:
                return _backgroundsEmpty;
            case PoolType.Obstacle:
                return _obstaclesEmpty;
            case PoolType.Tile:
                return _tilesEmpty;
            case PoolType.None:
                return null;
            default:
                return null;
        }
    }

    #endregion

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnposition, Quaternion spawnRotation, PoolType poolType = PoolType.None) //spawn by type enum for parenting
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

            GameObject parentObject = SetParentObject(poolType); //addition for pool object parenting - 1

            spawnableObj = Instantiate(objectToSpawn, spawnposition, spawnRotation);

            if (parentObject != null)  //addition for pool object parenting - 2
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
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

    public static GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform) //spawn by Parent
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
            spawnableObj = Instantiate(objectToSpawn, parentTransform);
        }
        else
        {
            spawnableObj.transform.SetParent(parentTransform);
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }



    public static void ReturnObjectToPool(GameObject obj)
    {
        string gameObjectName = obj.name.Replace("(Clone)", "").Trim();

        ObjectPool pool = ObjectPools.Find(x => x.lookupString == gameObjectName);

        if (pool == null)
        {
            Debug.LogWarning($"Trying to release an object that is not pooled: {obj.name}");
        }
        else
        {
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }

    }

}

public class ObjectPool
{
    public string lookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}
