using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoreGameplayManagerBase : Entity<GameCoreModel>
{
    [SerializeField] private Transform _contentHolder;

    public Transform ContentHolder => _contentHolder;
    protected abstract void StartCoreLoop();

    public abstract void OnPLayerDeath(PlayerController player);

    public override void Refresh()
    {
        StartCoreLoop(); 
        Model.Player.SetModel(new PlayerModel
        {
            coreManager = this,
            isActive = true
        });
    }
}


public class GameCoreModel
{
    public Transform GameContentHolder;

    public Transform ManagersContentHolder;

    public PlayerController Player { get; set; }
    public SessionManagerBase SessionManager { get; set; }
}