using UnityEngine;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(UIButtonInteractionHandler))]
    internal sealed class UILoadGameButton : MonoBehaviour
    {
        private UIButtonInteractionHandler _interactionHandler;
        
        private void Awake()
        {
            _interactionHandler = GetComponent<UIButtonInteractionHandler>();
        }

        private void OnEnable() => _interactionHandler.Clicked += LoadGame;
        private void OnDisable() => _interactionHandler.Clicked -= LoadGame;
        
        private void LoadGame()
        {
            CanvasManager.Instance.LoadGame();
        }
    }
}