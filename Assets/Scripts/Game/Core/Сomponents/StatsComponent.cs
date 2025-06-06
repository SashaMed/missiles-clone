using System;
using System.Collections;
using UnityEngine;

public class StatsComponent : CoreComponent
{
    public event Action<GameObject> onHealthZero;

    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }

    public float MaxMana { get => maxMana; private set => maxMana = value; }

    public float CurrentMana { get => currentMana; private set => currentMana = value; }

    public float CurrentHealth { get => currentHealth; private set => currentHealth = value; }

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxMana = 100f;
    
    [Space]
    [SerializeField] private float manaRestoreTime = 1f;
    [SerializeField] private float healthRestoreTime = 1f;

    [Space]
    [SerializeField] private float manaRestoreAmount = 1f;
    [SerializeField] private float healthRestoreAmount = 1f;

    [Space]
    [SerializeField] private GlobalPoolType deathVFXType = GlobalPoolType.None;


    private float currentHealth;
    private float currentMana;

    private int deathCount;
    private bool isDead = false;
    private bool ignoreSetStats = false;
    public bool IsDead { get => isDead; }

    private float _totalReceivedDamage;
    private string _lastHealthChangeSender;

    public string LastHealthChangeSender => _lastHealthChangeSender;
    public float TotalReceivedDamage => _totalReceivedDamage;

    protected override void Awake()
    {
        base.Awake();
        SetHealth();
    }

    private void Start()
    {
        InitStats();
    }

    private void SetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    public void InitStats()
    {
        SetHealth();
        if (core.isPlayerComponent)
        {
            StartCoroutine(RestoreHealthCoroutine());
            StartCoroutine(ReviveManaCoroutine());
        }
    }

    private IEnumerator ReviveManaCoroutine()
    {
        while (true)
        {
            if (currentMana < maxMana)
            {
                ChangeMana(manaRestoreAmount);
            }
            yield return new WaitForSeconds(manaRestoreTime);
        }
    }


    private IEnumerator RestoreHealthCoroutine()
    {
        while (true)
        {
            if (currentHealth < maxHealth)
            {
                ChangeHealth(healthRestoreAmount, "restore");
            }
            yield return new WaitForSeconds(healthRestoreTime);
        }
    }

    public void ChangeHealth(float amount, string sender)
    {
        _lastHealthChangeSender = sender;

        if (amount < 0)
        {
            _totalReceivedDamage += Math.Abs(amount);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (currentHealth <= 0 && !isDead)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        isDead = true;
        currentHealth = 0;
        GlobalPool.Instance.Get(deathVFXType, transform.position);
        //ParticlePool.Instance.TakeParticlesPool.GetFromPoolWithRandowRotation(transform, core.ParticleManager.bloodParticlesColor);
        //core.SoundComponent.StopAllSounds();
        onHealthZero?.Invoke(core.gameObject);
        deathCount++;
    }

    private void ChangeMana(float amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
    }


    public bool Heal(float amount)
    {
        if (currentHealth == maxHealth)
        {
            return false;
        }
        ChangeHealth(amount, "heal");
        return true;
    }


    public void SetStats(float health, float mana, bool ignoreNextSetStats = false)
    {
        if (ignoreSetStats)
        {
            return;
        }
        ignoreSetStats = ignoreNextSetStats;
        if (health > 0)
        {
            maxHealth = health;
            currentHealth = maxHealth;
        }
        if (mana > 0)
        {
            MaxMana = mana;
            currentMana = MaxMana;
        }

    }

    public void SetRestoreHealthStats(float health, float mana, float interval)
    {
        StopAllCoroutines();
        manaRestoreTime = interval;
        healthRestoreAmount = health;
        manaRestoreAmount = mana;
        StartCoroutine(RestoreHealthCoroutine());
        StartCoroutine(ReviveManaCoroutine());
    }

    public bool ManaHeal(float amount)
    {
        if (currentMana == maxMana)
        {
            return false;
        }
        ChangeMana(amount);
        return true;
    }
}
