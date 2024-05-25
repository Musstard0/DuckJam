using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(UIButtonInteractionHandler))]
    internal sealed class UIBackButtonNavigationHandler : MonoBehaviour
    {
        private UIButtonInteractionHandler _interactionHandler;
        
        private void Awake()
        {
            _interactionHandler = GetComponent<UIButtonInteractionHandler>();
        }

        private void OnEnable() => _interactionHandler.Clicked += Navigate;
        private void OnDisable() => _interactionHandler.Clicked -= Navigate;

        private void Navigate()
        {
            CanvasManager.Instance.NavigateToParent();
        }
    }
}