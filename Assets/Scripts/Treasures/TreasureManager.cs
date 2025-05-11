using System;
using System.Collections.Generic;
using DG.Tweening;
using Match;
using MoreMountains.Feedbacks;
using Project.Runtime.Scripts.Game.Matches;
using Project.Runtime.Scripts.General;
using Treasures;
using UnityEngine;
using UnityEngine.UI;

public class TreasureManager : Singleton<TreasureManager>
{
    private static MatchPlayerController SelfMatchPlayer => MatchController.Instance.SelfPlayerController;
    
    [SerializeField] private MMF_Player _closeTreasurePopupFeedback;
    
    [SerializeField] private TreasureDataSO[] _treasureDataSos;
    
    [SerializeField] private TreasureView[] _treasureViews;
    
    public void Setup()
    {
        Debug.Log("TreasureManager started");
        SetupTreasures();
    }

    private void SetupTreasures()
    {
        var chosenTreasures = GetRandomTreasures(3);

        for (int i = 0; i < _treasureViews.Length; i++)
        {
            var treasureData = chosenTreasures[i];

            Action onClick = () =>
            {
                OnTreasureClicked(treasureData);
            };
            
            _treasureViews[i].Setup(treasureData);
            _treasureViews[i].InputHandler.Setup(onClick);
            
            Vector3 targetPosition = i switch
            {
                0 => new Vector3(-5f, 3f, 0f),
                1 => new Vector3(0f, 3f, 0f),
                2 => new Vector3(5f, 3f, 0f),
                _ => Vector3.zero
            };

            // Move a carta para a posição com uma animação de 0.5 segundos
            _treasureViews[i].transform.DOLocalMove(targetPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }
    
    private void OnTreasureClicked(TreasureDataSO data)
    {
        DisableAllInputs();
    
        switch (data.Id)
        {
            case 0: 
                GainHealthBonus();
                break;
            case 1: 
                GainFuelBonus();
                break;
            case 2:
                GainArmorBonus();
                break;
            case 3:
                GainDamageBonus();
                break;
            default:
                DoMatchEnd();
                break;
        }
    }

    private void DisableAllInputs()
    {
        foreach (var view in _treasureViews)
        {
            view.InputHandler.enabled = false;
        }
    }

    private TreasureDataSO[] GetRandomTreasures(int count)
    {
        var copyList = new List<TreasureDataSO>(_treasureDataSos);
        var result = new TreasureDataSO[count];
        var random = new System.Random();

        for (int i = 0; i < count; i++)
        {
            int index = random.Next(copyList.Count);
            result[i] = copyList[index];
            copyList.RemoveAt(index);
        }

        return result;
    }
    
    private void GainHealthBonus()
    {
        // Logic to gain health bonus
        Debug.Log("Gained Health Bonus!");
        
        StatusManager.Instance.SetHealth(SelfMatchPlayer.MatchPlayer.Health);
        StatusManager.Instance.RegenHealth(10);
        SelfMatchPlayer.MatchPlayer.Health = StatusManager.Instance.CurrentHealth;
        
        DoMatchEnd();
    }
    
    private void GainFuelBonus()
    {
        // Logic to gain fuel bonus
        Debug.Log("Gained Fuel Bonus!");
        
        StatusManager.Instance.RegenFuel(10);
        
        DoMatchEnd();
    }
    
    private void GainArmorBonus()
    {
        // Logic to gain armor bonus
        Debug.Log("Gained Armor Bonus!");
        StatusManager.Instance.AddArmorBonus();
        
        DoMatchEnd();
    }
    
    private void GainDamageBonus()
    {
        // Logic to gain damage bonus
        Debug.Log("Gained Damage Bonus!");
        StatusManager.Instance.AddDamageBonus();

        DoMatchEnd();
    }
    
    private void DoMatchEnd()
    {
        // Save remaining health
        Debug.Log("Saving remaining health");
        StatusManager.Instance.SetHealth(SelfMatchPlayer.MatchPlayer.Health);
        
        var state = GameManager.Instance.CurrentPlanetState;
        if (state != null)
        {
            state.completed = true;
            state.hidden = false;
        }
        GameManager.Instance.ExitPlanet();
        
        _closeTreasurePopupFeedback?.PlayFeedbacks();
        MatchGameOverSystem.Instance.GameOver(true);
    }
}
