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
        // Verificar se a ação deve afetar todos os inimigos
        bool isToAllEnemies = action.ToAllEnemies;

        if (isToAllEnemies)
        {
            // Aplica dano a todos os inimigos
            foreach (var enemy in EnemyViews)
            {
                if (enemy != null)
                {
                    var bonusAmount = StatusManager.Instance.DamageBonus;
                    enemy.DealDamage(action.Amount + bonusAmount);
                }
            }
        }
        else
        {
            // Se o dano for para um único alvo, verificar qual é o alvo selecionado
            if (SelectedObjectToDamage == null)
            {
                Debug.LogError("No target selected for damage.");
                yield break;
            }

            if (SelectedObjectToDamage.TryGetComponent(out MatchPlayerController player))
            {
                // Dano para o jogador
                var bonusAmount = StatusManager.Instance.DamageBonus;
                player.DealDamage(action.Amount + bonusAmount);
            }
            else if (SelectedObjectToDamage.TryGetComponent(out EnemyView enemy))
            {
                // Dano para o inimigo
                var bonusAmount = StatusManager.Instance.DamageBonus;
                enemy.DealDamage(action.Amount + bonusAmount);
            }
            else
            {
                Debug.LogError("Selected object is not a valid target for damage.");
                yield break;
            }
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
            var bonusAmount = StatusManager.Instance.ArmorBonus;
            player.GainArmor(action.Amount + bonusAmount);
        }
        else if (SelectedObjectToDamage.TryGetComponent(out EnemyView enemy))
        {
            // Gain armor for the enemy
            var bonusAmount = StatusManager.Instance.ArmorBonus;
            enemy.GainArmor(action.Amount + bonusAmount);
        }
        else
        {
            Debug.LogError("Selected object is not a valid target for armor gain.");
            yield break;
        }
        
        yield return null;
    }
}
