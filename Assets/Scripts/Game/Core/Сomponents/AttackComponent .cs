using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : CoreComponent
{
    [SerializeField] private WeaponConfig config;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private SimplePool bulletsPool;

    private float _lastShotTime;
    private float _nextBurstTime;
    private int _shotsRemaining;
    private bool _attackInput;



    public void Init(Transform bulletsHolder)
    {
        if (!bulletsPool.IsActive)
        {
            bulletsPool.InitPool(config.projectilePrefab, bulletsHolder, config.burstCount);
        }
    }


    public void SetAttackInput(bool val)
    {
        _attackInput = val;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!_attackInput)
        {
            return;
        }

        if (Time.time < _nextBurstTime)
        {
            return;
        }

        if (_shotsRemaining <= 0)
        {
            _shotsRemaining = config.burstCount;
            _lastShotTime = Time.time - config.interval;
        }

        if (Time.time >= _lastShotTime + config.interval)
        {
            SpawnProjectiles();
            _lastShotTime = Time.time;
            _shotsRemaining--;
            if (_shotsRemaining <= 0)
            {
                _nextBurstTime = Time.time + config.cooldown;
            }
        }
    }

    private void SpawnProjectiles()
    {
        if (config == null || config.projectilePrefab == null)
            return;

        foreach (var point in spawnPoints)
        {
            if (point == null) continue;
            var go = bulletsPool.Get((bullet) => {
                bullet.SetActive(false);
                bullet.transform.position = point.position;
                bullet.transform.rotation = point.rotation;
                bullet.SetActive(true);
            });
        }
    }
}