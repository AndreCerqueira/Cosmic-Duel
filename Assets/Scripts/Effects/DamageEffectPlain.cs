using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

public class DamageEffectPlain : EffectPlain
{
    public bool ToAllEnemies;
    
    public override GameAction GetGameAction()
    {
        Debug.Log($"Dealt {amount} damage.");
        
        DealDamageGA dealDamageGA = new(amount, ToAllEnemies);
        return dealDamageGA;
    }
}
