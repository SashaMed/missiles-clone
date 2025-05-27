using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    bool CriticalDamage(float damageFmount, string sender)
    {
        Damage(damageFmount, sender);
        return false;
    }

    bool Damage(float amount, string sender);
    //void ObstaclesDamage(float amount, float knockbackForce, string sender);
    //bool IsComponentActive();

    //void CheckIfShouldStun(float stunDamage);
}
