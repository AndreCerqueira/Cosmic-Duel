using System;
using System.Collections;
using Armor;
using Characters.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Attack
{
    public int Damage;
    public int Armor;
}


public class EnemyView : MonoBehaviour
{
    [SerializeField] private EnemyDataSO _enemyData;
    
    [SerializeField] private SpriteRenderer _artwork;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _attack;
    [SerializeField] private ArmorView _armorView;
    [SerializeField] private Animator _animator;
    
    public Attack NextAttack { get; private set; }
    
    private float _healthBarTransitionDuration = 0.25f;
    
    public int Health { get; private set; }
    public int Armor { get; private set; }
    
    public EnemyDataSO EnemyData => _enemyData;
    
    public event Action OnEnemyDeath; 
    
    private void Start()
    {
        _artwork.sprite = _enemyData.Artwork;
        _healthBar.maxValue = _enemyData.Health;
        _healthBar.value = _enemyData.Health;
        _healthText.text = $"{_enemyData.Health.ToString()}/{_enemyData.Health.ToString()}";
        Health = _enemyData.Health;
        Armor = 0;
        _armorView.UpdateArmorText(Armor);
        
        _animator.runtimeAnimatorController = _enemyData.EnemyAnimator;

        GenerateNextAttack();
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
        
        if (Health <= 0) Die();
    }
    
    
    public void GainArmor(int amount)
    {
        Armor += amount;
        _armorView.UpdateArmorText(Armor);
    }
    
    
    public void LoseAllArmor()
    {
        Armor = 0;
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
    
    
    public void GenerateNextAttack()
    {
        float chance = Random.value;
        bool isImpredictable = chance < 0.3f;
        
        NextAttack = GenerateAttack();

        if (isImpredictable) {
            _attack.text = "Next turn: ?";
            return;
        }
        
        _attack.text = "Next turn: ";
    
        if (NextAttack.Damage > 0)
        {
            _attack.text += $"{NextAttack.Damage} <sprite name=sword>";
        }
    
        if (NextAttack.Armor > 0)
        {
            _attack.text += $"{NextAttack.Armor} <sprite name=shield>";
        }
    }
    
    
    private Attack GenerateAttack()
    {
        // choose one randomly 75% to damage, 25% to armor
        int randomValue = Random.Range(0, 100);
        
        int damage = 0;
        int armor = 0;
        
        if (randomValue < 80)
        {
            damage = Random.Range(1, 10);
        }
        else
        {
            armor = Random.Range(1, 10);
        }
        
        // Create a new Attack object with the generated values
        Attack attack = new Attack
        {
            Damage = damage,
            Armor = armor
        };

        return attack;
    }
    
    private void Die()
    {
        Debug.Log("Enemy defeated!");
        OnEnemyDeath?.Invoke();
        
        Destroy(GetComponent<Collider>());

        var rb = gameObject.AddComponent<Rigidbody>();

        Destroy(gameObject, 5f);
    }
}
