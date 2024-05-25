using UnityEngine;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(UIButtonInteractionHandler))]
    internal sealed class UILoadMainMenuButton : MonoBehaviour
    {
        private UIButtonInteractionHandler _interactionHandler;
        
        private void Awake()
        {
            _interactionHandler = GetComponent<UIButtonInteractionHandler>();
        }

        private void OnEnable() => _interactionHandler.Clicked += LoadMainMenu;
        private void OnDisable() => _interactionHandler.Clicked -= LoadMainMenu;
        
        private static void LoadMainMenu()
        {
            CanvasManager.Instance.LoadMainMenu();
        }
    }
}