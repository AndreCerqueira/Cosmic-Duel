using TMPro;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Armor
{
    public class ArmorView : MonoBehaviour
    {
        private float _baseScale = 0.7f; // Escala base do transform

        [SerializeField] private TextMeshProUGUI _armorText;

        private Tween _scaleTween;
        private int _currentArmor = -1; // Força atualização na 1ª vez

        private void Awake()
        {
            if (GetComponentInParent<EnemyView>() != null || GetComponent<EnemyView>() != null)
            {
                _baseScale = 0.07f;
            }
        }
        
        private void Start()
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.zero;
        }

        private void OnEnable()
        {
            transform.localScale = Vector3.zero;
            _scaleTween = transform.DOScale(_baseScale, 0.4f).SetEase(Ease.OutBack);
        }

        [Button]
        public void UpdateArmorText(int armor)
        {
            if (armor == _currentArmor)
                return;

            _currentArmor = armor;
            _armorText.text = $"{armor}";

            if (armor <= 0)
            {
                if (_scaleTween != null && _scaleTween.IsActive())
                    _scaleTween.Kill();

                _scaleTween = transform
                    .DOScale(Vector3.zero, 0.3f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true); // Chama OnEnable e faz pop-in
                }
                else
                {
                    // Pop curto se já estiver visível
                    if (_scaleTween != null && _scaleTween.IsActive())
                        _scaleTween.Kill();

                    transform.localScale = _baseScale * Vector3.one;
                    _scaleTween = transform
                        .DOScale(_baseScale * 1.2f, 0.1f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                            transform.DOScale(_baseScale, 0.1f).SetEase(Ease.InQuad)
                        );
                }
            }
        }
    }
}
