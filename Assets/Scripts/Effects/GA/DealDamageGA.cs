using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

public class DealDamageGA : GameAction
{
    public int Amount { get; private set; }
    public bool ToAllEnemies { get; private set; }
    public DealDamageGA(int amount, bool toAllEnemies)
    {
        ToAllEnemies = toAllEnemies;
        Amount = amount;
    }
}
