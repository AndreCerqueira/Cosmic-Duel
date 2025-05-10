using Project.Runtime.Scripts.General.ActionSystem;

namespace Cards.Systems.GA
{
    public class DrawCardGA : GameAction
    {
        public int Amount { get; private set; }
        
        public DrawCardGA(int amount)
        {
            Amount = amount;
        }
    }
}
