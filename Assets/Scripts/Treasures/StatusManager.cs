using Project.Runtime.Scripts.General;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance { get; private set; }

    public int DamageBonus { get; private set; }
    public int ArmorBonus { get; private set; }
    
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Garante que só existe um
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantém entre cenas
        }
        
        MaxHealth = 50; 
        CurrentHealth = MaxHealth;
    }

    public void AddDamageBonus(int amount = 1)
    {
        DamageBonus += amount;
        Debug.Log("Damage Bonus: " + DamageBonus);
    }

    public void AddArmorBonus(int amount = 1)
    {
        ArmorBonus += amount;
        Debug.Log("Armor Bonus: " + ArmorBonus);
    }
    
    public void SetHealth(int amount)
    {
        CurrentHealth = amount;
        Debug.Log("Health: " + CurrentHealth);
    }
    
    // Regen % of health, dont allow to pass max health
    public void RegenHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
        Debug.Log("Regen Health: " + CurrentHealth);
    }
    
    public void ResetStatus()
    {
        DamageBonus = 0;
        ArmorBonus = 0;
        MaxHealth = 100;
        CurrentHealth = MaxHealth;
        Debug.Log("Reset Status");
    }
}
