using DG.Tweening;
using Match;
using Project.Runtime.Scripts.Game.Matches;
using Project.Runtime.Scripts.General;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class EnergySystem : Singleton<EnergySystem>
{
    private static MatchPlayer SelfMatchPlayer => MatchController.Instance.SelfPlayer;
    
    [SerializeField] private Transform _energyStatusContainer;
    [SerializeField] private TextMeshProUGUI _energyText;

    private float _baseScale = 1f;
    private Tween _scaleTween;
    private int _currentEnergy = 0;

    [Button]
    public void UpdateEnergyText(int energy)
    {
        if (energy != _currentEnergy)
        {
            _currentEnergy = energy;
            _energyText.text = $"{energy}";

            if (_scaleTween != null && _scaleTween.IsActive())
                _scaleTween.Kill();

            _energyStatusContainer.localScale = _baseScale * Vector3.one;
            _scaleTween = _energyStatusContainer
                .DOScale(_baseScale * 1.2f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                    _energyStatusContainer.DOScale(_baseScale, 0.1f).SetEase(Ease.InQuad)
                );
        }
    }
}
