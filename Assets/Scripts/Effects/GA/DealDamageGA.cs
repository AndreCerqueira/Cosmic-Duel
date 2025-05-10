using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

public class DealDamageGA : GameAction
{
    public int Amount { get; private set; }
    public DealDamageGA(int amount)
    {
        Amount = amount;
    }
}
