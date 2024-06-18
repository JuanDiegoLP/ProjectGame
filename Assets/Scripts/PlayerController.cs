using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public event OnHealthChanged HealthChanged;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [Range(0, 0.3f)][SerializeField] private float movementSmoothing;
    private Rigidbody2D rb2D;
    private float movement = 0f;
    private Vector3 velocity = Vector3.zero;
    private bool facingRight = true;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 groundCheckSize;
    private bool isGrounded;
    private bool jump = false;

    [Header("Combat Settings")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPoint;
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Game Over Settings")]
    [SerializeField] private float fallThreshold = -10f;
    public GameOverManager gameOverManager;
    public float gameOverDelay = 0.25f;
    public AudioClip deathSound;
    private AudioSource audioSource;

    private Animator animator;

    private void Start()
    {
        InitializeComponents();
        currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void InitializeComponents()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
        HandleAttackInput();
    }

    private void HandleMovementInput()
    {
        movement = Input.GetAxisRaw("Horizontal") * movementSpeed;
        animator.SetFloat("movement", Math.Abs(movement));
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();
        Move(movement * Time.fixedDeltaTime, jump);
        jump = false;
        CheckFallStatus();
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void CheckFallStatus()
    {
        if (transform.position.y < fallThreshold)
        {
            Die();
        }
    }

    private void Move(float move, bool jump)
    {
        Vector3 targetVelocity = new Vector2(move, rb2D.velocity.y);
        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, targetVelocity, ref velocity, movementSmoothing);

        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }

        if (isGrounded && jump)
        {
            isGrounded = false;
            rb2D.AddForce(new Vector2(0f, jumpForce));
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyBehavior>().TakeDamage(20);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        animator.SetTrigger("Death");
        yield return new WaitForSeconds(gameOverDelay);

        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
