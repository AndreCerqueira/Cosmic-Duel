using System;
using System.Collections.Generic;
using Cards.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Runtime.Scripts.Game.Cards.Models
{
    public class Hand
    {
        private readonly int _maxHandSize;
        
        public List<Card> Cards { get; private set; }
        public int Count => Cards.Count;

        
        public event Action<Card> CardAddedEvent;
        public event Action<Card> CardRemovedEvent;
        
        
        public Hand(int maxHandSize)
        {
            _maxHandSize = maxHandSize;
            Cards = new List<Card>(maxHandSize);
        }
        
        
        private void OnCardAdded(Card card) => CardAddedEvent?.Invoke(card);
        private void OnCardRemoved(Card card) => CardRemovedEvent?.Invoke(card);
        
        public bool CanAddCard() => Cards.Count < _maxHandSize;
        
        public void AddCard(Card card)
        {
            if (!CanAddCard())
            {
                Debug.LogWarning("Hand is full");
                return;
            }

            Cards.Add(card);
            OnCardAdded(card);
        }
        
        
        public void RemoveCard(Card card)
        {
            if (card == null)
            {
                Debug.LogError("Card is null");
                return;
            }

            if (!Cards.Remove(card))
            {
                Debug.LogError("Card not found in hand");
            }
            
            OnCardRemoved(card);
        }
        
        
        public void Clear()
        {
            Cards.Clear();
        }
    }
}
