using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public string characterName;
    public int MaxHealth = 100;
    public int currentHealth;

    public Slider healthBar;
    public TextMeshProUGUI healthText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
       
    }

}
