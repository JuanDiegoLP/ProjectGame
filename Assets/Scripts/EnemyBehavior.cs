using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public  CharacterStats enemyStats;

    public float speed = 2f;
    public float detectionRange = 5f;
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    public LayerMask playerLayer;
    public float attackRange = 1f;
    public int maxHealth = 100;

    private int currentHealth;
    private Transform currentPatrolPoint;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isDead = false;
    private Transform player;
    private Collider2D enemyCollider;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        currentPatrolPoint = patrolPoint1;
        rb.isKinematic = true;

        IgnorePlayerCollision();
    }

    void Update()
    {
        if (isDead)
            return;

        if (player == null)
        {
            DetectPlayer();
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > detectionRange)
            {
                player = null;
                animator.SetBool("walk", false);
            }
            else if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(AttackPlayer());
            }
            else
            {
                ChasePlayer();
            }
        }

        if (!isAttacking && player == null)
        {
            Patrol();
        }
    }

    void DetectPlayer()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, detectionRange, playerLayer);
        if (hitPlayers.Length > 0)
        {
            player = hitPlayers[0].transform;
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
        rb.MovePosition(Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime));

        if (transform.position.x < player.position.x)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        enemyStats.Attack(player.gameObject, 20);
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        enemyStats.TakeDamage(damage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Death");
        Destroy(gameObject);
    }

    private void IgnorePlayerCollision()
    {
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
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
