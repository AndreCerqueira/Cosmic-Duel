using System;
using System.Text.RegularExpressions;
using Armor;
using Match;
using Project.Runtime.Scripts.Game.Cards.View;
using Project.Runtime.Scripts.General;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Scripts.Game.Matches
{
    public class MatchController : Singleton<MatchController>
    {
        private Match _match;
        
        [Title("Prefabs")]
        [SerializeField] private Transform _playerPrefabContainer;
        [SerializeField] private MatchPlayerController _playerPrefab;
        [SerializeField] private EnemyView _enemyBotPrefab;
        
        [Title("Views")]
        [SerializeField] private HandView _handView;
        [SerializeField] private Slider _playerHealthBar;
        [SerializeField] private TextMeshProUGUI _playerHealthText;
        [SerializeField] private ArmorView _playerArmorView;

        public MatchPlayer SelfPlayer => _match.SelfPlayer;
        [NonSerialized] public MatchPlayerController SelfPlayerController;
        public EnemyView Enemy => _match.Enemy;
        
        private void Start()
        {
            var player = Instantiate(_playerPrefab, _playerPrefabContainer).GetComponent<MatchPlayerController>();
            var enemy = Instantiate(_enemyBotPrefab, _playerPrefabContainer).GetComponent<EnemyView>();
            
            player.Setup(_playerHealthBar, _playerHealthText, _playerArmorView);
            
            SelfPlayerController = player;
            
            player.MatchPlayer.OnEnergyChanged += EnergySystem.Instance.UpdateEnergyText;
            
            _match = new Match(player.MatchPlayer, enemy);

            StartMatch();
        }

        private void StartMatch()
        {
            _handView.Setup(_match.SelfPlayer.Hand);
            
            SelfPlayer.DrawStartingHand();
            SelfPlayer.GainEnergy(3);
        }
    }
}
