using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cards.Data
{
    [CreateAssetMenu(fileName = "New Deck", menuName = "Deck")]
    [Serializable]
    public class DeckDataSO : SerializedScriptableObject
    {
        [Title("Cards")]
        [ListDrawerSettings(ShowPaging = false)]
        public CardDataSO[] Cards = new CardDataSO[15];
    }
}
