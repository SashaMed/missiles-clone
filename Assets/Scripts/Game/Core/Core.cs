using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public bool isPlayerComponent = false;
    public CoreMovementComponent Movement { get; private set; }
    public CoreCombatComponent Combat { get; private set; }
    public StatsComponent Stats { get; private set; }
    public AttackComponent Attack { get; private set; }

    //public CollisionSenses CollisionSenses { get; private set; }
    //public ParticleManager ParticleManager { get; private set; }
    //public Sound SoundComponent { get; private set; }
    //public Death Death { get; private set; }

    private List<CoreComponent> components = new List<CoreComponent>();


    private void Awake()
    {
        Combat = GetComponentInChildren<CoreCombatComponent>();
        Movement = GetComponentInChildren<CoreMovementComponent>();
        Stats = GetComponentInChildren<StatsComponent>();
        Attack = GetComponentInChildren<AttackComponent>();
        //CollisionSenses = GetComponentInChildren<CollisionSenses>();
        //ParticleManager = GetComponentInChildren<ParticleManager>();
        //SoundComponent = GetComponentInChildren<Sound>();
        //Death = GetComponentInChildren<Death>();
        //if (!Movement || !CollisionSenses)
        //{
        //    Debug.Log("no core component");
        //}
    }

    public void LogicUpdate()
    {
        foreach (var component in components)
        {
            component.LogicUpdate();
        }
    }

    public void AddComponent(CoreComponent comp)
    {
        if (!components.Contains(comp))
        {
            components.Add(comp);
        }
    }
}

