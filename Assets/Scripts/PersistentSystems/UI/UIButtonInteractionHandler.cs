using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(Button))]
    internal sealed class UIButtonInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Button _button;
        //private Tween _tween;
        private Sequence _sequence;
        
        public event Action Clicked;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UIAudioManager.Instance.PlayButtonHoverSound();
            
            _sequence?.Complete();
            _sequence = DOTween.Sequence()
                .Append(_button.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack))
                .SetUpdate(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _sequence?.Complete();
            _sequence = DOTween.Sequence()
                .Append(_button.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack))
                .SetUpdate(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _sequence?.Complete();
            _sequence = DOTween.Sequence()
                .Append(_button.transform.DOScale(0.8f, 0.1f).SetEase(Ease.OutBack))
                .AppendCallback(() => UIAudioManager.Instance.PlayButtonClickSound())
                .Append(_button.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack))
                .OnComplete(() => Clicked?.Invoke())
                .SetUpdate(true);

        }
    }
}