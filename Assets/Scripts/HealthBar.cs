using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public PlayerController playerController;

    void Start()
    {
        if (playerController != null)
        {
            playerController.HealthChanged += UpdateHealthBar;
        }
    }

    void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    private void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.HealthChanged -= UpdateHealthBar;
        }
    }
}
