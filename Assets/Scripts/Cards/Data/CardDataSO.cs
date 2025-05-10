using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum BorderType
{
    Copper,
    Silver,
    Golden
}

namespace Cards.Data
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    [Serializable]
    public class CardDataSO : SerializedScriptableObject
    {
        [Title("Card Info")]
        public new string Name;
        public string Description;
        public Sprite Artwork;
        public BorderType BorderType;
        
        public bool HaveRedDetail;
        public bool HaveBlueDetail;
        
        [Title("Stats")]

        [MinValue(0)]
        [MaxValue(3)]
        public int Cost;
        
        [Title("Effects")]
        public List<EffectPlain> Effects;
    }
}
