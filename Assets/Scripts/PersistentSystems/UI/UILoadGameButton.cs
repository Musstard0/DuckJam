using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(Button))]
    internal sealed class UILoadGameButton : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        
        private void OnEnable() => _button.onClick.AddListener(LoadGame);
        private void OnDisable() => _button.onClick.RemoveListener(LoadGame);
        
        private void LoadGame()
        {
            CanvasManager.Instance.LoadGame();
        }
    }
}