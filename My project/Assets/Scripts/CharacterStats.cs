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


    public int maxMana = 10;
    public int currentMana;
    public Slider manaBar;
    public TextMeshProUGUI manaText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = MaxHealth;
        currentMana = maxMana;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
       
    }

    public void UseMana(int emount)
    {
        currentMana -= emount;
        if (currentMana < 0)
        {
            currentMana = 0;
            UpdateUI();
        }
        
            
    }

    public void GainMana(int amount)
    {
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        UpdateUI();

    }

    private void UpdateUI()
    {
        if(healthBar !=null)
        {
            healthBar.value = (float)currentHealth / MaxHealth;
        }
        if (healthBar != null)
        {
           healthText.text = $"{currentHealth}/{MaxHealth}";

        }

        if(manaBar != null)
        {
            manaBar.value = (float)currentMana / maxMana;
        }

        if (manaBar != null)
        {
            manaText.text = $"{currentMana}/{maxMana}";
        }

    }

}
