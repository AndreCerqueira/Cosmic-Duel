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
            //var state = GameManager.Instance.CurrentPlanetState.difficulty;
            
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
            string difficulty = "";
            try
            {
                difficulty = GameManager.Instance.CurrentPlanetState.difficulty.ToString().ToLower();
            }
            catch (Exception _)
            {
            }
            
            // Se não conseguir pegar a dificuldade, pega uma aleatória
            if (string.IsNullOrEmpty(difficulty))
            {
                var difficulties = new List<string> { "easy", "medium", "hard", "boss" };
                difficulty = difficulties[UnityEngine.Random.Range(0, difficulties.Count)];
            }
            
            var selectedEnemies = new List<EnemyView>();

            // Probabilidade crescente de boss conforme dificuldade
            float bossChance = difficulty switch
            {
                "easy" => 0.0f,
                "medium" => 0.0f,
                "hard" => 0.75f,
                "boss" => 1.0f,
                _ => 0.0f
            };

            Debug.Log($"[MatchController] SpawnEnemies: {difficulty} - Boss chance: {bossChance}");
            bool spawnBoss = UnityEngine.Random.value < bossChance;
            Debug.Log($"[MatchController] Spawn boss: {spawnBoss}");

            if (spawnBoss)
            {
                var bossData = _allEnemies
                    .Where(e => e.IsBoss)
                    .OrderBy(e => UnityEngine.Random.value)
                    .FirstOrDefault();

                if (bossData != null)
                {
                    var boss = Instantiate(_enemyBotPrefab, _playerPrefabContainer).GetComponent<EnemyView>();
                    boss.Setup(bossData);
                    boss.transform.localScale *= 2;
                    boss._canvasTransform.localScale /= 2;
                    
                    // get boss collider  /2, 
                    /*
                    var bossCollider = boss.GetComponent<BoxCollider>();
                    if (bossCollider != null)
                    {
                        bossCollider.size = new Vector3(bossCollider.size.x / 2, bossCollider.size.y / 2, bossCollider.size.z /3);
                        bossCollider.center = new Vector3(bossCollider.center.x, bossCollider.center.y / 2, bossCollider.center.z);
                    }
                    else
                    {
                        Debug.LogWarning("Boss collider not found.");
                    }*/

                    boss.OnEnemyDeath += () =>
                    {
                        if (selectedEnemies.All(e => e.IsDead))
                        {
                            _VictoryPopup.gameObject.SetActive(true);
                            RemoveHandView();
                            Debug.Log("You Win");
                        }
                    };

                    selectedEnemies.Add(boss);

                    int allyCount = bossData.MaxAllies; // Número de aliados baseado no valor do int

                    if (allyCount > 0)
                    {
                        var cEnemies = _allEnemies.Where(e => !e.IsBoss).ToList();

                        // Número máximo de aliados = nº total de spawn points - 1 (o boss já ocupa um)
                        int maxAllies = Mathf.Min(allyCount, _enemySpawnPoints.Count - 1); // Garante que não ultrapassa o número de spawn points disponíveis
                        int allyCountToSpawn = maxAllies;//UnityEngine.Random.Range(1, maxAllies + 1);

                        for (int i = 0; i < allyCountToSpawn && selectedEnemies.Count < _enemySpawnPoints.Count; i++)
                        {
                            var enemyData = cEnemies[UnityEngine.Random.Range(0, cEnemies.Count)];
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

                    return selectedEnemies;
                }
            }

            // Caso não vá spawnar boss, decide quantos inimigos normais
            int enemyCount = difficulty switch
            {
                "easy" => UnityEngine.Random.Range(1, 3), // 1 ou 2
                "medium" => UnityEngine.Random.Range(2, 4), // 2 ou 3
                "hard" => 3,
                _ => UnityEngine.Random.Range(1, 3)
            };

            var commonEnemies = _allEnemies.Where(e => !e.IsBoss).ToList();

            for (int i = 0; i < enemyCount && i < _enemySpawnPoints.Count; i++)
            {
                var enemyData = commonEnemies[UnityEngine.Random.Range(0, commonEnemies.Count)];

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
