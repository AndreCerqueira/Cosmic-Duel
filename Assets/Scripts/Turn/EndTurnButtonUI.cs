using MoreMountains.Feedbacks;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Turn
{
    public class EndTurnButtonUI : MonoBehaviour
    {
        [SerializeField] private MMF_Player _pointerExitFeedback;
        
        public void OnClick()
        {
            if (ActionSystem.Instance.IsPerforming)
            {
                Debug.Log("Action is already being performed.");
                return;
            }
            
            EnemyTurnGA enemyTurnGA = new EnemyTurnGA();
            ActionSystem.Instance.Perform(enemyTurnGA);
        }
    }
}
