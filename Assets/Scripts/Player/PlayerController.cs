using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float forwardSpeed = 70f;
    [SerializeField] float turnSpeed = 150;

    private PlayerInputHandler playerInputHandler;
    private RollComponent playerRoll;
    private Core core;


    private void Awake()
    {
        core = GetComponentInChildren<Core>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
        playerRoll = GetComponent<RollComponent>();
    }

    private void Start()
    {
        InitCoreComponents();
    }


    void Update()
    {
        core.Movement.SetMovementInput(playerInputHandler.RawMovementInput);
        core.Movement.SetTurnInput(playerInputHandler.TurnInput);
        core.LogicUpdate();
    }

    private void InitCoreComponents()
    {
        core.Movement.Init(transform, forwardSpeed, turnSpeed);
        core.Stats.onHealthZero += OnDeath;
    }

    public void OnDeath(GameObject coreGO)
    {
        gameObject.SetActive(false);
    }
}
