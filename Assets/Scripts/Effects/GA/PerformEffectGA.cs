using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

public class PerformEffectGA : GameAction
{
    public EffectPlain Effect { get; set; }
    
    public PerformEffectGA(EffectPlain effect)
    {
        Effect = effect;
    }
}
