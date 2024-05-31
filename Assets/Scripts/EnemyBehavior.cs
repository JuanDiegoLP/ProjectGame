using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyBehavior : MonoBehaviour
{
    public  CharacterStats enemyStats;

    public float speed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public int maxHealth = 100;
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    public LayerMask playerLayer;
    public Transform player1;

    private int currentHealth;
    private Transform currentPatrolPoint;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isDead = false;
    private Collider2D enemyCollider;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        currentPatrolPoint = patrolPoint1;
        rb.isKinematic = true;
    }

    void Update()
    {
        if (isDead) return;

        DetectAndChasePlayer();

        if (!isAttacking && player1 == null)
        {
            Patrol();
        }
    }

    void DetectAndChasePlayer()
    {
        if (player1 == null)
        {
            DetectPlayer();
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player1.position);
            if (distanceToPlayer > detectionRange)
            {
                player1 = null;
                animator.SetBool("walk", false);
            }
            else if (distanceToPlayer <= attackRange && !isAttacking)
            {
                AttackPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }
    }

    void DetectPlayer()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, detectionRange, playerLayer);
        if (hitPlayers.Length > 0)
        {
            player1 = hitPlayers[0].transform;
            Debug.Log("Player detected!");
            animator.SetBool("walk", true);
            IgnorePlayerCollision();
        }
    }

    void Patrol()
    {
        animator.SetBool("walk", true);
        rb.MovePosition(Vector2.MoveTowards(transform.position, currentPatrolPoint.position, speed * Time.deltaTime));

        if (Vector2.Distance(transform.position, currentPatrolPoint.position) < 0.2f)
        {
            currentPatrolPoint = currentPatrolPoint == patrolPoint1 ? patrolPoint2 : patrolPoint1;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

    void ChasePlayer()
    {
        animator.SetBool("walk", true);
        rb.MovePosition(Vector2.MoveTowards(transform.position, player1.position, speed * Time.deltaTime));

        if (transform.position.x < player1.position.x)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    void AttackPlayer()
    {
        if (player1 != null && player1.CompareTag("Player"))
        {
            PlayerController player = player1.GetComponent<PlayerController>();
            if (player != null)
            {
                isAttacking = true;
                animator.SetTrigger("Attack");
                player.TakeDamage(20);
                Invoke("ResetAttack", 1f); // Cooldown for attack
            }
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Death");
        isDead = true;
        Destroy(gameObject, 4f);
    }

    private void IgnorePlayerCollision()
    {
        if (player1 != null)
        {
            Collider2D playerCollider = player1.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, playerCollider);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
