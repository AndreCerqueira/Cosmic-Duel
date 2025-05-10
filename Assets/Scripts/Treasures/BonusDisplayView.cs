using TMPro;
using UnityEngine;

public class BonusDisplayView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _damageBonusText;
    [SerializeField] private TextMeshProUGUI _armorBonusText;

    [SerializeField] private GameObject _container;
    [SerializeField] private GameObject _damageBonusContainer;
    [SerializeField] private GameObject _armorBonusContainer;
    
    private void Start()
    {
        UpdateBonusDisplay();
    }
    
    private void UpdateBonusDisplay()
    {
        if (BonusManager.Instance.DamageBonus > 0)
            _damageBonusText.text = $"+{BonusManager.Instance.DamageBonus}";
        else
            _damageBonusContainer.SetActive(false);
        
        if (BonusManager.Instance.ArmorBonus > 0)
            _armorBonusText.text = $"+{BonusManager.Instance.ArmorBonus}";
        else
            _armorBonusContainer.SetActive(false);
        
        // if both are disabled, hide the container
        if (!_damageBonusContainer.activeSelf && !_armorBonusContainer.activeSelf)
        {
            _container.SetActive(false);
        }
        else
        {
            _container.SetActive(true);
        }
    }
}
