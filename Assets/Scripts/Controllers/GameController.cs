using System.Collections.Generic;
using DuckJam.Configuration;
using DuckJam.Entities;
using DuckJam.Models;
using UnityEngine;

namespace DuckJam.Controllers
{
    internal sealed class GameController : MonoBehaviour
    {
        [SerializeField] private EnemySpawnConfig enemySpawnConfig;
        [SerializeField] private TimeScaleConfig timeScaleConfig;
        
        private MapModel _mapModel;
        private EnemiesModel _enemiesModel;
        
        private readonly HashSet<EnemyController> _deadEnemyBuffer = new();
        private float _nextEnemySpawnTime;
        private bool _nextSpawnAtLineStart;
        
        private void Start()
        {
            _mapModel = GameModel.Get<MapModel>();
            _enemiesModel = GameModel.Get<EnemiesModel>();
            
            _nextEnemySpawnTime = Time.time + enemySpawnConfig.RandomSpawnInterval;
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _mapModel.RotateTimeScaleLine(deltaTime);
            
            ClearDeadEnemies();
            SpawnEnemies();
            UpdateEnemies(deltaTime);
        }
        
        private void ClearDeadEnemies()
        {
            foreach (var enemy in _enemiesModel.ActiveEnemies)
            {
                if (enemy.Health > 0) continue;
                _deadEnemyBuffer.Add(enemy);
            }

            foreach (var enemy in _deadEnemyBuffer)
            {
                _enemiesModel.DestroyEnemy(enemy);
            }
        }
        
        private void SpawnEnemies()
        {
            if(_enemiesModel.ActiveEnemies.Count >= enemySpawnConfig.MaxEnemies) return;
            
            var time = Time.time;
            if(time < _nextEnemySpawnTime) return;
            
            var spawnPosition = _nextSpawnAtLineStart ? _mapModel.TimeScaleLineStart : _mapModel.TimeScaleLineEnd;
            var directionToCenter = (_mapModel.CenterPosition - spawnPosition).normalized;
            spawnPosition += directionToCenter * enemySpawnConfig.MapEdgeSpawnBuffer;
            
            var newEnemy = _enemiesModel.SpawnEnemy(spawnPosition);
            _nextEnemySpawnTime = time + enemySpawnConfig.RandomSpawnInterval;
            _nextSpawnAtLineStart = !_nextSpawnAtLineStart;
        }

        private void UpdateEnemies(float deltaTime)
        {
            // this is temp behavior just to get the enemies moving randomly
            
            // their color changes based on their position in the map
            // (blue for slow time scale, red for fast time scale)
            
            var timeScaleDeltaAbs = timeScaleConfig.TimeScaleChangeSpeed * deltaTime;
            
            foreach (var enemy in _enemiesModel.ActiveEnemies)
            {
                // set time scale
                var timeScaleSignAtPosition = _mapModel.GetTimeScaleSignAtPosition(enemy.transform.position);
                enemy.CurrentTimeScale = Mathf.Clamp
                (
                    enemy.CurrentTimeScale + timeScaleSignAtPosition * timeScaleDeltaAbs, 
                    timeScaleConfig.MinTimeScale, 
                    timeScaleConfig.MaxTimeScale
                );

                
                // set color to visualize time scale
                var color = Color.white;
                if (enemy.CurrentTimeScale > 1f)
                {
                    color = Color.Lerp(Color.white, Color.red, (enemy.CurrentTimeScale - 1f) / (timeScaleConfig.MaxTimeScale - 1f));
                }
                else if(enemy.CurrentTimeScale < 1f)
                {
                    color = Color.Lerp(Color.white, Color.blue, (1f - enemy.CurrentTimeScale) / (1f - timeScaleConfig.MinTimeScale));
                }
                enemy.Color = color;
                
                
                
                // move towards random target position
                var targetOffset = enemy.TargetPosition - enemy.transform.position;
                var targetDirection = targetOffset.normalized;
                var targetDistance = targetOffset.magnitude;

                var deltaDistance = enemy.ScaledSpeed * deltaTime;
                
                if (targetDistance > deltaDistance)
                {
                    enemy.transform.position += targetDirection * deltaDistance;
                    continue;
                }
                
                enemy.transform.position = enemy.TargetPosition;
                
                // set a new random target position within map bounds
                enemy.TargetPosition = new Vector3
                (
                    Random.Range(_mapModel.SouthWestPosition.x, _mapModel.NorthEastPosition.x),
                    Random.Range(_mapModel.SouthWestPosition.y, _mapModel.NorthEastPosition.y),
                    Random.Range(_mapModel.SouthWestPosition.z, _mapModel.NorthEastPosition.z)
                );
            }
        }
    }
}
