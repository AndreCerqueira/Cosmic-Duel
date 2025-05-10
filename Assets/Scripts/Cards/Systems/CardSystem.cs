using System.Collections;
using Cards.Systems.GA;
using Cards.View;
using Match;
using Project.Runtime.Scripts.Game.Cards.View;
using Project.Runtime.Scripts.Game.Matches;
using Project.Runtime.Scripts.General;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

namespace Cards.Systems
{
    public class CardSystem : Singleton<CardSystem>
    {
        private static MatchPlayer SelfMatchPlayer => MatchController.Instance.SelfPlayer;
        
        [SerializeField] private MatchController _matchController;
        [SerializeField] private HandView _handView;

        public bool IsAnyCardBeingDragged;
        
        public GameObject SelectedObjectWithCard;
        
        private void OnEnable()
        {
            ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
            ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
        }

        private void OnDisable()
        {
            ActionSystem.DetachPerformer<PlayCardGA>();
            ActionSystem.DetachPerformer<DrawCardGA>();
        }

        private IEnumerator DrawCardPerformer(DrawCardGA drawCardGA)
        {
            for (int i = 0; i < drawCardGA.Amount; i++)
            {
                var card = MatchController.Instance.SelfPlayer.DrawCard();
                if (card == null) continue;

                Debug.Log($"[AC] Drawing card: {card.Name}");
            }

            yield return null;
        }

        private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
        {
            Debug.Log($"[AC] Playing card: {playCardGA.CardView.name} at: {playCardGA.Target.name}");
            
            SelectedObjectWithCard = playCardGA.Target;
            
            // Get effects from the card and apply them
            var cardEffects = playCardGA.CardView.Card.Effects;
            
            foreach (var effect in cardEffects)
            {
                PerformEffectGA performEffectGA = new(effect);
                ActionSystem.Instance.AddReaction(performEffectGA);
            }
            
            SelfMatchPlayer.Hand.RemoveCard(playCardGA.CardView.Card);
            _handView.UpdateCardPositions();
            
            yield return null;
        }

    }
}
