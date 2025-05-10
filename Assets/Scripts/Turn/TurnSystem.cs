using System.Collections;
using MoreMountains.Feedbacks;
using Project.Runtime.Scripts.Game.Matches;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

namespace Turn
{
    public class TurnSystem : MonoBehaviour
    {
        private static MatchPlayerController SelfMatchPlayer => MatchController.Instance.SelfPlayerController;
        
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
            // Perform enemy turn logic here
            Debug.Log("Enemy's turn is being performed.");
            _changeToEnemyTurnFeedback?.PlayFeedbacks();
            
            
            // Deal 10 damage to the player
            SelfMatchPlayer.DealDamage(10);
            
            
            
            // Simulate some delay for the enemy's turn
            yield return new WaitForSeconds(2f);

            EnemyEndTurnReaction();
        }
        
        
        private void EnemyEndTurnReaction()
        {
            // Logic to handle the end of the enemy's turn
            Debug.Log("Enemy's turn has ended.");
            _changeToPlayerTurnFeedback?.PlayFeedbacks();
            
            SelfMatchPlayer.MatchPlayer.GainEnergy(3);
            
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
