using Cards.Data;
using Cards.Models;
using JetBrains.Annotations;
using Project.Runtime.Scripts.Game.Cards.Models;
using UnityEngine;

namespace Match
{
    public class MatchPlayer
    {
        private const int HAND_SIZE = 5;
        
        private readonly DeckDataSO _deckData;
        
        public string Name { get; private set; }
        
        public Deck Deck { get; set; }
        public Hand Hand { get; set; }
        
        
        public MatchPlayer(DeckDataSO deckData, string name)
        {
            _deckData = deckData;
            Name = name;
            Deck = new Deck(_deckData);
            Hand = new Hand(HAND_SIZE);
        }
        
        
        public void DrawStartingHand()
        {
            for (var i = 0; i < HAND_SIZE; i++)
            {
                DrawCard();
            }
        }
        
        
        [CanBeNull]
        public Card DrawCard()
        {
            var card = Deck.DrawCard();
            if (card == null) return null;
        
            Hand.AddCard(card);
            Debug.Log($"Drew card: {card.Name} for {Name}");
            
            return card;
        }


        public void ReplaceStartingHand()
        {
            Deck.AddCards(Hand.Cards);
            Deck.Shuffle();
            Hand.Clear();
            DrawStartingHand();
        }
    }
}
