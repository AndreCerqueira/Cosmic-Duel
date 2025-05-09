using System;
using Sirenix.OdinInspector;
using UnityEngine;

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
        
        [Title("Stats")]

        [MinValue(0)]
        [MaxValue(3)]
        public int Cost;
    }
}
