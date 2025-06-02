using System;
using System.Collections;
using UnityEngine;

public class MissileController : MonoBehaviour, IThreat
{
    [SerializeField] Transform target;
   
    [Space]
    [SerializeField] float forwardSpeed = 70f;
    [SerializeField] float turnSpeed = 150;

    [Header("Missile Settings")]
    [SerializeField] float lifetime = 5f;

    [Space]
    [SerializeField] float spawnDistanceToPlayer = 200;

    [Space]
    [SerializeField] private ParticleSystem trailParticles;

    [Space]
    [SerializeField] private GameObject missileModel;

    [Space]
    [SerializeField] float afterHitTrailWorkingTime = 0.1f;

    public Action OnTimeToLiveReachZeroAction;

    private float lifeTimer;

    private Core core;

    private bool isActive = true;
    private float originalEmissionRateOverTime;

    public event Action<IThreat> OnThreatEnded;

    public void StartThreat(CoreGameplayManagerBase coreManager)
    {
        var player = coreManager.Model.Player.transform;

        var dir2D = UnityEngine.Random.insideUnitCircle.normalized;
        var offset = new Vector3(dir2D.x, 0, dir2D.y) * spawnDistanceToPlayer;
        var spawnPos = player.position + offset;


        var toPlayer = player.position - spawnPos;
        toPlayer.y = player.position.y;

        var yawOnly = Quaternion.LookRotation(toPlayer);
        transform.position = spawnPos;
        transform.rotation = yawOnly;
        SetTarget(player.transform);
    }

    public void EarlyThreatDisable(CoreGameplayManagerBase coreManager)
    {
        Destroy(gameObject);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isActive)
        {
            return;
        }
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
        {
            return;
        }
        if (damageable.Damage(100, transform.name))
        {
            Debug.Log($"Missile {transform.name} hit {other.transform.parent.parent.name} and dealt damage.");
            OnMissileHits();
        }
    }

    private void Awake()
    {
        core = GetComponentInChildren<Core>();
        trailParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        isActive = true;
        lifeTimer = 0f;
        core.Stats.onHealthZero += OnMissileDeath;
        core.Movement.Init(transform, forwardSpeed, turnSpeed);
    }

    private void Update()
    {
        core.LogicUpdate();
        if (!isActive)
        {
            return;
        }

        SetMovementInput();
        CheckLifetime();
    }

    private void OnDisable()
    {
        core.Stats.onHealthZero -= OnMissileDeath;
    }

    private void CheckLifetime()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
            OnTimeToLiveReachZero();
    }

    private void SetMovementInput()
    {
        if (target == null)
        {
            core.Movement.SetMovementInput(Vector2.zero);
        }
        else
        {
            Vector3 toTarget = target.position - transform.position;
            Vector2 dir2D = new Vector2(toTarget.x, toTarget.z).normalized;
            core.Movement.SetMovementInput(dir2D);
        }
    }

    private void OnMissileDeath(GameObject core)
    {
        Debug.Log($"Missile {transform.name} was hit.");
        OnMissileHits();
    }

    private void OnMissileHits()
    {
        DisableMissile();
        missileModel.SetActive(false);
    }

    private void DisableMissile()
    {
        OnThreatEnded?.Invoke(this);
        DisableTrail();
        core.gameObject.SetActive(false);
        isActive = false;
    }

    private void DisableTrail()
    {
        if (trailParticles == null)
        {
            return;
        }

        StartCoroutine(DisableTrailCoroutine());

    }

    private IEnumerator DisableTrailCoroutine()
    {
        yield return new WaitForSeconds(afterHitTrailWorkingTime);

        var emission = trailParticles.emission;
        originalEmissionRateOverTime = emission.rateOverTime.constant;
        emission.rateOverTime = 0;

        var trailLifetime = trailParticles.main.startLifetime.constant;

        yield return new WaitForSeconds(trailLifetime);
        ResetMissile();
    }

    private void ResetMissile()
    {
        gameObject.SetActive(false);

        if (trailParticles != null)
        {
            var emission = trailParticles.emission;
            emission.rateOverTime = originalEmissionRateOverTime;
        }
    }

    private void OnTimeToLiveReachZero()
    {
        DropMissile();
        DisableMissile();
    }


    private void DropMissile()
    {
        var rb = missileModel.GetComponent<Rigidbody>();
        if (rb == null)
        {
            return;
        }
        rb.isKinematic = false;
        rb.useGravity = true;

        float power = 100;
        float rotatePower = 1f;
        float randomizeDirection = 1f;

        rb.velocity = (missileModel.transform.forward + UnityEngine.Random.insideUnitSphere * 0.5f * randomizeDirection) * power;
        rb.AddTorque(UnityEngine.Random.onUnitSphere * 5 * rotatePower);
    }
}
