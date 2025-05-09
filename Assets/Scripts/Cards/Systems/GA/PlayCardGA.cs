using Cards.View;
using Project.Runtime.Scripts.Game.Cards.View;
using Project.Runtime.Scripts.General.ActionSystem;

namespace Cards.Systems.GA
{
    public class PlayCardGA : GameAction
    {
        public CardView CardView { get; private set; }
        
        public PlayCardGA(CardView cardView)
        {
            CardView = cardView;
        }
    }
}
