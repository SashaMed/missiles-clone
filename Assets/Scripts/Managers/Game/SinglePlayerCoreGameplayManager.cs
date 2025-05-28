using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerCoreGameplayManager : CoreGameplayManagerBase
{

    protected override void StartCoreLoop()
    {
        
    }

    public override void OnPLayerDeath(PlayerController player)
    {
        Model.SessionManager.EndSession();
    }
}
