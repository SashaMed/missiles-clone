using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static CoreLoopConfig;
using System;

public class SinglePlayerCoreGameplayManager : CoreGameplayManagerBase
{

    [SerializeField] private CoreLoopConfig config;


    private float _elapsed;
    private List<IThreat> _active = new();
    private UniTaskVoid _coreCycle;
    private int _currentIteration;


    private Dictionary<GameObject, SimplePool> _poolsByPrefab = new Dictionary<GameObject, SimplePool>();
    private CancellationTokenSource _cts;

    protected override void StartCoreLoop()
    {
        if (config == null)
        {
            Debug.LogError("CoreLoopConfig is not set in SinglePlayerCoreGameplayManager.");
            return;
        }

        _cts?.Cancel(); 
        _cts = new CancellationTokenSource();
        _coreCycle = StartCoreLoopAsync(_cts.Token);
    }

    public override void KillCoreLoop()
    {
        _cts?.Cancel();
        var holder = Model.GameContentHolder;

        foreach (Transform child in holder)
        {
            GameObject.Destroy(child.gameObject);
        }
        Destroy(gameObject);
    }

    public override void OnPLayerDeath(PlayerController player)
    {
        _cts?.Cancel();
        Model.SessionManager.EndSession();
    }

    public override void Refresh()
    {
        DisableOldThreats();
        base.Refresh();
        _currentIteration = 0;
    }

    private void DisableOldThreats()
    {
        foreach (var threat in _active.ToList())
        {
            if (threat == null) continue;

            threat.EarlyThreatDisable(this);
        }
        _active = new List<IThreat>();
    }

    private async UniTaskVoid StartCoreLoopAsync(CancellationToken token)
    {
        while (Model.SessionManager.IsActive)
        {
            int budget = Mathf.RoundToInt(config.budgetCurve.Evaluate(_currentIteration));

            var thisBatch = new List<IThreat>();

            while (budget > 0 && _active.Count < config.maxActive)
            {
                var threatConfig = PickThreat(budget);
                if (threatConfig == null) break;

                var threatGO = GetTreat(threatConfig.threatPrefab);
                var threat = threatGO.GetComponent<IThreat>();
                threat.StartThreat(this);
                budget -= threatConfig.cost;
                _active.Add(threat);
                thisBatch.Add(threat);
                threat.OnThreatEnded += t => _active.Remove(t);
                Debug.Log($"Spawned threat[{_currentIteration}]: {threatConfig.threatPrefab.name}, Remaining budget: {_currentIteration}");
            }


            var tracker = new ThreatBatchTracker(thisBatch, token, 0.5f);
            await tracker.WaitTask;

            var iterationDelay = UnityEngine.Random.Range(config.minIterationDelay, config.maxIterationDelay);
            await UniTask.Delay(TimeSpan.FromSeconds(iterationDelay), cancellationToken: token);
            //_elapsed += iterationDelay;
            _currentIteration++;
        }
    }

    private ThreatEntry PickThreat(int availableBudget)
    {
        var candidates = new List<CoreLoopConfig.ThreatEntry>();
        var totalWeight = 0;

        foreach (var entry in config.threats)
        {
            if (entry == null || entry.threatPrefab == null)         
                continue;

            if (entry.startIteration > _currentIteration)             
                continue;

            if (!entry.threatPrefab.TryGetComponent<IThreat>(out var threatComp))
                continue;                                            

            if (entry.cost > availableBudget)                
                continue;

            candidates.Add(entry);
            totalWeight += Mathf.Max(1, entry.cost);                
        }


        if (candidates.Count == 0)
            return null;


        var roll = UnityEngine.Random.Range(0, totalWeight);
        foreach (var entry in candidates)
        {
            roll -= Mathf.Max(1, entry.cost);
            if (roll < 0)
                return entry;
        }


        return candidates[candidates.Count - 1];
    }


    private GameObject GetTreat(GameObject prefab)
    {
        if (_poolsByPrefab.TryGetValue(prefab, out var pool))
        {
            Debug.Log($"Pool for prefab: {prefab.name} already exist");
            return pool.Get((poolable) => { });
        }
        else
        {
            Debug.Log($"Creating new pool for prefab: {prefab.name}");
            var newPool = GlobalPool.Instance.CreatePoolInstance(prefab.name);
            newPool.InitPool(prefab, Model.GameContentHolder, 2);
            _poolsByPrefab[prefab] = newPool;
            return newPool.Get((poolable) => { });
        }
    }

}



