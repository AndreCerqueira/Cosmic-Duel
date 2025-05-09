using Match;
using Project.Runtime.Scripts.Game.Cards.View;
using Project.Runtime.Scripts.Game.Matches;
using Project.Runtime.Scripts.General;
using UnityEngine;

namespace Cards.Systems
{
    public class CardSystem : Singleton<CardSystem>
    {
        private static MatchPlayer SelfMatchPlayer => MatchController.Instance.SelfPlayer;
        
        [SerializeField] private MatchController _matchController;
        [SerializeField] private HandView _handView;

        public bool IsAnyCardBeingDragged;
        
        /*
        private void OnEnable()
        {
            ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
            ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);

            ActionSystem.SubscribeReaction<SummonCharacterGA>(SummonCharacterReaction, ReactionTiming.POST);
        }

        private void OnDisable()
        {
            ActionSystem.DetachPerformer<PlayCardGA>();
            ActionSystem.DetachPerformer<DrawCardGA>();

            ActionSystem.UnsubscribeReaction<SummonCharacterGA>(SummonCharacterReaction, ReactionTiming.POST);
        }

        private IEnumerator DrawCardPerformer(DrawCardGA drawCardGA)
        {
            var card = drawCardGA.Player.DrawCard();
            if (card == null) yield break;

            Debug.Log($"[AC] Drawing card: {card.Name}");

            yield return null;
        }

        private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
        {
            Debug.Log($"[AC] Playing card: {playCardGA.CardView.name} at: {playCardGA.AreaView.name}");

            SummonCharacterGA summonCharacterGA = new(playCardGA.CardView, playCardGA.AreaView, playCardGA.Player);
            ActionSystem.Instance.AddReaction(summonCharacterGA);

            yield return null;
        }

        private void SummonCharacterReaction(SummonCharacterGA summonCharacterGA)
        {
            summonCharacterGA.Player.Hand.RemoveCard(summonCharacterGA.CardView.Card);

            DrawCardGA drawCardGA = new(summonCharacterGA.Player);
            ActionSystem.Instance.AddReaction(drawCardGA);
        }*/
    }
}
