using System.Collections;
using MoreMountains.Feedbacks;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

namespace Turn
{
    public class TurnSystem : MonoBehaviour
    {
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
            
            // Simulate some delay for the enemy's turn
            yield return new WaitForSeconds(2f);
            
            // End the enemy's turn
            Debug.Log("Enemy's turn has ended.");
            _changeToPlayerTurnFeedback?.PlayFeedbacks();
        }
    }
}
