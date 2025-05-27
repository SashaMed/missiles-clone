using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;

[Serializable] public class UIStatesBool : UIStates<bool> { }
[Serializable] public class UIStatesString : UIStates<string> { }
public enum BoolWithProccess { OFF, PROCCESS, ON }
[Serializable] public class UIStatesBoolWithProccess : UIStates<BoolWithProccess> { }

[Serializable]
public class UIStates<TKey> : Hidden.UIStatesBase
{
    [Serializable]
    public class UIState
    {
        public TKey Key;

        [SerializeField]
        public Transform[] ShowObjects;
    }

    [SerializeField]
    private UIState[] entries;

    public event Action<TKey> OnStateChanged;
    public TKey Value => targetKey;

    private TKey targetKey;
    private Action targetCallback;

    private UIState current;


    public void SetState(TKey key, Action callback = null)
    {
        targetKey = key;
        targetCallback = callback;


        SetStateProccess(key, callback);

    }

    /// <summary>
    /// Ignore current state 
    /// </summary>
    public void SetStateImmediately(TKey key, Action callback = null)
    {
        current = null;

        SetState(key, callback);
    }

    private async void SetStateProccess(TKey key, Action callback = null)
    {
        if (entries.Length == 0) return; // not configurated
        if (current != null && current.Key.Equals(key)) return;

        var previos = current;
        current = GetStateEntry(key);


        SetStateObjects(current);

        OnStateChanged?.Invoke(current.Key);
        callback?.Invoke();

        // Check to replace state
        if (!key.Equals(targetKey)) SetState(targetKey);
    }

    private UIState GetStateEntry(TKey key)
    {
        return entries.FirstOrDefault(state => state.Key.Equals(key)) ?? entries[0];
    }

    private void SetStateObjects(UIState stateEntry)
    {
        var objects = new Transform[] { }.AsEnumerable();
        entries.ForEach(state => objects = objects.Union(state.ShowObjects.Where(o => o != null)));

        objects.ForEach(obj => obj.gameObject.SetActive(stateEntry.ShowObjects.Contains(obj)));

    }


}

namespace Hidden
{

    public abstract class UIStatesBase
    {

    }
}
