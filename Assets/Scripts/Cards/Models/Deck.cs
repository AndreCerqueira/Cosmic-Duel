using System.Collections.Generic;
using Cards.Data;
using Cards.Models;
using JetBrains.Annotations;
using UnityEngine;

namespace Project.Runtime.Scripts.Game.Cards.Models
{
    public class Deck
    {
        private readonly List<Card> _cards;
        private readonly DeckDataSO _data;
    
        public Deck(DeckDataSO deckData)
        {
            _data = deckData;
            
            _cards = new List<Card>();
            foreach (var cardData in deckData.Cards)
            {
                var card = new Card(cardData);
                _cards.Add(card);
            }
            
            Shuffle();
        }
        
        
        public void AddCard(Card card) => _cards.Add(card);
        public void AddCards(List<Card> cards) => _cards.AddRange(cards);
        
        
        // Fisher-Yates algorithm.
        public void Shuffle()
        {
            var n = _cards.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Range(0, n + 1);
                (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
            }
        }
        
        
        [CanBeNull]
        public Card DrawCard()
        {
            if (_cards.Count == 0)
            {
                Debug.LogError("No cards left in the deck.");
                return null;
            }

            var card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }
    }
}
