using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalPool : MonoBehaviourSingleton<GlobalPool>
{
    [SerializeField] private SimplePool simplePoolPrefab;

    [Space]
    [SerializeField] private SimplePool bigExposionPool;
    [SerializeField] private SimplePool smallExposionPool;

    public SimplePool BigExplosionPool => bigExposionPool;
    public SimplePool SmallExplosionPool => smallExposionPool;

    private List<SimplePool> pools = new List<SimplePool>();

    private void Start()
    {
        pools.Add(bigExposionPool);
        pools.Add(smallExposionPool);    
    }

    public GameObject Get(string poolType, Vector3 position)
    {
        return pools.FirstOrDefault(p => p.PoolId == poolType)?.Get(position);
    }

    public GameObject Get(string poolType, Action<GameObject> initializer)
    {
        return pools.FirstOrDefault(p => p.PoolId == poolType)?.Get(initializer);
    }


    public GameObject Get(GlobalPoolType poolType, Vector3 position)
    {
        return Get(poolType.ToString(), position);
    }

    public GameObject Get(GlobalPoolType poolType, Action<GameObject> initializer)
    {
        return Get(poolType.ToString(), initializer);
    }

    public SimplePool CreatePoolInstance(string poolName)
    {
        var pool = Instantiate(simplePoolPrefab, transform);
        pool.name = poolName;
        return pool;
    }
}


public enum GlobalPoolType
{
    None,
    BigExplosionVFX,
    SmallExplosionVFX,

}