using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

[System.Serializable]
public abstract class EffectPlain
{
    public int amount;
    
    public abstract GameAction GetGameAction();
}
