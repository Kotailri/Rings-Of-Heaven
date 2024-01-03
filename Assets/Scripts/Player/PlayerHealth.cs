using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3; // Maximum hit points
    public int currentHealth; // Current hit points
    public Image[] healthImages; // Array to hold the UI Image elements representing hit points

    private void Start()
    {
        SetMaxHealth(maxHealth);
    }

    // Set the player's maximum health
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Add to the player's maximum health
    public void AddMaxhealth(int newMaxHealthAdd)
    {
        maxHealth += newMaxHealthAdd;
        currentHealth += newMaxHealthAdd;
        UpdateHealthUI();
    }

    // Function to reduce health (called when the player is hit)
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (damageAmount > 0)
        {
            AudioManager.instance.PlaySound("damaged");
        }

        Global.vignetteTween.SetVignetteDamage();

        // Ensure health doesn't go below zero
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // Check if the player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    // Function to heal the player to maximum health
    public void HealToMaxHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Update the UI to represent the current health
    protected void UpdateHealthUI()
    {
        for (int i = 0; i < healthImages.Length; i++)
        {
            // Activate or deactivate UI images based on current health
            healthImages[i].gameObject.SetActive(i < currentHealth);
        }
    }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
