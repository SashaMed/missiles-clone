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
    private readonly List<IThreat> _active = new();
    private UniTaskVoid coreCycle;
    private int _currentIteration;

    protected override void StartCoreLoop()
    {
        if (config == null)
        {
            Debug.LogError("CoreLoopConfig is not set in SinglePlayerCoreGameplayManager.");
            return;
        }

        coreCycle = StartCoreLoopAsync();
    }

    public override void OnPLayerDeath(PlayerController player)
    {
        Model.SessionManager.EndSession();
    }

    public override void Refresh()
    {
        base.Refresh();
        _currentIteration = 0;
    }

    private async UniTaskVoid StartCoreLoopAsync()
    {
        while (true)
        {
            int budget = Mathf.RoundToInt(config.budgetCurve.Evaluate(_elapsed));

            var thisBatch = new List<IThreat>();

            while (budget > 0 && _active.Count < config.maxActive)
            {
                var threatConfig = PickThreat(budget);
                if (threatConfig == null) break;

                var threatPrefab = Instantiate(threatConfig.threatPrefab, ContentHolder);
                var threat = threatConfig.threatPrefab.GetComponent<IThreat>();
                threat.StartThreat(this);
                budget -= threatConfig.cost;
                _active.Add(threat);
                thisBatch.Add(threat);
                threat.OnEnded += t => _active.Remove(t);
            }


            var tracker = new ThreatBatchTracker(thisBatch, 0.5f);
            await tracker.WaitTask;


            await UniTask.Delay(TimeSpan.FromSeconds(config.iterationDelay));
            _elapsed += config.iterationDelay;
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
}



public class ThreatBatchTracker
{
    private readonly int _initialCount;
    private readonly int _finishCount;               
    private readonly TaskCompletionSource<bool> _tcs = new();
    public Task WaitTask => _tcs.Task;

    public ThreatBatchTracker(IEnumerable<IThreat> threats, float remainingFraction = 0.5f)
    {
        var list = threats.ToList();
        _initialCount = list.Count;
        _finishCount = Mathf.CeilToInt(_initialCount * remainingFraction);

        foreach (var t in list)
            t.OnEnded += OnThreatEnded;
    }

    private int _alive;
    private void OnThreatEnded(IThreat t)
    {
        t.OnEnded -= OnThreatEnded;
        if (Interlocked.Decrement(ref _alive) <= _finishCount)
            _tcs.TrySetResult(true);
    }
}