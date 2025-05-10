using System.Collections;
using System.Collections.Generic;
using Cards.Systems;
using Effects.GA;
using Match;
using Project.Runtime.Scripts.Game.Matches;
using Project.Runtime.Scripts.General;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

public class DamageSystem : Singleton<MonoBehaviour>
{
    private static MatchPlayerController SelfMatchPlayer => MatchController.Instance.SelfPlayerController;
    
    private static List<EnemyView> EnemyViews => MatchController.Instance.Enemies;

    public GameObject SelectedObjectToDamage => CardSystem.Instance.SelectedObjectWithCard;
    
    
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
        ActionSystem.AttachPerformer<GainArmorGA>(GainArmorPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
        ActionSystem.DetachPerformer<GainArmorGA>();
    }

    private IEnumerator DealDamagePerformer(DealDamageGA action)
    {
        // Check if the target is valid
        if (SelectedObjectToDamage == null)
        {
            Debug.LogError("No target selected for damage.");
            yield break;
        }
        
        if (SelectedObjectToDamage.TryGetComponent(out MatchPlayerController player))
        {
            // Deal damage to the player
            player.DealDamage(action.Amount);
        }
        else if (SelectedObjectToDamage.TryGetComponent(out EnemyView enemy))
        {
            // Deal damage to the enemy
            enemy.DealDamage(action.Amount);
        }
        else
        {
            Debug.LogError("Selected object is not a valid target for damage.");
            yield break;
        }
        
        yield return null;
    }
    
    private IEnumerator GainArmorPerformer(GainArmorGA action)
    {
        // Check if the target is valid
        if (SelectedObjectToDamage == null)
        {
            Debug.LogError("No target selected for armor gain.");
            yield break;
        }
        
        if (SelectedObjectToDamage.TryGetComponent(out MatchPlayerController player))
        {
            // Gain armor for the player
            player.GainArmor(action.Amount);
        }
        else if (SelectedObjectToDamage.TryGetComponent(out EnemyView enemy))
        {
            // Gain armor for the enemy
            enemy.GainArmor(action.Amount);
        }
        else
        {
            Debug.LogError("Selected object is not a valid target for armor gain.");
            yield break;
        }
        
        yield return null;
    }
}
