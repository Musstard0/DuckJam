using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.Modules
{
    [RequireComponent(typeof(Button))]
    internal sealed class UILoadMainMenuButton : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        
        private void OnEnable() => _button.onClick.AddListener(LoadMainMenu);
        private void OnDisable() => _button.onClick.RemoveListener(LoadMainMenu);
        
        private static void LoadMainMenu()
        {
            CanvasManager.Instance.LoadMainMenu();
        }
    }
}