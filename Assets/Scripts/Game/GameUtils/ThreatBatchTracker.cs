using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime;

public class ThreatBatchTracker
{
    private readonly int _initialCount;
    private readonly int _finishCount;
    private readonly TaskCompletionSource<bool> _tcs = new();
    public Task WaitTask => _tcs.Task;
    private readonly CancellationToken _token;

    public ThreatBatchTracker(IEnumerable<IThreat> threats, CancellationToken token = default, float remainingFraction = 0.5f)
    {
        var list = threats.ToList();
        _initialCount = list.Count;
        _finishCount = Mathf.CeilToInt(_initialCount * remainingFraction);
        _token = token;

        foreach (var t in list)
            t.OnThreatEnded += OnThreatEnded;

        if (_token.CanBeCanceled)
        {
            _token.Register(() =>
            {
                //Debug.Log("ThreatBatchTracker: Cancelled by token.");
                _tcs.TrySetCanceled(_token);
            });
        }
    }

    private int _alive;
    private void OnThreatEnded(IThreat t)
    {
        t.OnThreatEnded -= OnThreatEnded;
        if (Interlocked.Decrement(ref _alive) <= _finishCount)
        {
            _tcs.TrySetResult(true);
        }
    }
}