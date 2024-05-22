using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.Modules
{
    [RequireComponent(typeof(Button))]
    internal sealed class UINavigationButtonHandler : MonoBehaviour
    {
        [SerializeField] private UIPanel navigateToPanel;
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            if (navigateToPanel == UIPanel.None)
            {
                Debug.LogError($"No panel to navigate to", this);
            }
        }

        private void OnEnable() => _button.onClick.AddListener(Navigate);
        private void OnDisable() => _button.onClick.RemoveListener(Navigate);

        private void Navigate()
        {
            CanvasManager.Instance.NavigateTo(navigateToPanel);
        }
    }
}