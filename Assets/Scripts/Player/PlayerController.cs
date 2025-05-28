using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity<PlayerModel>
{

    [SerializeField] float forwardSpeed = 70f;
    [SerializeField] float turnSpeed = 150;

    private PlayerInputHandler playerInputHandler;
    private Core core;


    private void Awake()
    {
        core = GetComponentInChildren<Core>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
        InitCoreComponents();
    }

    private bool IsActive()
    {
        return Model != null && Model.isActive;
    }


    void Update()
    {
        if (IsActive())
        {
            ModelActiveUpdate();
        }

        core.LogicUpdate();
    }

    private void ModelActiveUpdate()
    {
        core.Movement.SetMovementInput(playerInputHandler.RawMovementInput);
        core.Movement.SetTurnInput(playerInputHandler.TurnInput);
    }

    private void InitCoreComponents()
    {
        core.Movement.Init(transform, forwardSpeed, turnSpeed);
        core.Stats.onHealthZero += OnDeath;
    }

    public void OnDeath(GameObject coreGO)
    {
        gameObject.SetActive(false);
        Model.coreManager.OnPLayerDeath(this);
    }

    public override void Refresh()
    {
        gameObject.SetActive(true);
        core.Stats.InitStats();
    }
}

public class PlayerModel 
{
    public CoreGameplayManagerBase coreManager;

    public bool isActive;
}