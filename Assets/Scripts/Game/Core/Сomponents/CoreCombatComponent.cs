using System;
using UnityEngine;

public class CoreCombatComponent : CoreComponent, IDamageable
{
    //public event Action onStun;
    public event Func<bool> onGettingDamage;

    [HideInInspector] public float lastDamageTime;

    public bool Damage(float amount, string sender)
    {
        if (onGettingDamage != null)
        {
            if (onGettingDamage.Invoke())
                return false;
        }

        DoDamage(amount, sender);
        return true;
    }

    public bool CriticalDamage(float damageFmount, string sender)
    {
        if (onGettingDamage != null)
        {
            if (onGettingDamage.Invoke())
                return false;
        }
        //core.SoundComponent.PlayCriticalHitSound();
        //ParticlePool.Instance.CriticalHitParticlesPool.GetFromPoolWithoutColorChange(transform);
        //PopupTextPool.Instance.GetFromPoolWithRandom(
        //    transform.position,
        //    LocalizationManager.Instance.GetCurrentLocaleString("crit_msg_id"),
        //    Color.red, 6);
        //lastDamageTime = Time.time;
        //core.ParticleManager.PlayBloodParticleFromPool();
        core.Stats.ChangeHealth(-damageFmount, sender);
        //DoDamage(damageFmount);
        return true;
    }

    private void DoDamage(float amount, string sender)
    {
        //if (core.isPlayerComponent)
        //{
        //    MainCameraScript.Instance.ShakeCamera();
        //}
        lastDamageTime = Time.time;
        //core.SoundComponent.PlayHitSound();
        //core.ParticleManager.PlayBloodParticleFromPool();
        core.Stats.ChangeHealth(-amount, sender);

    }

    public bool IsComponentActive()
    {
        return !core.Stats.IsDead;
    }

    //public void DamageWithoutEffects(float amount, string sender)
    //{
    //    //if (onGettingDamage != null)
    //    //{
    //    //    if (onGettingDamage.Invoke())
    //    //        return;
    //    //}
    //    if (Time.time >= lastDamageTime + effectsCooldown)
    //    {
    //        core.SoundComponent.PlayHitSound();
    //        core.ParticleManager.PlayBloodParticleFromPool();
    //        lastDamageTime = Time.time;
    //    }

    //    core.Stats.DecreaseHealth(amount, sender);

    //}
}
