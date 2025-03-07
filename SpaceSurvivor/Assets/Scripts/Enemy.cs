using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float detectionRange = 15f;
    public float stoppingDistance = 10f;
    public float maxHealth = 100f;
    public Image healthBar;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;
    public float shootingCooldown = 1f;
    public float bulletLifetime = 3f;
    public AudioSource shootSound;

    private NavMeshAgent agent;
    private Animator animator;
    private bool movingToPointA = true;
    private Transform playerTransform;
    private bool isChasingPlayer = false;
    private float currentHealth;
    private float lastShotTime = 0f;
    private bool isDead = false;
    private Player playerScript;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;
        currentHealth = maxHealth;
        MoveToNextPoint();

        playerScript = Object.FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        if (isDead) return;

        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);

        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isChasingPlayer)
        {
            movingToPointA = !movingToPointA;
            MoveToNextPoint();
        }

        if (isChasingPlayer && playerTransform != null)
        {
            ChasePlayer();
        }

        ShootAtPlayer();
    }

    private void MoveToNextPoint()
    {
        if (!isChasingPlayer)
        {
            agent.SetDestination(movingToPointA ? pointA.position : pointB.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            isChasingPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasingPlayer = false;
            playerTransform = null;
        }
    }

    private void ChasePlayer()
    {
        if (playerScript != null && !playerScript.IsAlive) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        if (distanceToPlayer > 3f)
        {
            animator.SetTrigger("IsShooting");
            agent.SetDestination(playerTransform.position);
            agent.isStopped = false;
        }
        else
        {
            agent.isStopped = true;
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void ShootAtPlayer()
    {
        if (playerTransform != null && playerScript != null && playerScript.IsAlive && Time.time - lastShotTime >= shootingCooldown)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.useGravity = false;

                Vector3 playerPosition = new Vector3(playerTransform.position.x, bulletSpawnPoint.position.y, playerTransform.position.z);
                Vector3 bulletDirection = (playerPosition - bulletSpawnPoint.position).normalized;

                rb.velocity = bulletDirection * bulletSpeed;
                Destroy(bullet, bulletLifetime);

                if (shootSound != null)
                {
                    shootSound.Play();
                }

                lastShotTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("IsDead");
        agent.isStopped = true;

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (playerScript != null)
        {
            playerScript.GainHealth(5f);
        }

        float delayTime = 2.0f;
        Destroy(gameObject, delayTime);
    }
}
