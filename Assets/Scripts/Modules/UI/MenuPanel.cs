using UnityEngine;

namespace DuckJam.Modules
{
    [RequireComponent(typeof(CanvasGroup))]
    internal sealed class MenuPanel : MonoBehaviour
    {
        [SerializeField] private UIPanel panel = UIPanel.None;
        [SerializeField] private EscapeAction escapeAction = EscapeAction.None;
        
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
            HidePanel(CanvasGroup);
        }
        
        public void ShowImmediate()
        {
            ShowPanel(CanvasGroup);
        }
        
        public void Hide()
        {
            HidePanel(CanvasGroup);
        }
        
        public void Show()
        {
            ShowPanel(CanvasGroup);
        }
        
        private static void HidePanel(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        private static void ShowPanel(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}