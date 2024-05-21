using System.Collections.Generic;
using DuckJam.Configuration;
using DuckJam.Models;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam.Entities
{
    internal sealed class EnemiesController : MonoBehaviour
    {
        [SerializeField] private EnemyCfg enemyConfig;
        
        private readonly HashSet<EnemyModel> _enemyBuffer = new();
        
        private EnemiesModel _enemiesModel;
        private TimeScaleConfig _timeScaleConfig;
        private MapModel _mapModel;
        private PlayerModel _playerModel;
        
        private void Awake()
        {
            _enemiesModel = new EnemiesModel(enemyConfig);
            GameModel.Register(_enemiesModel);
        }

        private void Start()
        {
            _timeScaleConfig = GameModel.Get<TimeScaleConfig>();
            _mapModel = GameModel.Get<MapModel>();
            _playerModel = GameModel.Get<PlayerModel>();
        }

        private void LateUpdate()
        {
            ClearDeadEnemies();
            UpdateEnemies(Time.deltaTime);
        }
        
        private void ClearDeadEnemies()
        {
            foreach (var enemy in _enemiesModel)
            {
                if(!enemy.IsDead) continue;
                _enemyBuffer.Add(enemy);
            }

            foreach (var enemy in _enemyBuffer)
            {
                _enemiesModel.DestroyEnemy(enemy);
            }
            
            _enemyBuffer.Clear();
        }
        
        private void UpdateEnemies(float deltaTime)
        {
            var timeScaleDeltaAbs = _timeScaleConfig.TimeScaleChangeSpeed * deltaTime;
            var playerPosition = _playerModel.Transform.position.XY();
            
            foreach (var enemy in _enemiesModel)
            {
                SetTimeScale(enemy, timeScaleDeltaAbs);
                SetTimeScaleColor(enemy);
                HandleMovement(enemy, playerPosition, deltaTime);
            }
        }
        
        private void SetTimeScale(EnemyModel enemy, float timeScaleDeltaAbs)
        {
            var timeScaleDelta = _mapModel.GetTimeScaleSignAtPosition(enemy.transform.position) * timeScaleDeltaAbs;
            enemy.TimeScale = Mathf.Clamp
            (
                enemy.TimeScale + timeScaleDelta, 
                _timeScaleConfig.MinTimeScale, 
                _timeScaleConfig.MaxTimeScale
            );
        }

        private void SetTimeScaleColor(EnemyModel enemy)
        {
            var color = Color.white;
            if (enemy.TimeScale > 1f)
            {
                color = Color.Lerp(Color.white, Color.red, (enemy.TimeScale - 1f) / (_timeScaleConfig.MaxTimeScale - 1f));
            }
            else if(enemy.TimeScale < 1f)
            {
                color = Color.Lerp(Color.white, Color.blue, (1f - enemy.TimeScale) / (1f - _timeScaleConfig.MinTimeScale));
            }
            enemy.Color = color;
        }
        
        private void HandleMovement(EnemyModel enemy, Vector2 targetPosition, float deltaTime)
        {
            var offset = targetPosition - enemy.Position2D;
            var distance = offset.magnitude;
            var distanceRangeGoal = enemy.Ranged ? enemyConfig.RangedEnemyTargetRange : enemyConfig.MeleeEnemyTargetRange;
            
            if(distance > distanceRangeGoal.Min && distance < distanceRangeGoal.Max) return;
            
            var direction = distance > distanceRangeGoal.Max ? offset.normalized : -offset.normalized;
            var baseSpeed = enemy.Ranged ? enemyConfig.RangedEnemySpeed : enemyConfig.MeleeEnemySpeed;
            var deltaDistance = baseSpeed * enemy.TimeScale * deltaTime;
            
            enemy.transform.position += direction.XY0() * deltaDistance;
        }
    }
}