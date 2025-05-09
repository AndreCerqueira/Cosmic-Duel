using System.Text.RegularExpressions;
using Match;
using Project.Runtime.Scripts.Game.Cards.View;
using Project.Runtime.Scripts.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scripts.Game.Matches
{
    public class MatchController : Singleton<MatchController>
    {
        private Match _match;
        
        [Title("Prefabs")]
        [SerializeField] private Transform _playerPrefabContainer;
        [SerializeField] private MatchPlayerController _playerPrefab;
        //[SerializeField] private MatchEnemyController _enemyBotPrefab;
        
        [Title("Views")]
        [SerializeField] private HandView _handView;

        public MatchPlayer SelfPlayer => _match.SelfPlayer;
        
        private void Start()
        {
            var player = Instantiate(_playerPrefab, _playerPrefabContainer).MatchPlayer;
            //var enemy = Instantiate(_enemyBotPrefab, _playerPrefabContainer).MatchEnemy;
            _match = new Match(player);//, enemy);
            //_boardView.Setup(_match.Players);

            //StartMatch();
            StartMulligan();
        }

        private void StartMatch()
        {
            /*
#if UNITY_EDITOR
            if (_skipIntro)
            {
                _startMatchSkipIntroFeedback?.PlayFeedbacks();
                return;
            }
#endif
            _startMatchFeedback?.PlayFeedbacks();
            */
        }
        
        public void StartMulligan()
        {
            _handView.Setup(_match.SelfPlayer.Hand);
            
            //foreach (var player in _match.Players)
            SelfPlayer.DrawStartingHand();
            
            //_mulliganView.Setup(_match.SelfPlayer);
            _handView.Setup(_match.SelfPlayer.Hand);
        }
        
        
        public void StartGame()
        {
        }
    }
}
