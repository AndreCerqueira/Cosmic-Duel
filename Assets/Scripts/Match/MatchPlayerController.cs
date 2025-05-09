using Cards.Data;
using Match;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scripts.Game.Matches
{
    public class MatchPlayerController : MonoBehaviour
    {
        [Title("Player")]
        [SerializeField] private DeckDataSO _deckData;
        [SerializeField] private string _playerName;
        
        public MatchPlayer MatchPlayer { get; private set; }
        
        
        private void OnEnable()
        {
            MatchPlayer = new MatchPlayer(_deckData, _playerName);
        }
    }
}
