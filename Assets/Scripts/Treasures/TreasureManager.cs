using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class TreasureManager : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _treasureLayoutGroup;
    [SerializeField] private Button _gainHealthBonusButton;
    [SerializeField] private Button _gainFuelBonusButton;
    [SerializeField] private Button _gainArmorBonusButton;
    [SerializeField] private Button _gainDamageBonusButton;
    [SerializeField] private MMF_Player _closeTreasurePopupFeedback;
    
    private void Start()
    {
        _gainHealthBonusButton.onClick.AddListener(GainHealthBonus);
        _gainFuelBonusButton.onClick.AddListener(GainFuelBonus);
        _gainArmorBonusButton.onClick.AddListener(GainArmorBonus);
        _gainDamageBonusButton.onClick.AddListener(GainDamageBonus);
        
        // disable all, enable 3 random
        DisableAllTreasures();
        EnableRandomTreasures(3);
    }
    
    private void DisableAllTreasures()
    {
        foreach (Transform child in _treasureLayoutGroup.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    
    private void EnableRandomTreasures(int count)
    {
        int enabledCount = 0;
        Transform[] children = new Transform[_treasureLayoutGroup.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = _treasureLayoutGroup.transform.GetChild(i);
        }

        while (enabledCount < count)
        {
            int randomIndex = Random.Range(0, children.Length);
            if (!children[randomIndex].gameObject.activeSelf)
            {
                children[randomIndex].gameObject.SetActive(true);
                enabledCount++;
            }
        }
    }
    
    private void GainHealthBonus()
    {
        // Logic to gain health bonus
        Debug.Log("Gained Health Bonus!");
        
        DoMatchEnd();
    }
    
    private void GainFuelBonus()
    {
        // Logic to gain fuel bonus
        Debug.Log("Gained Fuel Bonus!");
        
        DoMatchEnd();
    }
    
    private void GainArmorBonus()
    {
        // Logic to gain armor bonus
        Debug.Log("Gained Armor Bonus!");
        BonusManager.Instance.AddArmorBonus();
        
        DoMatchEnd();
    }
    
    private void GainDamageBonus()
    {
        // Logic to gain damage bonus
        Debug.Log("Gained Damage Bonus!");
        BonusManager.Instance.AddDamageBonus();

        DoMatchEnd();
    }
    
    private void DoMatchEnd()
    {
        _closeTreasurePopupFeedback?.PlayFeedbacks();
        MatchGameOverSystem.Instance.GameOver(true);
    }
}
