using System.Collections;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    [SerializeField] Transform target;
   
    [Space]
    [SerializeField] float forwardSpeed = 70f;
    [SerializeField] float turnSpeed = 150;

    [Header("Missile Settings")]
    [SerializeField] float lifetime = 5f;

    [Space]
    [SerializeField] private ParticleSystem trailParticles;

    [Space]
    [SerializeField] private GameObject missileModel;

    [Space]
    [SerializeField] float afterHitTrailWorkingTime = 0.1f;

    private float lifeTimer;

    private Core core;

    private bool isActive = true;
    private float originalEmissionRateOverTime;


    private void Awake()
    {
        core = GetComponentInChildren<Core>();
        trailParticles = GetComponentInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        lifeTimer = 0f;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
        {
            return;
        }
        if (damageable.Damage(100, transform.name))
        {
            OnMissileHits();
        }
    }

    private void Start()
    {
        isActive = true;
        core.Movement.Init(transform, forwardSpeed, turnSpeed);
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        SetMovementInput();
        core.LogicUpdate();
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

    private void OnMissileHits()
    {
        DisableTrail();
        isActive = false;
        missileModel.SetActive(false);
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
        var rb = missileModel.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            float power = 100;
            float rotatePower = 1f; 
            float randomizeDirection = 1f;

            rb.velocity = (missileModel.transform.forward + UnityEngine.Random.insideUnitSphere * 0.5f * randomizeDirection) * power;
            rb.AddTorque(UnityEngine.Random.onUnitSphere * 5 * rotatePower);
        }
        isActive = false;
        DisableTrail();
    }
}
