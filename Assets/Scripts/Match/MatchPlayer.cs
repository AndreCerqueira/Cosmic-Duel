using Cards.Data;
using Cards.Models;
using JetBrains.Annotations;
using Project.Runtime.Scripts.Game.Cards.Models;
using UnityEngine;

namespace Match
{
    public class MatchPlayer
    {
        private const int HAND_START_SIZE = 5;
        private const int HAND_MAX_SIZE = 10;
        
        private readonly DeckDataSO _deckData;
        
        public string Name { get; private set; }
        
        public Deck Deck { get; set; }
        public Hand Hand { get; set; }
        
        public int Health { get; set; }
        public int MaxHealth;
        
        public int Armor { get; set; }
        
        
        public MatchPlayer(DeckDataSO deckData, string name, int health)
        {
            Health = health;
            MaxHealth = health;
            Armor = 0;
            _deckData = deckData;
            Name = name;
            Deck = new Deck(_deckData);
            Hand = new Hand(HAND_MAX_SIZE);
        }
        
        
        public void DrawStartingHand()
        {
            for (var i = 0; i < HAND_START_SIZE; i++)
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
        
        
        public void DealDamage(int amount)
        {
            Health -= amount;
            if (Health < 0) Health = 0;
            
            Debug.Log($"{Name} took {amount} damage. Remaining health: {Health}");
        }
    }
}
