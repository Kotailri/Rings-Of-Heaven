using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private int health;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = health;        
    }

    public void SetHealth(int _health)
    {
        health = _health;
        currentHealth = health;
    }

    public int GetMaxHealth()
    {
        return health;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void AddHealth(int _hp)
    {
        currentHealth += _hp;
        BalanceHealth();
    }

    public void RemoveHealth(int _hp)
    {
        if (_hp <= 0)
        {
            return;
        }

        currentHealth -= _hp;
        
        BalanceHealth();
        CheckDeath();
    }

    private void BalanceHealth()
    {
        if (currentHealth > health) 
        {
            currentHealth = health;
        }

        if (currentHealth <= 0) 
        {
            currentHealth = 0;
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    private void CheckDeath()
    {
        if (currentHealth == 0) 
        {
            if (TryGetComponent(out IDeathEffect death))
            {
                death.DoDeathEffect();
            }
            transform.position = Config.poolPosition;
            currentHealth = health;
        }
    }
}
