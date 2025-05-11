using Effects.GA;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

public class GainArmorEffect : EffectPlain
{
    public override GameAction GetGameAction()
    {
        Debug.Log($"Gained {amount} armor.");
        
        GainArmorGA gainArmorGA = new(amount);
        return gainArmorGA;
    }
}
