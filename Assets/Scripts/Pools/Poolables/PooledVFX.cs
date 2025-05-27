using System.Collections;
using System.Linq;
using UnityEngine;

public class PooledVFX : MonoBehaviour, IPoolableGO
{
    [SerializeField] private ParticleSystem[] systems;

    public SimplePool ParentPool { get; set; }

    public void ResetPoolable()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {
        if (ParentPool == null)
        {
            return;
        }
        ParentPool.AddToPool(gameObject);
    }


    void Start()
    {
        StartCoroutine(WaitAndReturn());
    }

    private IEnumerator WaitAndReturn()
    {
        yield return new WaitUntil(() => systems.All(ps => !ps.IsAlive(true)));
        ReturnToPool();
    }
}
