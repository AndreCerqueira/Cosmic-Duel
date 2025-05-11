using Project.Runtime.Scripts.General.ActionSystem;

namespace Effects.GA
{
    public class GainEnergyGA : GameAction
    {
        public int Amount { get; private set; }
        public GainEnergyGA(int amount)
        {
            Amount = amount;
        }
    }
}