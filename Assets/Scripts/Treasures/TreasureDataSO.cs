using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Treasures
{
    [CreateAssetMenu(fileName = "New Treasure", menuName = "Treasure")]
    [Serializable]
    public class TreasureDataSO : SerializedScriptableObject
    {
        [Title("Treasure Info")] 
        public int Id;
        public new string Name;
        public string Description;
        public Sprite Artwork;
        public BorderType BorderType;
        
        public bool HaveRedDetail;
        public bool HaveBlueDetail;
        
        [Title("Effects")]
        public List<EffectPlain> Effects;
    }
}
