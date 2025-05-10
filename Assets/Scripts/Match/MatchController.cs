using System;
using System.Collections.Generic;
using System.Linq;
using Armor;
using Characters.Enemies;
using DG.Tweening;
using MoreMountains.Feedbacks;
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
        
        [Title("Enemies")]
        [SerializeField] private List<EnemyDataSO> _allEnemies;
        [SerializeField] private List<Transform> _enemySpawnPoints;
        
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
        
        [Title("Feedbacks")]
        [SerializeField] private Transform _GameOverPopup;
        [SerializeField] private Transform _VictoryPopup;

        public MatchPlayer SelfPlayer => _match.SelfPlayer;
        [NonSerialized] public MatchPlayerController SelfPlayerController;
        public List<EnemyView> Enemies { get; private set; } = new List<EnemyView>();
        
        private void Start()
        {
            var player = Instantiate(_playerPrefab, _playerPrefabContainer).GetComponent<MatchPlayerController>();
            
            player.Setup(_playerHealthBar, _playerHealthText, _playerArmorView);
            
            SelfPlayerController = player;
            
            player.MatchPlayer.OnEnergyChanged += EnergySystem.Instance.UpdateEnergyText;
            player.OnPlayerDeath += () =>
            {
                _GameOverPopup.gameObject.SetActive(true);
                RemoveHandView();
                Debug.Log("Game Over");
            };
            
            var enemies = SpawnEnemies();
            Enemies.AddRange(enemies);
            _match = new Project.Runtime.Scripts.Game.Matches.Match(player.MatchPlayer, enemies.FirstOrDefault());

            StartMatch(enemies);
        }

        private void StartMatch(List<EnemyView> enemies)
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

            MoveEnemiesToStartPosition(enemies);
        }
        
        
        
        
        
        
        
        private List<EnemyView> SpawnEnemies()
        {
            var selectedEnemies = new List<EnemyView>();
            
            // Decida se o inimigo gerado será um Boss ou Mobs comuns
            bool spawnBoss = UnityEngine.Random.Range(0, 2) == 0; // 50% de chance de spawnar um Boss

            if (spawnBoss)
            {
                // Se for um Boss, escolhe um único inimigo Boss e dobra o seu tamanho
                var bossData = _allEnemies.FirstOrDefault(e => e.IsBoss);
                if (bossData != null)
                {
                    var spawnPoint = _enemySpawnPoints[0]; // O Boss é o único, então ele ocupa a primeira posição
                    var boss = Instantiate(_enemyBotPrefab, _playerPrefabContainer).GetComponent<EnemyView>();
                    boss.Setup(bossData);
                    boss.transform.localScale *= 2; // Dobra o tamanho do Boss
                    boss._canvasTransform.localScale /= 2; 

                    selectedEnemies.Add(boss);
                }
                else
                {
                    // Caso não haja um Boss disponível, forçamos a criação de pelo menos um inimigo comum
                    var commonEnemyData = _allEnemies.FirstOrDefault(e => !e.IsBoss); // Busca um inimigo comum
                    if (commonEnemyData != null)
                    {
                        var spawnPoint = _enemySpawnPoints[0];
                        var commonEnemy = Instantiate(_enemyBotPrefab, _playerPrefabContainer).GetComponent<EnemyView>();
                        commonEnemy.Setup(commonEnemyData);
                        selectedEnemies.Add(commonEnemy);
                    }
                }
            }
            else
            {
                // Caso contrário, spawnar de 1 a 3 mobs comuns
                int enemyCount = UnityEngine.Random.Range(1, 4); // 1 to 3
                for (int i = 0; i < enemyCount; i++)
                {
                    var enemyData = _allEnemies[UnityEngine.Random.Range(0, _allEnemies.Count)];
                    
                    // Verifica se o inimigo não é um Boss
                    if (enemyData.IsBoss) continue; // Não deve spawnar um Boss aqui

                    var spawnPoint = _enemySpawnPoints[i];
                    var enemy = Instantiate(_enemyBotPrefab, _playerPrefabContainer).GetComponent<EnemyView>();
                    enemy.Setup(enemyData);

                    enemy.OnEnemyDeath += () =>
                    {
                        if (selectedEnemies.All(e => e.IsDead))
                        {
                            _VictoryPopup.gameObject.SetActive(true);
                            RemoveHandView();
                            Debug.Log("You Win");
                        }
                    };

                    selectedEnemies.Add(enemy);
                }
            }

            // Caso a lista de inimigos ainda esteja vazia, força a criação de pelo menos um inimigo comum
            if (selectedEnemies.Count == 0)
            {
                var commonEnemyData = _allEnemies.FirstOrDefault(e => !e.IsBoss);
                if (commonEnemyData != null)
                {
                    var spawnPoint = _enemySpawnPoints[0];
                    var commonEnemy = Instantiate(_enemyBotPrefab, _playerPrefabContainer).GetComponent<EnemyView>();
                    commonEnemy.Setup(commonEnemyData);
                    selectedEnemies.Add(commonEnemy);
                }
            }

            return selectedEnemies;
        }
        
        
        
        
        // -----------

        private void MovePlayerToStartPosition(Transform playerTransform, Action onComplete = null)
        {
            playerTransform.DOMoveX(playerTransform.position.x + 10f, 2f).SetEase(Ease.OutQuad).OnComplete(() => onComplete?.Invoke());
        }
        
        private void MoveEnemiesToStartPosition(List<EnemyView> enemies)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                var targetPosition = _enemySpawnPoints[i].position;

                enemy.transform.DOMove(targetPosition, 2f).SetEase(Ease.OutQuad);
            }
        }
        
        private void PopInTurnButton(Vector3 targetScale)
        {
            _turnButton.transform.DOScale(targetScale, 1f).SetEase(Ease.OutBack);
        }

        private void RemoveHandView()
        {
            Destroy(_handView.GetComponent<Collider>());

            var rb = _handView.gameObject.AddComponent<Rigidbody>();
            
            Destroy(_handView, 5f);
        }
    }
}
