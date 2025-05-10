using Cards.Systems.GA;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

public class DrawCardEffect : EffectPlain
{
    public int amount;

    public override GameAction GetGameAction()
    {
        Debug.Log($"Draw {amount} cards.");
        
        DrawCardGA drawCardGA = new(amount);
        return drawCardGA;
    }
}
