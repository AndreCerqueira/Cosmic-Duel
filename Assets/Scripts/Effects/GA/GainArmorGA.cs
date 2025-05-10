using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

namespace Effects.GA
{
    public class GainArmorGA : GameAction
    {
        public int Amount { get; private set; }
        public GainArmorGA(int amount)
        {
            Amount = amount;
        }
    }
}
