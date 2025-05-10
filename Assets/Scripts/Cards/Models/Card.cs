using System.Collections.Generic;
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
        
        public BorderType BorderType => _data.BorderType;
        public bool HaveRedDetail => _data.HaveRedDetail;
        public bool HaveBlueDetail => _data.HaveBlueDetail;
        
        public List<EffectPlain> Effects => _data.Effects;
        
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
