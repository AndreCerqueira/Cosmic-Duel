using System.Collections;
using System.Collections.Generic;
using Cards.Creators;
using Cards.Models;
using Cards.View;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Project.Runtime.Scripts.Game.Cards.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scripts.Game.Cards.View
{
    public class HandView : MonoBehaviour
    {
        private const float CARD_UPDATE_POSITION_DURATION = 0.75f;
        
        private Hand _hand;
        
        [Title("Hand Settings")]
        [SerializeField] private float _radius;
        [SerializeField] private float _angleStep;
        [SerializeField] private float _arcDegrees;
        [SerializeField] private float _verticalOffset;
        
        [Title("Feedbacks")]
        [SerializeField] private MMF_Player _cardStartDragFeedback;
        [SerializeField] private MMF_Player _cardEndDragFeedback;
        
        [Title("Card Views")]
        public List<CardView> _cardViews = new();

        public void Setup(Hand hand, List<CardView> cardViews = null)
        {
            Debug.Log("Hand criada");
            _cardViews.Clear();
            if (cardViews != null) 
                _cardViews.AddRange(cardViews);

            _hand = hand;
            
            _hand.CardAddedEvent += OnCardAdded;
            _hand.CardRemovedEvent += OnCardRemoved;
        }
        
        private void OnCardAdded(Card card)
        {
            Debug.Log("Card added to hand: " + card.Name);
            var cardView = CardViewCreator.Instance.CreateCardView(card, transform);
            _cardViews.Add(cardView);
            
            StartCoroutine(UpdateCardPositionsCoroutine(CARD_UPDATE_POSITION_DURATION));
        }
        
        
        private void OnCardRemoved(Card card)
        {
            var cardView = _cardViews.Find(c => c.Card == card);
            if (cardView == null) return;
            _cardViews.Remove(cardView);
            Destroy(cardView.gameObject);
            
            StartCoroutine(UpdateCardPositionsCoroutine(CARD_UPDATE_POSITION_DURATION));
        }

        
        public void UpdateCardPositions() => StartCoroutine(UpdateCardPositionsCoroutine(CARD_UPDATE_POSITION_DURATION));
        private IEnumerator UpdateCardPositionsCoroutine(float duration)
        {
            if (_hand.Count == 0) yield break;

            var count = _hand.Count;
            var totalArc = _angleStep * (count - 1);
            var startAngle = -totalArc / 2f;

            for (var i = 0; i < count; i++)
            {
                var angle = startAngle + _angleStep * i;
                var rad = Mathf.Deg2Rad * angle;

                var localPos = new Vector3(
                    Mathf.Sin(rad) * _radius,
                    Mathf.Cos(rad) * _radius + _verticalOffset,
                    0f
                );

                var worldPos = transform.TransformPoint(localPos);

                var toCamera = worldPos - Camera.main.transform.position;
                var lookRotation = Quaternion.LookRotation(toCamera, Vector3.up);
                
                var cardView = _cardViews[i];
                
                var sequence = DOTween.Sequence();
                sequence.Append(cardView.transform.DOMove(worldPos, duration)); 
                sequence.Join(cardView.transform.DORotateQuaternion(lookRotation, duration));
                sequence.OnComplete(() => { 
                    cardView.InputHandler.Setup(_cardStartDragFeedback, _cardEndDragFeedback);
                });
            }

            yield return new WaitForSeconds(duration);
        }
    }
}
