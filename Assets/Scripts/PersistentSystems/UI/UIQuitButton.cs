using UnityEngine;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(UIButtonInteractionHandler))]
    internal sealed class UIQuitButton : MonoBehaviour
    {
        private UIButtonInteractionHandler _interactionHandler;
        
        private void Awake()
        {
            _interactionHandler = GetComponent<UIButtonInteractionHandler>();
            
#if UNITY_WEBGL
            Destroy(gameObject);
            return;
#endif
        }

        private void OnEnable() => _interactionHandler.Clicked += ExitGame;
        private void OnDisable() => _interactionHandler.Clicked -= ExitGame;
        
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}