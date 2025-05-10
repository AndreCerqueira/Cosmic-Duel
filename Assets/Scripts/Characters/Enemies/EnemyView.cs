using System.Collections;
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
    
    private float _healthBarTransitionDuration = 0.25f;
    
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
        
        StartCoroutine(UpdateHealthBarSmoothly(Health));
        
        if (Health <= 0) Debug.Log("Enemy defeated!");
    }
    
    
    public void GainArmor(int amount)
    {
        Armor += amount;
        _armorView.UpdateArmorText(Armor);
    }
    
    
    private IEnumerator UpdateHealthBarSmoothly(int targetHealth)
    {
        float startValue = _healthBar.value;
        float endValue = Mathf.Clamp(targetHealth, 0, _enemyData.Health);
        float elapsedTime = 0f;

        while (elapsedTime < _healthBarTransitionDuration)
        {
            _healthBar.value = Mathf.Lerp(startValue, endValue, elapsedTime / _healthBarTransitionDuration);
            _healthText.text = $"{(int)_healthBar.value}/{_enemyData.Health}";
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garantir que o valor final seja exatamente o alvo
        _healthBar.value = endValue;
        _healthText.text = $"{(int)_healthBar.value}/{_enemyData.Health}";
    }
}
