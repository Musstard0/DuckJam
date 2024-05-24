using DuckJam.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.Entities.Player
{
    internal sealed class PlayerStatsUIController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Health")]
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Image healthFill;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Image healthRegenDelayFill;
        
        [Header("Time Scale")]
        [SerializeField] private TMP_Text timeScaleText;
        
        private PlayerModel _playerModel;
        private EnemiesModel _enemiesModel;
        
        private void Start()
        {
            _playerModel = GameModel.Get<PlayerModel>();
            _enemiesModel = GameModel.Get<EnemiesModel>();
            
            UpdateHealth();
        }

        private void LateUpdate()
        {
            if (_playerModel.Health <= 0)
            {
                canvasGroup.alpha = 0f;
                return;
            }
            
            
            UpdateHealth();
            UpdateTimeScale();
            UpdateScore();
        }

        private void UpdateHealth()
        {
            var health = Mathf.Max(_playerModel.Health, 0f);
            healthText.text = $"{(int)health} / {_playerModel.MaxHealth}";
            healthFill.fillAmount = health / _playerModel.MaxHealth;
            
            var regenDelayProgress = 1f - Mathf.Clamp01(_playerModel.HealthRegenerationCountdown / _playerModel.HealthRegenerationDelay);
            healthRegenDelayFill.fillAmount = regenDelayProgress;
        }

        private void UpdateTimeScale()
        {
            var timeScale = Mathf.Round(_playerModel.TimeScale * 10f) / 10f;
            timeScaleText.text = $"{timeScale}x";
        }

        private void UpdateScore()
        {
            var score = _enemiesModel.DeadEnemyCount;
            scoreText.text = score.ToString();
        }
    }
}
