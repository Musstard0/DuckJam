using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(CanvasGroup))]
    internal sealed class MenuPanel : MonoBehaviour
    {
        private const float FadeDuration = 0.3f;
        
        
        [SerializeField] private TMP_Text optionalText;
        
        [SerializeField] private UIPanel panel = UIPanel.None;
        [SerializeField] private EscapeAction escapeAction = EscapeAction.None;
        
        private Sequence _sequence;
        
        
        public string OptionalText
        {
            set
            {
                if(optionalText == null) return;
                optionalText.text = value;
            }
        }

        public UIPanel Panel => panel;
        public EscapeAction EscapeAction => escapeAction;
        
        public CanvasGroup CanvasGroup { get; private set; }
        
        private void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            HideImmediate();
        }
        
        public void HideImmediate()
        {
            CanvasGroup.alpha = 0f;
            DisablePanel(CanvasGroup);
        }
        
        public void ShowImmediate()
        {
            CanvasGroup.alpha = 1f;
            EnablePanel(CanvasGroup);
        }
        
        public void Hide(Action onComplete = null)
        {
            _sequence?.Complete();
            
            _sequence = DOTween.Sequence()
                .AppendCallback(() => DisablePanel(CanvasGroup))
                .Append(CanvasGroup.DOFade(0f, FadeDuration).From(1f).SetEase(Ease.InSine))
                .OnComplete(() => onComplete?.Invoke());

            _sequence.SetUpdate(true);
        }
        
        public void Show(Action onComplete = null)
        {
            _sequence?.Complete();
            
            _sequence = DOTween.Sequence()
                .Append(CanvasGroup.DOFade(1f, FadeDuration).From(0f).SetEase(Ease.OutSine))
                .OnComplete(() =>
                {
                    EnablePanel(CanvasGroup);
                    onComplete?.Invoke();
                });
            
            _sequence.SetUpdate(true);
        }
        
        private static void DisablePanel(CanvasGroup canvasGroup)
        {
            //canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        private static void EnablePanel(CanvasGroup canvasGroup)
        {
            //canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}