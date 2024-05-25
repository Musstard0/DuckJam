using UnityEngine;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(UIButtonInteractionHandler))]
    internal sealed class UINavigationButtonHandler : MonoBehaviour
    {
        [SerializeField] private UIPanel navigateToPanel;
        
        private UIButtonInteractionHandler _interactionHandler;
        
        private void Awake()
        {
            _interactionHandler = GetComponent<UIButtonInteractionHandler>();
#if UNITY_EDITOR
            if (navigateToPanel == UIPanel.None)
            {
                Debug.LogError($"No panel to navigate to", this);
            }
#endif
        }

        private void OnEnable() => _interactionHandler.Clicked += Navigate;
        private void OnDisable() => _interactionHandler.Clicked -= Navigate;

        private void Navigate()
        {
            CanvasManager.Instance.NavigateTo(navigateToPanel);
        }
    }
}