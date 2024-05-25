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
        private Tween _tween;
        
        public event Action Clicked;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UIAudioManager.Instance.PlayButtonHoverSound();
            
            _tween?.Complete();
            _tween = _button.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack);
            _tween.SetUpdate(true);

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tween?.Complete();
            _tween = _button.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack);
            _tween.SetUpdate(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _tween?.Complete();
            _tween = _button.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                UIAudioManager.Instance.PlayButtonClickSound();
                Clicked?.Invoke();
            });
            _tween.SetUpdate(true);
        }
    }
}