using Project.Runtime.Scripts.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters
{
    public class CharacterHoverEffect : MonoBehaviour
    {
        private const float PULSE_SPEED = 3f;
        private const float PULSE_INTENSITY = 0.5f;
    
        private readonly Color _hoverColor = new(0.8f, 0.8f, 0.8f);
        private readonly Color _availableColor = new(0.6f, 0.8627f, 0.4627f);
        private readonly Color _confirmColor = new(0.878f, 0.827f, 0.309f);
    
        private Renderer _renderer;
        private Color _originalColor;
        private Material _material;
        private float _pulseTimer = 0f;
   
        [ReadOnly] [SerializeField] private bool _hover;
        [ReadOnly] [SerializeField] private bool _available;
    
    
        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer == null) return;

            _material = _renderer.material;
            _originalColor = _material.color;
        }
    
        private void Update()
        {
            if (!_hover && !_available)
            {
                _pulseTimer = 0f;
                _material.color = _originalColor;
                return;
            }

            _pulseTimer += Time.deltaTime * PULSE_SPEED;
            var pulseValue = (Mathf.Sin(_pulseTimer) + 1f) * 0.5f;
            pulseValue = Mathf.Lerp(1f - PULSE_INTENSITY, 1f, pulseValue);

            var targetColor = GetTargetColor();
            _material.color = Color.Lerp(_originalColor, targetColor, pulseValue);
        }


        private Color GetTargetColor()
        {
            if (_hover && _available) return _confirmColor;
            if (_hover) return _hoverColor;
            if (_available) return _availableColor;

            return _originalColor;
        }
    

        private void UpdateCursor()
        {
            if (_hover && _available)
            {
                CursorManager.Instance.SetInteractCursor();
            }
            else if (_hover)
            {
                CursorManager.Instance.SetDefaultCursor();
            }
        }

        private void OnMouseEnter()
        {
            Debug.Log("Mouse Entered");
            _hover = true;
            UpdateCursor();
        }

        private void OnMouseExit()
        {
            Debug.Log("Mouse Exited");
            _hover = false;
            CursorManager.Instance.SetDefaultCursor();
        }

        public void SetAvailable() 
        { 
            _available = true;
            UpdateCursor();
        }

        public void ResetAvailable() 
        { 
            _available = false;
            if (_hover)
            {
                CursorManager.Instance.SetDefaultCursor();
            }
        }
    }
}
