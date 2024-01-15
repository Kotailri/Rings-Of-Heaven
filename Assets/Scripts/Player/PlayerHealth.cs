using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] 
    private int maxHealth;    // Maximum hit points
    [SerializeField]
    private int currentHealth;    // Current hit points

    [Header("UI Images")]
    public GameObject HealthImage;    // Array to hold the UI Image elements representing hit points
    public GameObject NonHealthImage; // Array to hold the UI Image elements representing hit points

    [Space(5.0f)]
    public RectTransform HealthImageParent;

    private void Awake()
    {
        EventManager.StartListening(EventStrings.PLAYER_DAMAGED, OnDamageTaken);
        EventManager.StartListening(EventStrings.PLAYER_HEALED, OnPlayerHealed);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventStrings.PLAYER_DAMAGED, OnDamageTaken);
        EventManager.StopListening(EventStrings.PLAYER_HEALED, OnPlayerHealed);
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    private void OnDamageTaken(Dictionary<string, object> payload)
    {
        TakeDamage((int)payload["amount"]);
    }

    private void OnPlayerHealed(Dictionary<string, object> payload)
    {
        Heal((int)payload["amount"]);
    }

    private void AddMaxhealth(int _maxHealth)
    {
        maxHealth += _maxHealth;
        currentHealth += _maxHealth;
        UpdateHealthUI();
    }

    private void TakeDamage(int damageAmount)
    {
        AudioManager.instance.PlaySound("damaged");
        Global.vignetteTween.SetVignetteDamage();

        currentHealth -= damageAmount;
        UpdateHealthUI();

        // Check if the player is dead
        if (currentHealth <= 0) { }
    }

    private void Heal(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    private void HealToMax()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // check bounds
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        float spacing = 3f;
        float startingX = HealthImageParent.GetComponent<RectTransform>().position.x;
        float startingY = HealthImageParent.GetComponent<RectTransform>().position.y;

        for (int i = 0; i < maxHealth; i++)
        {
            if (i < currentHealth)
            {
                GameObject obj = Instantiate(HealthImage);
                obj.GetComponent<RectTransform>().SetParent(HealthImageParent, false);
                obj.GetComponent<RectTransform>().position = new Vector2(startingX + i * spacing, startingY);
            }
            else
            {
                GameObject obj = Instantiate(NonHealthImage);
                obj.GetComponent<RectTransform>().SetParent(HealthImageParent, false);
                obj.GetComponent<RectTransform>().position = new Vector2(startingX + i * spacing, startingY);
            }
        }
    }
}
