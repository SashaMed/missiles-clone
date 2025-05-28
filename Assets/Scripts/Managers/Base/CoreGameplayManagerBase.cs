using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoreGameplayManagerBase : Entity<GameCoreModel>
{

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
    
    public PlayerController Player { get; set; }
    public SessionManagerBase SessionManager { get; set; }
}