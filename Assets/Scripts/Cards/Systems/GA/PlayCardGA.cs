using Cards.View;
using Project.Runtime.Scripts.Game.Cards.View;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

namespace Cards.Systems.GA
{
    public class PlayCardGA : GameAction
    {
        public CardView CardView { get; private set; }
        public GameObject Target { get; private set; }
        
        public PlayCardGA(CardView cardView, GameObject target)
        {
            CardView = cardView;
            Target = target;
        }
    }
}
