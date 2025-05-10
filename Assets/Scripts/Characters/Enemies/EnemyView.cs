using Armor;
using Characters.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyView : MonoBehaviour
{
    [SerializeField] private EnemyDataSO _enemyData;
    
    [SerializeField] private SpriteRenderer _artwork;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _attack;
    [SerializeField] private ArmorView _armorView;
    
    public int Health { get; private set; }
    public int Armor { get; private set; }
    
    public EnemyDataSO EnemyData => _enemyData;
    
    private void Start()
    {
        _artwork.sprite = _enemyData.Artwork;
        _healthBar.maxValue = _enemyData.Health;
        _healthBar.value = _enemyData.Health;
        _healthText.text = $"{_enemyData.Health.ToString()}/{_enemyData.Health.ToString()}";
        _attack.text = $"Next turn: {_enemyData.BaseAttack.ToString()}";
        Health = _enemyData.Health;
        Armor = 0;
        _armorView.UpdateArmorText(Armor);
    }
    

    public void DealDamage(int amount)
    {
        if (Armor > 0)
        {
            int damageToArmor = Mathf.Min(amount, Armor);
            Armor -= damageToArmor;
            amount -= damageToArmor;
        }

        if (amount > 0) Health -= amount;
        
        _armorView.UpdateArmorText(Armor);
        _healthBar.value = Health;
        _healthText.text = $"{Health}/{_enemyData.Health}";
        
        if (Health <= 0) Debug.Log("Enemy defeated!");
    }
    
    
    public void GainArmor(int amount)
    {
        Armor += amount;
        _armorView.UpdateArmorText(Armor);
    }
}
