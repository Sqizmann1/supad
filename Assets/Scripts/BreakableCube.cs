using UnityEngine;

public class BreakableCube : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Break")]
    public GameObject brokenVersionPrefab;  // Prefab из осколков
    public float breakForce = 300f;         // Сила взрыва осколков
    public float breakForceRadius = 2f;

    [Header("Visual Feedback")]
    public Material damagedMaterial;        // Материал при повреждении (опционально)
    private Material originalMaterial;
    private Renderer cubeRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer) originalMaterial = cubeRenderer.material;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Визуальный отклик — смена материала при низком HP
        if (damagedMaterial && cubeRenderer)
        {
            float healthPercent = currentHealth / maxHealth;
            if (healthPercent < 0.5f)
                cubeRenderer.material = damagedMaterial;
        }

        if (currentHealth <= 0f)
            Break();
    }

    void Break()
    {
        // Спавним разбитую версию
        if (brokenVersionPrefab != null)
        {
            GameObject broken = Instantiate(brokenVersionPrefab, transform.position, transform.rotation);

            // Разлетаем все осколки от центра куба
            foreach (Rigidbody chunk in broken.GetComponentsInChildren<Rigidbody>())
            {
                chunk.AddExplosionForce(breakForce, transform.position, breakForceRadius);
            }

            // Убираем осколки через 4 секунды
            Destroy(broken, 4f);
        }

        Destroy(gameObject);
    }
}