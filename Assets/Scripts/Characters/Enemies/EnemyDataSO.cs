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
        
        [Title("Stats")]
        public int Health;
        public int BaseAttack;
        public float Altura;
        public bool needFlip;
        public bool IsBoss;
        
        public AnimatorOverrideController EnemyAnimator;
    }
}
