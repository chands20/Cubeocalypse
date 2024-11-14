using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    public int health = 20;
    public Renderer zombieRenderer; // Reference to the Renderer component (assign in inspector)
    public Color damageColor = Color.red; // Color to flash when damaged
    private Color originalColor; // Store original color
    public GameObject coinPrefab;

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;

    public delegate void ZombieDeathHandler();
    public event ZombieDeathHandler OnZombieDeath;

    void Start()
    {
        //Get animator
        animator = gameObject.GetComponent<Animator>();
        // Get the player target by tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }

        // Get the zombie's renderer if not already assigned
        if (zombieRenderer == null)
        {
            zombieRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        // Store the original color of the material
        if (zombieRenderer != null)
        {
            originalColor = zombieRenderer.material.color;
        }

        // Set up the NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = false; // Disable NavMeshAgent's auto-rotation
    }

    private void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);

            // Rotate to face the target while locking rotation on the X-axis to -90 and Z-axis to 0
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0; // Flatten direction to keep the zombie upright
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                if(health <= 0)
                {
                    animator.SetBool("isWalking", false);
                }
                else
                {
                    animator.SetBool("isWalking", true);
                }
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        // Change to damage color
        if (zombieRenderer != null)
        {
            zombieRenderer.material.color = damageColor;
        }

        // Wait for a split second
        yield return new WaitForSeconds(.1f);

        // Revert to the original color
        if (zombieRenderer != null)
        {
            zombieRenderer.material.color = originalColor;
        }
    }

    void Die()
    {
        // Stop the NavMeshAgent from moving
        if (agent != null)
        {
            agent.isStopped = true;
        }

        // Trigger the death animation
        animator.SetBool("isDead", true);

        OnZombieDeath?.Invoke();

        // Drop Coins
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        // Start a coroutine to wait for the animation to complete before destroying
        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {

        // Wait for the length of the death animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Destroy the zombie after the animation ends
        Destroy(gameObject);
    }
}
