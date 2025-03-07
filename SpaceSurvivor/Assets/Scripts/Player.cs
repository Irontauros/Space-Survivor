using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
    public float speed = 2f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill;
    public Camera mainCamera;
    public Camera overheadCamera;
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;
    public AudioSource shootSound;

    public MonoBehaviour cameraControlScript; // Script de controle da câmera

    private Vector3 moveDirection;
    private Vector3 velocity;
    private CharacterController controller;
    private Animator animator;
    private bool isGrounded;
    private bool isGameOver = false;

    public bool IsAlive
    {
        get { return currentHealth > 0; }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (mainCamera != null) mainCamera.enabled = true;
        if (overheadCamera != null) overheadCamera.enabled = false;

    }

    private void Update()
    {
        if (isGameOver) return;
        if (PauseManager.isPaused) return;

        HandleMovement();
        HandleGravity();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void HandleMovement()
    {
        if (isGameOver) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude > 0)
        {
            animator.SetBool("IsWalkingForward", vertical > 0);
            animator.SetBool("IsWalkingBackward", vertical < 0);
            animator.SetBool("IsWalkingRight", horizontal > 0);
            animator.SetBool("IsWalkingLeft", horizontal < 0);

            Vector3 movement = moveDirection * speed * Time.deltaTime;
            controller.Move(transform.TransformDirection(movement));
        }
        else
        {
            animator.SetBool("IsWalkingForward", false);
            animator.SetBool("IsWalkingBackward", false);
            animator.SetBool("IsWalkingRight", false);
            animator.SetBool("IsWalkingLeft", false);
        }
    }

    private void HandleGravity()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Shoot()
    {
        if (bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 bulletDirection = bulletSpawnPoint.forward;
                rb.AddForce(bulletDirection * bulletSpeed, ForceMode.VelocityChange);
            }
            Destroy(bullet, 2f);
            if (shootSound != null) shootSound.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            animator.SetTrigger("Die");
            EndGame();
        }
    }

    public void GainHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private void DisablePlayerControls()
    {
        this.enabled = false;
        controller.enabled = false;

        if (cameraControlScript != null)
            cameraControlScript.enabled = false; 

    }


    private void EndGame()
    {
        isGameOver = true;
        DisablePlayerControls();

        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetGameOver(true);
        }

        SwitchToOverheadCamera();
        StartCoroutine(WaitForDeathAnimationAndLoadScene());
    }


    private void SwitchToOverheadCamera()
    {
        if (overheadCamera != null && mainCamera != null)
        {
            overheadCamera.enabled = true;
            mainCamera.enabled = false;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    private IEnumerator WaitForDeathAnimationAndLoadScene()
    {
        float deathAnimationDuration = 2f;
        yield return new WaitForSeconds(deathAnimationDuration);

        SceneManager.LoadScene("Menu");
    }
}
