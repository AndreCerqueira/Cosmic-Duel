using System.Collections;
using Cards.Systems;
using Cards.Systems.GA;
using DG.Tweening;
using Match;
using MoreMountains.Feedbacks;
using Project.Runtime.Scripts.Game.Cards.View;
using Project.Runtime.Scripts.Game.Matches;
using Project.Runtime.Scripts.General;
using Project.Runtime.Scripts.General.ActionSystem;
using UnityEngine;

namespace Cards.View
{
    [RequireComponent(typeof(Collider))]
    public class CardInputHandler : MonoBehaviour
    {
        private const string DROP_TARGET_TAG = "Character";
        private static MatchPlayer SelfMatchPlayer => MatchController.Instance.SelfPlayer;
        
        [Header("Hover Settings")]
        [SerializeField] private float _hoverScale = 0.175f;
        [SerializeField] private float _hoverLift = 0.75f;

        [Header("Drag Settings")]
        [SerializeField] private float _dragLift = 1f;
        [SerializeField] private float _dragScale = 0.25f;
        [SerializeField] private float _maxTiltAngle = 15f;
        [SerializeField] private float _tiltResponse = 0.5f;
        [SerializeField] private LayerMask _boardLayerMask;

        [Header("Animation Settings")]
        [SerializeField] private float _duration = 0.2f;
        [SerializeField] private Ease _scaleEase = Ease.OutBack;
        [SerializeField] private Ease _moveEase = Ease.OutQuad;

        [Header("Card Wrapper")]
        [SerializeField] private Transform _cardWrapper;

        private Vector3 _originalScale;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private bool _isHovered;
        private bool _isDragging;
        private Vector3 _offset;
        private float _zDistance;
        private Vector3 _lastMouseWorldPos;
        private Tween _tiltTween;
        
        private CardViewAnimator _animator;
        private Collider _collider;

        private bool IsAnimatorValid => _animator != null;

        private void Awake()
        {
            enabled = false;
            _collider = GetComponent<Collider>();
            _originalScale = transform.localScale;
        }
        

        public void Setup(MMF_Player startDragFeedback, MMF_Player endDragFeedback)
        {
            enabled = true;
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;
            
            _animator = new CardViewAnimator(transform, _cardWrapper, _duration, _scaleEase, _moveEase, startDragFeedback, endDragFeedback);
        }
        

        private void OnMouseEnter()
        {
            if (!IsAnimatorValid) return;
            if (CardSystem.Instance.IsAnyCardBeingDragged) return;
            if (_isHovered || _isDragging) return;
            
            _isHovered = true;
            CursorManager.Instance.SetInteractCursor();
            
            _animator.AnimateHover(_originalPosition, _originalRotation, _hoverLift, _hoverScale);
        }

        private void OnMouseExit()
        {
            if (!IsAnimatorValid) return;
            if (!_isHovered || _isDragging) return;
            
            _isHovered = false;
            CursorManager.Instance.SetDefaultCursor();
            ReturnToOriginalPosition();
        }

        private void OnMouseDown()
        {
            if (!IsAnimatorValid) return;
            if (!_isHovered) return;
            
            _isDragging = true;
            CardSystem.Instance.IsAnyCardBeingDragged = true;
            _zDistance = Camera.main.WorldToScreenPoint(_cardWrapper.position).z;
            _offset = _cardWrapper.position - GetMouseWorldPos();
            _lastMouseWorldPos = GetMouseWorldPos();
            
            _animator.AnimateDrag(_originalPosition, _dragLift, _dragScale);
        }

        private void OnMouseDrag()
        {
            if (!IsAnimatorValid || !_isDragging) return;
            
            var velocity = (GetMouseWorldPos() - _lastMouseWorldPos) / Time.deltaTime;
            _lastMouseWorldPos = GetMouseWorldPos();
            var tiltX = Mathf.Clamp(-velocity.y * _tiltResponse, -_maxTiltAngle, _maxTiltAngle);
            var tiltZ = Mathf.Clamp(velocity.x * _tiltResponse, -_maxTiltAngle, _maxTiltAngle);

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _boardLayerMask))
            {
                var targetPos = hit.point + Vector3.up * 0.1f;
                var newRotationX = Mathf.Lerp(transform.localRotation.eulerAngles.x, 25, Time.deltaTime * 10f);
                
                _cardWrapper.position = Vector3.Lerp(_cardWrapper.position, targetPos, Time.deltaTime * 10f);
                transform.localRotation = Quaternion.Euler(newRotationX, transform.localRotation.y, transform.localRotation.z);
            }
            else
            {
                var currentMousePos = GetMouseWorldPos();
                var newRotationX = Mathf.Lerp(transform.localRotation.eulerAngles.x, 67.521f, Time.deltaTime * 10f);
                
                _cardWrapper.position = Vector3.Lerp(_cardWrapper.position, currentMousePos + _offset, Time.deltaTime * 10f);
                transform.localRotation = Quaternion.Euler(newRotationX, transform.localRotation.y, transform.localRotation.z);
            }

            _cardWrapper.localRotation = Quaternion.Euler(tiltX, 0f, tiltZ);
        }

        private void OnMouseUp()
        {
            if (!IsAnimatorValid) return;
            if (!_isDragging) return;
            
            _isDragging = false;
            CardSystem.Instance.IsAnyCardBeingDragged = false;
            _tiltTween?.Kill();
            _tiltTween = _animator.AnimateResetRotation();

            if (CheckDropTarget(out var hit))
            {
                enabled = false;
                _collider.enabled = false;
                
                var areaTransform = hit.collider.transform;

                var dropPosition = hit.point;// + Vector3.up * 0.1f; // pequeno offset visual
                
                // add offset to the left
                var offset = new Vector3(-2f, -0.5f, 0f);
                dropPosition += offset;
                
                var dropRotation = Quaternion.identity;
                
                _animator.AnimateDrop(dropPosition, dropRotation, () =>
                {
                    Debug.Log("Card successfully dropped and parented to " + (transform.parent != null ? transform.parent.name : "root"));
                    
                    if (ActionSystem.Instance.IsPerforming) return;
                    
                    //var enemyView = areaTransform.GetComponent<EnemyView>();
                    var cardView = GetComponent<CardView>();
                    
                    PlayCardGA playCardGA = new(cardView, areaTransform.gameObject);
                    ActionSystem.Instance.Perform(playCardGA);
                    
                });
            }
            else
            {
                ReturnToOriginalPosition();
            }
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = _zDistance;
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }

        private bool CheckDropTarget(out RaycastHit hitInfo)
        {
            hitInfo = default;
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                return false;

            if (hitInfo.collider == null || !hitInfo.collider.CompareTag(DROP_TARGET_TAG)) return false;
            
            // check if have energy to SelfMatchPlayer.Energy -= playCardGA.CardView.Card.Cost;
            if (SelfMatchPlayer.Energy < GetComponent<CardView>().Card.Cost) return false;

            // check if hitInfo have EnemyView component
            // var enemyView = hitInfo.collider.GetComponent<EnemyView>();
            // if (enemyView == null) return false;

            return true;
        }

        public void ReturnToOriginalPosition()
        {
            CursorManager.Instance.SetDefaultCursor();
            _animator.AnimateReturn(_originalScale, _originalPosition, _originalRotation);
            _isHovered = false;
        }
    }
}
