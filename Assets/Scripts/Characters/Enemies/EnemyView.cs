using System;
using System.Collections;
using Armor;
using Characters.Enemies;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Attack
{
    public int Damage;
    public int Armor;
    public bool IsImpredictable;
}


public class EnemyView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _artwork;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _attack;
    [SerializeField] private ArmorView _armorView;
    [SerializeField] private Animator _animator;
    [SerializeField] public Transform _canvasTransform;
    
    public Attack NextAttack { get; private set; }
    
    private float _healthBarTransitionDuration = 0.25f;
    
    public int Health { get; private set; }
    public int Armor { get; private set; }
    
    public bool IsDead => Health <= 0;
    
    public EnemyDataSO EnemyData;
    
    public event Action OnEnemyDeath; 
    
    public void Setup(EnemyDataSO enemyDataSo)
    {
        EnemyData = enemyDataSo;
        
        _healthBar.maxValue = EnemyData.Health;
        _healthBar.value = EnemyData.Health;
        _healthText.text = $"{EnemyData.Health.ToString()}/{EnemyData.Health.ToString()}";
        Health = EnemyData.Health;
        Armor = 0;
        _armorView.UpdateArmorText(Armor);

        _canvasTransform.position = new Vector3(_canvasTransform.position.x, EnemyData.Altura, _canvasTransform.position.z);
        
        if (EnemyData.needFlip) 
            _artwork.flipX = true;
        
        _animator.runtimeAnimatorController = EnemyData.EnemyAnimator;
        
        _artwork.color = AlienColorPalette.Instance.GetRandomColor();

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
        float endValue = Mathf.Clamp(targetHealth, 0, EnemyData.Health);
        float elapsedTime = 0f;

        while (elapsedTime < _healthBarTransitionDuration)
        {
            _healthBar.value = Mathf.Lerp(startValue, endValue, elapsedTime / _healthBarTransitionDuration);
            _healthText.text = $"{(int)_healthBar.value}/{EnemyData.Health}";
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garantir que o valor final seja exatamente o alvo
        _healthBar.value = endValue;
        _healthText.text = $"{(int)_healthBar.value}/{EnemyData.Health}";
    }
    
    
    public void RevealHiddenAttack()
    {
        if (NextAttack.IsImpredictable)
        {
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
    }
    
    
    public void GenerateNextAttack()
    {
        float chance = Random.value;
        bool isImpredictable = chance < 0.3f;
        
        NextAttack = GenerateAttack(isImpredictable);

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
    
    
    private Attack GenerateAttack(bool isImpredictable)
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
            Armor = armor,
            IsImpredictable = isImpredictable
        };

        return attack;
    }
    
    private void Die()
    {
        Debug.Log("Enemy defeated!");
        OnEnemyDeath?.Invoke();
        
        // PopOut with DOTween
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            //Destroy(gameObject);
        });
    }
}
