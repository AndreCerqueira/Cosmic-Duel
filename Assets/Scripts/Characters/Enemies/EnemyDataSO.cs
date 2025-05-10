using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Enemies
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
    [Serializable]
    public class EnemyDataSO : SerializedScriptableObject
    {
        public new string Name;
        public Sprite Artwork;
        
        [Title("Stats")]
        public int Health;
        public int BaseAttack;
        
        public AnimatorOverrideController EnemyAnimator;
    }
}
