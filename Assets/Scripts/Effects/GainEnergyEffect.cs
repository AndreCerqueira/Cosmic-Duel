using Effects.GA;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

public class GainEnergyEffect : EffectPlain
{
    public override GameAction GetGameAction()
    {
        Debug.Log($"Dealt {amount} damage.");
        
        GainEnergyGA gainEnergyGA = new(amount);
        return gainEnergyGA;
    }
}
