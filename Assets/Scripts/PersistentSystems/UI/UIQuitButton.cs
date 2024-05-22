using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(Button))]
    internal sealed class UIQuitButton : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            
#if UNITY_WEBGL
            Destroy(gameObject);
            return;
#endif
        }
        
        private void OnEnable() => _button.onClick.AddListener(ExitGame);
        private void OnDisable() => _button.onClick.RemoveListener(ExitGame);
        
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