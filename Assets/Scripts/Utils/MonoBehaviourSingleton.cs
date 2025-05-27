using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
            Debug.LogError($"Singleton '{typeof(T).Name}' trying initialise twice!");
    }
}
