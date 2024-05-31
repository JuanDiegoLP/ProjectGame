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

    //Movement
    [Header("Movement")]
    private Rigidbody2D rb2D;

    private float movement = 0f;

    [SerializeField] private float movementSpeed;
    [Range(0, 0.3f)] [SerializeField] private float movementSmoothing;

    private Vector3 speed = Vector3.zero;
    private bool rightDirection = true;

    //Jump
    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask isFloor;
    [SerializeField] private Transform floorController;
    [SerializeField] private Vector3 boxDimensions;
    [SerializeField] private bool onTheFloor;

    private bool jump = false;

    //Animator
    [Header("Animator")]
    private Animator animator;

    public LayerMask enemyLayer;
    [SerializeField]public float attackRange;
    [SerializeField] private Transform hitController;
    public int maxHealth = 100;

    private int currentHealth;

    public GameOverManager gameOverManager;
    public float gameOverDelay = 0.25f;
    public AudioClip deathSound;
    private AudioSource audioSource;

    //Start
    private void Start()
    {
        currentHealth = maxHealth;
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        HealthChanged?.Invoke(currentHealth, maxHealth);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        movement = Input.GetAxisRaw("Horizontal") * movementSpeed;

        animator.SetFloat("movement", Math.Abs(movement));

        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        onTheFloor = Physics2D.OverlapBox(floorController.position, boxDimensions, 0f, isFloor);
        animator.SetBool("on the floor", onTheFloor);
        Move(movement * Time.fixedDeltaTime, jump);

        jump = false;
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.transform.GetComponent<EnemyBehavior>().TakeDamage(20);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
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

    private void Move(float move, bool jump)
    {
        Vector3 targetSpeed = new Vector2(move, rb2D.velocity.y);
        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, targetSpeed, ref speed, movementSmoothing);

        if (move>0 && !rightDirection)
        {
            TurnLeftOrRight();
        }
        else if (move<0 && rightDirection)
        {
            TurnLeftOrRight();
        }

        if (onTheFloor && jump)
        {
            onTheFloor = false;
            rb2D.AddForce(new Vector2(0f, jumpForce));
        }
    }

    private void TurnLeftOrRight()
    {
        rightDirection = !rightDirection;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(floorController.position, boxDimensions);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitController.position, attackRange);
    }
}
