using System;
using System.Collections.Generic;
using DuckJam.Entities;
using DuckJam.SharedConfiguration;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam.Modules
{
    internal sealed class EnemiesController : MonoBehaviour
    {
        [SerializeField] private EnemiesConfig enemyConfig;
        
        private readonly HashSet<EnemyController> _enemyBuffer = new();
        
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
            
            var timeScaleDeltaAbs = _timeScaleConfig.TimeScaleChangeSpeed * Time.deltaTime;
            foreach (var enemy in _enemiesModel)
            {
                SetTimeScale(enemy, timeScaleDeltaAbs);
            }
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            var playerPosition = _playerModel.Transform.position.XY();
            foreach (var enemy in _enemiesModel)
            {
                UpdateTransform(enemy, playerPosition, deltaTime);
            }
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
        
        private void SetTimeScale(EnemyController enemy, float timeScaleDeltaAbs)
        {
            var timeScaleDelta = _mapModel.GetTimeScaleSignAtPosition(enemy.transform.position) * timeScaleDeltaAbs;
            enemy.TimeScale = Mathf.Clamp
            (
                enemy.TimeScale + timeScaleDelta, 
                _timeScaleConfig.MinTimeScale, 
                _timeScaleConfig.MaxTimeScale
            );
            
            // this is unlikely to remain - for now allows to see the effect of time scale on enemies
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
        
        private static void UpdateTransform(EnemyController enemy, Vector2 targetPosition, float deltaTime)
        {
            var offset = targetPosition - enemy.Position2D;
            
            // flip sprite to face target
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            enemy.transform.localScale = Mathf.Abs(angle) < 90f ? Vector3.one : new Vector3(-1f, 1f, 1f);
            
            var distance = offset.magnitude;
            
            // if within min/max range don't move
            if(distance > enemy.Attack.MinDistance && distance < enemy.Attack.MaxDistance) return;
            
            // move towards/away from target
            var moveDirection = distance > enemy.Attack.MaxDistance ? offset.normalized : -offset.normalized;
            var deltaDistance = enemy.Speed * enemy.TimeScale * deltaTime;
            
            enemy.Rigidbody2D.MovePosition(enemy.transform.position + moveDirection.XY0() * deltaDistance);
        }
    }
}