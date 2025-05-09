using Cards.Models;
using Cards.View;
using DG.Tweening;
using Project.Runtime.Scripts.Game.Cards.View;
using Project.Runtime.Scripts.General;
using UnityEngine;

namespace Cards.Creators
{
    public class CardViewCreator : Singleton<CardViewCreator>
    {
        private const float CARD_VIEW_SCALE_DURATION = 0.3f;
        
        [SerializeField] private CardView _cardViewPrefab;
        [SerializeField] private Transform _cardSpawnPoint;
        
        public CardView CreateCardView(Card card, Transform parent)
        {
            Debug.Log($"Creating card view for: {card.Name}");
            var cardView = Instantiate(_cardViewPrefab, parent);
            
            var originalScale = cardView.transform.localScale;
            cardView.transform.localScale = Vector3.zero;
            
            var startWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, -0.1f, 10f));
            cardView.transform.position = startWorldPos;
            cardView.transform.DOScale(originalScale, CARD_VIEW_SCALE_DURATION);
            
            cardView.Setup(card);
            
            return cardView;
        }
    }
}
