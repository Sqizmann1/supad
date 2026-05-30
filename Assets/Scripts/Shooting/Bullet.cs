using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public float damage = 25f;
    public float lifetime = 3f;
    public LayerMask hitLayers;

    [Header("FX")]
    public GameObject impactVFX;

    private Rigidbody rb;
    private TrailRenderer trail;
    private float spawnTime;
    private bool hasHit;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();

        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnEnable()
    {
        spawnTime = Time.time;
        hasHit = false;
        if (trail) trail.Clear();
    }

    void Update()
    {
        if (Time.time - spawnTime >= lifetime)
            Deactivate();
    }

    // Поворачиваем пулю по направлению полёта
    void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        // Проверка слоёв
        if (hitLayers != 0 && (hitLayers & (1 << collision.gameObject.layer)) == 0)
            return;

        hasHit = true;

        // Урон
        if (collision.collider.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(damage);

        // VFX
        if (impactVFX != null)
        {
            ContactPoint contact = collision.GetContact(0);
            Instantiate(impactVFX, contact.point, Quaternion.LookRotation(contact.normal));
        }

        Deactivate();
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    // Вызывается пулом при выстреле
    public void Launch(Vector3 vel)
    {
        rb.velocity = vel;
        rb.angularVelocity = Vector3.zero;
    }
}