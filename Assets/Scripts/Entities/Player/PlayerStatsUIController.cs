using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.Entities.Player
{
    internal sealed class PlayerStatsUIController : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Image healthFill;
        
        [Header("Time Scale")]
        [SerializeField] private TMP_Text timeScaleText;
        
        private PlayerModel _playerModel;
        
        private void Start()
        {
            _playerModel = GameModel.Get<PlayerModel>();
            
            UpdateHealth();
        }

        private void LateUpdate()
        {
            UpdateHealth();
            UpdateTimeScale();
        }

        private void UpdateHealth()
        {
            var health = Mathf.Max(_playerModel.Health, 0f);
            healthText.text = $"{health} / {_playerModel.MaxHealth}";
            healthFill.fillAmount = health / _playerModel.MaxHealth;
        }

        private void UpdateTimeScale()
        {
            var timeScale = Mathf.Round(_playerModel.TimeScale * 10f) / 10f;
            timeScaleText.text = $"{timeScale}X";
        }
    }
}
