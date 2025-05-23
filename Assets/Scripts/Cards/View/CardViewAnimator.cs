using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Cards.View
{
    public class CardViewAnimator
    {
        private readonly Transform _transform;
        private readonly Transform _wrapper;
        private readonly float _duration;
        private readonly Ease _scaleEase;
        private readonly Ease _moveEase;
        
        private readonly MMF_Player _startDragFeedback;
        private readonly MMF_Player _endDragFeedback;

        
        public CardViewAnimator(Transform transform, Transform wrapper, float duration, Ease scaleEase, Ease moveEase, MMF_Player startDragFeedback, MMF_Player endDragFeedback)
        {
            _startDragFeedback = startDragFeedback;
            _endDragFeedback = endDragFeedback;
            
            _transform = transform;
            _wrapper = wrapper;
            _duration = duration;
            _scaleEase = scaleEase;
            _moveEase = moveEase;
        }

        
        public void AnimateHover(Vector3 originalPos, Quaternion originalRot, float hoverLift, float hoverScale)
        {
            _transform.DOComplete();
            _transform.DOScale(hoverScale, _duration).SetEase(_scaleEase);
            _transform.DOLocalMoveY(originalPos.y + hoverLift, _duration).SetEase(_moveEase);
            _transform.DOLocalRotate(new Vector3(originalRot.eulerAngles.x, 0, 0), _duration).SetEase(_moveEase);
        }
        

        public void AnimateReturn(Vector3 originalScale, Vector3 originalPos, Quaternion originalRot)
        {
            _transform.DOComplete();
            _wrapper.DOComplete();

            _transform.DOScale(originalScale, _duration).SetEase(_scaleEase);
            _transform.DOLocalMove(originalPos, _duration).SetEase(_moveEase);
            _transform.DOLocalRotate(originalRot.eulerAngles, _duration).SetEase(_moveEase);

            _wrapper.DOLocalMove(Vector3.zero, _duration).SetEase(_moveEase);
            _wrapper.DOLocalRotate(Vector3.zero, _duration).SetEase(_moveEase);
        }
        

        public void AnimateDrag(Vector3 originalPos, float dragLift, float dragScale)
        {
            _transform.DOComplete();
            _transform.DOScale(dragScale, _duration * 0.5f).SetEase(_scaleEase);
            _transform.DOLocalMoveY(originalPos.y + dragLift, _duration * 0.5f).SetEase(_moveEase);
            
            _startDragFeedback?.PlayFeedbacks(); 
        }
        
        
        public Tween AnimateDrop(Vector3 worldPosition, Quaternion worldRotation, System.Action onComplete = null)
        {
            _transform.DOComplete();
            _wrapper.DOComplete();

            // if (setParent)
            // {
            //     _transform.SetParent(null); // ou outro parent, se quiseres
            // }

            float dropDuration = _duration * 0.5f;

            _wrapper.DOScale(Vector3.one, dropDuration).SetEase(_scaleEase);
            _wrapper.DORotateQuaternion(Quaternion.identity, dropDuration).SetEase(_moveEase);

            _transform.DOMove(worldPosition, dropDuration).SetEase(_moveEase);
            _transform.DORotateQuaternion(worldRotation, dropDuration).SetEase(_moveEase);

            if (onComplete != null)
                DOVirtual.DelayedCall(dropDuration, () => onComplete());

            return null;
        }
        

        public Tween AnimateResetRotation()
        {
            _endDragFeedback?.PlayFeedbacks();
            return _wrapper.DOLocalRotate(Vector3.zero, 0.3f).SetEase(Ease.OutQuad);
        }
    }
}
