using Cards.Data;
using UnityEngine;

namespace Cards.Models
{
    public class Card
    {
        public readonly string UID;
        
        public string Name => _data.Name;
        public string Description => _data.Description;
        public Sprite Artwork => _data.Artwork;
        
        public int Cost { get; private set; }
        
        private readonly CardDataSO _data;
        
        public Card(CardDataSO cardData)
        {
            _data = cardData;
            Cost = cardData.Cost;
            
            UID = System.Guid.NewGuid().ToString();
        }
    }
}
