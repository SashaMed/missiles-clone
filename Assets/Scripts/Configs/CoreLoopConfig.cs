using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CoreLoopConfig", menuName = "Configs/CoreLoopConfig")]
public class CoreLoopConfig : ScriptableObject
{
    public AnimationCurve budgetCurve;

    public float iterationDelay;
    public int maxActive = 60;

    public List<ThreatEntry> threats;


    [Serializable]
    public class ThreatEntry
    {
        public GameObject threatPrefab;   
        
        [Min(1)] 
        public int cost = 1;
        public int startIteration = 0;
        public bool blocksNextCycle;
    }

}
