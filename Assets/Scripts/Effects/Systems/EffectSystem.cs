using System.Collections;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<PerformEffectGA>();
    }

    private IEnumerator PerformEffectPerformer(PerformEffectGA action)
    {
        var effect = action.Effect.GetGameAction();
        ActionSystem.Instance.AddReaction(effect);
        yield return null;
    }
}
