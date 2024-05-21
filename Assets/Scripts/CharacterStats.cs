using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Attack(GameObject target, int damage)
    {
        CharacterStats targetStats = target.GetComponent<CharacterStats>();

        if (targetStats != null)
        {
            targetStats.TakeDamage(damage);
        }

    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
