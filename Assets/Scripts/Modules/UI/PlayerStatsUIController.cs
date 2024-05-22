using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.Modules
{
    internal sealed class PlayerStatsUIController : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Image healthFill;
        
        private PlayerModel _playerModel;
        
        private void Start()
        {
            _playerModel = GameModel.Get<PlayerModel>();
            
            UpdateHealth();
        }

        private void LateUpdate()
        {
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            var health = Mathf.Max(_playerModel.Health, 0f);
            healthText.text = $"{health} / {_playerModel.MaxHealth}";
            healthFill.fillAmount = health / _playerModel.MaxHealth;
        }
    }
}
