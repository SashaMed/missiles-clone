using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileThreat : MonoBehaviour, IThreat
{
    [SerializeField] private MissileController missilePrefab;
    [SerializeField] private float distanceToPlayer = 10f;

    public event Action<IThreat> OnThreatEnded;

    private MissileController missileController;

    public void StartThreat(CoreGameplayManagerBase coreManager)
    {
        var player = coreManager.Model.Player.transform;

        var dir2D = UnityEngine.Random.insideUnitCircle.normalized;
        var offset = new Vector3(dir2D.x, 0, dir2D.y) * distanceToPlayer;
        var spawnPos = player.position + offset;


        var toPlayer = player.position - spawnPos;
        toPlayer.y = player.position.y;                                 

        var yawOnly = Quaternion.LookRotation(toPlayer); 

        missileController = Instantiate(missilePrefab, spawnPos, yawOnly, coreManager.ContentHolder);
        missileController.SetTarget(player.transform);
        missileController.OnTimeToLiveReachZeroAction += MissileStopMoving;
    }


    public void MissileStopMoving()
    {
        missileController.OnTimeToLiveReachZeroAction += MissileStopMoving;
        OnThreatEnded?.Invoke(this);
    }

    private void OnDestroy()
    {
        if (missileController != null)
        {
            missileController.OnTimeToLiveReachZeroAction -= MissileStopMoving;
        }
    }

    public void EarlyThreatDisable(CoreGameplayManagerBase coreManager)
    {
        throw new NotImplementedException();
    }
}
