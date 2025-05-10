using System.Collections;
using DG.Tweening;
using Match;
using MoreMountains.Feedbacks;
using Project.Runtime.Scripts.Game.Matches;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

namespace Turn
{
    public class TurnSystem : MonoBehaviour
    {
        private static MatchPlayerController SelfMatchPlayer => MatchController.Instance.SelfPlayerController;
        
        [SerializeField] private Transform _enemyAttackDestination;
        
        [Header("Feedbacks")]
        [SerializeField] private MMF_Player _changeToEnemyTurnFeedback;
        [SerializeField] private MMF_Player _changeToPlayerTurnFeedback;

        
        private void OnEnable()
        {
            ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        }

        private void OnDisable()
        {
            ActionSystem.DetachPerformer<EnemyTurnGA>();
        }
        
        
        private IEnumerator EnemyTurnPerformer(EnemyTurnGA action)
        {
            Debug.Log("Enemy's turn is being performed.");
            _changeToEnemyTurnFeedback?.PlayFeedbacks();

            var enemies = MatchController.Instance.Enemies;
            var player = SelfMatchPlayer;

            foreach (var enemy in enemies)
            {
                if (enemy.IsDead) continue;

                // 2. Executar ataque
                enemy.LoseAllArmor();

                
                //var offset = 5f;

                var originalPosition = enemy.transform.position;

                var attack = enemy.NextAttack;
                
                //If the attack is hidden, reveal the text
                enemy.RevealHiddenAttack();

                if (attack.Damage > 0)
                {
                    var targetPosition = _enemyAttackDestination.position;
                    yield return enemy.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutQuad).WaitForCompletion();
                    
                    player.DealDamage(attack.Damage);
                    Debug.Log($"{enemy.name} dealt {attack.Damage} damage.");
                }

                if (attack.Armor > 0)
                {
                    // Pop Shake
                    var originalScale = enemy.transform.localScale;
                    var targetScale = originalScale * 1.2f;
                    yield return enemy.transform.DOScale(targetScale, 0.2f).SetEase(Ease.OutBack).WaitForCompletion();
                    yield return enemy.transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack).WaitForCompletion();
                    
                    enemy.GainArmor(attack.Armor);
                    Debug.Log($"{enemy.name} gained {attack.Armor} armor.");
                }

                enemy.GenerateNextAttack();

                // 3. Pequena pausa após ataque
                yield return new WaitForSeconds(0.3f);

                // 4. Voltar à posição original
                yield return enemy.transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InSine).WaitForCompletion();

                // 5. Pequena pausa entre inimigos
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.5f); // Buffer final

            EnemyEndTurnReaction();
        }

        
        
        private void EnemyEndTurnReaction()
        {
            // Logic to handle the end of the enemy's turn
            Debug.Log("Enemy's turn has ended.");
            _changeToPlayerTurnFeedback?.PlayFeedbacks();
            
            SelfMatchPlayer.MatchPlayer.ResetEnergy(3);
            
            // draw cards until having 5
            while (SelfMatchPlayer.MatchPlayer.Hand.Count < 5)
            {
                var card = SelfMatchPlayer.MatchPlayer.DrawCard();
                if (card == null) break; // No more cards to draw
            }
            
            // lose all armor
            SelfMatchPlayer.LoseAllArmor();
        }
    }
}
