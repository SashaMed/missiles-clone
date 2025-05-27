using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoBehaviour
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int countOfrefabs = 10;
    [SerializeField] private PoolType poolType;


    public PoolType PoolType => poolType;

    protected List<GameObject> poolItems = new List<GameObject>();
    protected Dictionary<GameObject, IPoolableGO> ipoolables = new Dictionary<GameObject, IPoolableGO>();

    private bool isActive;

    protected virtual void Awake()
    {
        GrowPool();
    }

    protected virtual void GrowPool()
    {
        if (prefab == null)
        {
            Debug.LogWarning($"At {gameObject.name} prefab is null");
            return;
        }

        var interfaceComponent = prefab.GetComponent<IPoolableGO>();
        if (interfaceComponent == null)
        {
            Debug.LogWarning($"At {gameObject.name} prefab {prefab.gameObject.name} does not implement IPoolableGO interface");
            return;
        }

        for (int i = 0; i < countOfrefabs; i++)
        {
            var instanceToAdd = Instantiate(prefab);
            instanceToAdd.transform.SetParent(transform);

            var ipoolable = instanceToAdd.GetComponent<IPoolableGO>();
            if (ipoolable != null)
            {
                ipoolables.Add(instanceToAdd, ipoolable);
                ipoolable.ParentPool = this;
            }

            AddToPool(instanceToAdd);
        }
        isActive = true;
    }

    private GameObject Get()
    {
        if (poolItems.Count == 0)
        {
            GrowPool();
        }
        var go = poolItems[poolItems.Count - 1];
        poolItems.RemoveAt(poolItems.Count - 1);
        return go;
    }

    public virtual void AddToPool(GameObject obj)
    {
        obj.SetActive(false);

        if (!isActive)
        {
            return;
        }

        if (ipoolables.TryGetValue(obj, out var ipoolable))
        {
            ipoolable.ResetPoolable();
        }

        poolItems.Add(obj);
    }

    public virtual GameObject Get(Vector3 position)
    {
        if (!isActive)
        {
            return null;
        }
        var go = Get();
        go.SetActive(true);
        go.transform.position = position;
        return go;
    }


    public virtual GameObject Get(Action<GameObject> initializer)
    {
        if (!isActive)
        {
            return null;
        }
        var go = Get();

        go.SetActive(true);
        initializer?.Invoke(go);
        return go;
    }
}

public interface IPoolableGO
{
    SimplePool ParentPool { get; set; }

    void ReturnToPool();

    void ResetPoolable();
}