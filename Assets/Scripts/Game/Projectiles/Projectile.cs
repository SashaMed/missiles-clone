using System;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float forwardSpeed = 70f;
    [SerializeField] protected float turnSpeed = 150f;
    [SerializeField] protected float lifetime = 5f;

    public Action OnTimeToLiveReachZeroAction;

    protected Core core;
    protected bool isActive = true;
    private float lifeTimer;

    protected virtual void Awake()
    {
        core = GetComponentInChildren<Core>();
    }

    protected virtual void Start()
    {
        isActive = true;
        lifeTimer = 0f;
        if (core != null)
        {
            core.Movement.Init(transform, forwardSpeed, turnSpeed);
        }
    }

    protected virtual void Update()
    {
        if (core != null)
        {
            core.LogicUpdate();
        }

        if (!isActive)
        {
            return;
        }

        Move();
        CheckLifetime();
    }

    protected abstract void Move();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isActive)
        {
            return;
        }
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
        {
            return;
        }
        if (damageable.Damage(100, transform.name))
        {
            OnHit();
        }
    }

    protected virtual void OnHit()
    {
        isActive = false;
    }

    protected virtual void OnTimeToLiveReachZero()
    {
        OnTimeToLiveReachZeroAction?.Invoke();
        isActive = false;
    }

    private void CheckLifetime()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
        {
            OnTimeToLiveReachZero();
        }
    }
}
