using Project.Runtime.Scripts.General;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance { get; private set; }

    public int DamageBonus { get; private set; }
    public int ArmorBonus { get; private set; }
    
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
}
