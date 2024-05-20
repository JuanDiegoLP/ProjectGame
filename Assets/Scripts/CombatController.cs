using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public int maxHealth = 3;
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
            Destroy(gameObject);
        }
    }

    public void Attack(GameObject target, int damage)
    {
        CombatController targetHealth = target.GetComponent<CombatController>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
    }
}
