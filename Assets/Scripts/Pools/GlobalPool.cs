using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalPool : MonoBehaviourSingleton<GlobalPool>
{
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

    public GameObject Get(PoolType poolType, Vector3 position)
    {
        return pools.FirstOrDefault(p => p.PoolType == poolType)?.Get(position);
    }

    public GameObject Get(PoolType poolType, Action<GameObject> initializer)
    {
        return pools.FirstOrDefault(p => p.PoolType == poolType)?.Get(initializer);
    }
}


public enum PoolType
{
    None,
    BigExplosionVFX,
    SmallExplosionVFX,

}