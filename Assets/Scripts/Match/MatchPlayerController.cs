using System;
using System.Collections;
using Armor;
using Cards.Data;
using Match;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Scripts.Game.Matches
{
    public class MatchPlayerController : MonoBehaviour
    {
        private float _healthBarTransitionDuration = 0.25f;
        
        [Title("Player")]
        [SerializeField] private int _startingHealth = 100;
        [SerializeField] private DeckDataSO _deckData;
        [SerializeField] private string _playerName;
        
        public MatchPlayer MatchPlayer { get; private set; }
        
        private Slider _healthBar;
        private TextMeshProUGUI _healthText;
        private ArmorView _armorView;
        
        public event Action OnPlayerDeath;
        
        
        private void OnEnable()
        {
            MatchPlayer = new MatchPlayer(_deckData, _playerName, _startingHealth);
        }

        public void Setup(Slider healthBar, TextMeshProUGUI healthText, ArmorView armorView)
        {
            _healthBar = healthBar;
            _healthText = healthText;
            _armorView = armorView;
            
            _healthBar.maxValue = MatchPlayer.Health;
            _healthBar.value = MatchPlayer.Health;
            _healthText.text = $"{MatchPlayer.Health.ToString()}/{MatchPlayer.Health.ToString()}";
        }

        public void DealDamage(int amount)
        {
            if (MatchPlayer.Armor > 0)
            {
                int damageToArmor = Mathf.Min(amount, MatchPlayer.Armor);
                MatchPlayer.Armor -= damageToArmor;
                amount -= damageToArmor;
            }

            if (amount > 0) MatchPlayer.Health -= amount;

            _armorView.UpdateArmorText(MatchPlayer.Armor);
            
            StartCoroutine(UpdateHealthBarSmoothly(MatchPlayer.Health));

            if (MatchPlayer.Health <= 0) Die();
        }
        
        public void GainArmor(int amount)
        {
            MatchPlayer.Armor += amount;
            _armorView.UpdateArmorText(MatchPlayer.Armor);
        }
        
        public void LoseAllArmor()
        {
            MatchPlayer.Armor = 0;
            _armorView.UpdateArmorText(MatchPlayer.Armor);
        }
        
    
        private void Die()
        {
            Debug.Log("Player defeated!");
            OnPlayerDeath?.Invoke();
        
            Destroy(GetComponent<Collider>());

            var rb = gameObject.AddComponent<Rigidbody>();

            Destroy(gameObject, 5f);
        }
    
    
        private IEnumerator UpdateHealthBarSmoothly(int targetHealth)
        {
            float startValue = _healthBar.value;
            float endValue = Mathf.Clamp(targetHealth, 0, MatchPlayer.Health);
            float elapsedTime = 0f;

            while (elapsedTime < _healthBarTransitionDuration)
            {
                _healthBar.value = Mathf.Lerp(startValue, endValue, elapsedTime / _healthBarTransitionDuration);
                _healthText.text = $"{(int)_healthBar.value}/{MatchPlayer.MaxHealth}";
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _healthBar.value = endValue;
            _healthText.text = $"{(int)_healthBar.value}/{MatchPlayer.MaxHealth}";
        }
    }
}
