using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.Modules
{
    [RequireComponent(typeof(Button))]
    internal sealed class UIBackButtonNavigationHandler : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable() => _button.onClick.AddListener(Navigate);
        private void OnDisable() => _button.onClick.RemoveListener(Navigate);

        private void Navigate()
        {
            CanvasManager.Instance.NavigateToParent();
        }
    }
}