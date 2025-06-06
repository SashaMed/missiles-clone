using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : Projectile
{

    protected override void Move() {}

    protected override void OnTimeToLiveReachZero()
    {
        base.OnTimeToLiveReachZero();
        gameObject.SetActive(false);
        ReturnToPool();
    }

    protected override void OnHit()
    {
        base.OnHit();
        gameObject.SetActive(false);
        ReturnToPool();
    }
}
