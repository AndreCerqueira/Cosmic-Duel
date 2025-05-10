using System;
using Armor;
using DG.Tweening;
using Project.Runtime.Scripts.Game.Cards.View;
using Project.Runtime.Scripts.Game.Matches;
using Project.Runtime.Scripts.General;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match
{
    public class MatchController : Singleton<MatchController>
    {
        private Project.Runtime.Scripts.Game.Matches.Match _match;
        
        [Title("Prefabs")]
        [SerializeField] private Transform _playerPrefabContainer;
        [SerializeField] private MatchPlayerController _playerPrefab;
        [SerializeField] private EnemyView _enemyBotPrefab;
        
        [Title("Views")]
        [SerializeField] private Button _turnButton;
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
            
            _match = new Project.Runtime.Scripts.Game.Matches.Match(player.MatchPlayer, enemy);

            StartMatch();
        }

        private void StartMatch()
        {
            _handView.Setup(_match.SelfPlayer.Hand);
            
            var targetScale = _turnButton.transform.localScale;
            _turnButton.transform.localScale = Vector3.zero;
            
            MovePlayerToStartPosition(SelfPlayerController.transform, () =>
            {
                SelfPlayer.DrawStartingHand();
                SelfPlayer.GainEnergy(3);
            
                PopInTurnButton(targetScale);
            });
            
            MoveEnemyToStartPosition(Enemy.transform);
        }
        

        private void MovePlayerToStartPosition(Transform playerTransform, Action onComplete = null)
        {
            playerTransform.DOMoveX(playerTransform.position.x + 10f, 2f).SetEase(Ease.OutQuad).OnComplete(() => onComplete?.Invoke());
        }

        private void MoveEnemyToStartPosition(Transform enemyTransform)
        {
            enemyTransform.DOMoveX(enemyTransform.position.x - 10f, 2f).SetEase(Ease.OutQuad);
        }
        
        private void PopInTurnButton(Vector3 targetScale)
        {
            _turnButton.transform.DOScale(targetScale, 1f).SetEase(Ease.OutBack);
        }
    }
}
