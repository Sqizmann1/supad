using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour, IDamageable
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Distances")]
    public float walkDistance = 15f;
    public float runDistance = 7f;
    public float attackDistance = 2f;

    [Header("Attack")]
    public float damage = 15f;
    public float attackCooldown = 1.5f;

    private float nextAttackTime;
    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > walkDistance)
        {
            agent.isStopped = true;

            animator.SetFloat("Speed", 0f);
            animator.SetBool("Attack", false);
        }
        else if (distance > runDistance)
        {
            agent.isStopped = false;
            agent.speed = 2f;
            agent.SetDestination(player.position);

            animator.SetFloat("Speed", 0.5f);
            animator.SetBool("Attack", false);
        }
        else if (distance > attackDistance)
        {
            agent.isStopped = false;
            agent.speed = 4f;
            agent.SetDestination(player.position);

            animator.SetFloat("Speed", 1f);
            animator.SetBool("Attack", false);
        }
        else
        {
            agent.isStopped = true;

            animator.Play("PedrosoAttack");

            // animator.SetFloat("Speed", 0f);
            // animator.SetBool("Attack", true);

            transform.LookAt(new Vector3(
                player.position.x,
                transform.position.y,
                player.position.z));

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;

                IDamageable target =
                    player.GetComponent<IDamageable>();

                if (target != null)
                    target.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        agent.isStopped = true;
        agent.enabled = false;

        if (Random.Range(0, 2) == 1)
        {
            animator.Play("PedrosoDeath");
        }
        else { animator.Play("PedrosoDeath2"); }
        // animator.SetBool("Dead", true);

        Destroy(gameObject, 5f);
    }
}