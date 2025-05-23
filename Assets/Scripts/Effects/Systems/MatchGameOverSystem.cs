using System.Collections;
using MoreMountains.Feedbacks;
using Project.Runtime.Scripts.General;
using UnityEngine;

public class MatchGameOverSystem : Singleton<MatchGameOverSystem>
{
    [SerializeField] private MMF_Player _loseChangeScreenFeedback;
    [SerializeField] private MMF_Player _winChangeScreenFeedback;

    public void GameOver(bool isWin)
    {
        if (isWin)
        {
            _winChangeScreenFeedback?.PlayFeedbacks();
        }
        else
        {
            StatusManager.Instance.ResetStatus();
            GameManager.Instance.ResetGame();

            _loseChangeScreenFeedback?.PlayFeedbacks();
        }
    }
}
