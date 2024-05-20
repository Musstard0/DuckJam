using DuckJam.Models;
using UnityEngine;

namespace DuckJam.Controllers
{
    internal sealed class GameController : MonoBehaviour
    {
        private MapModel _mapModel;
        private EnemiesModel _enemiesModel;
        private EnemySpawnConfig _enemySpawnConfig;
        
        private float _nextEnemySpawnTime;
        
        private void Start()
        {
            _mapModel = GameModel.Get<MapModel>();
            _enemiesModel = GameModel.Get<EnemiesModel>();
            _enemySpawnConfig = GameModel.Get<EnemySpawnConfig>();
            
            _nextEnemySpawnTime = Time.time + _enemySpawnConfig.RandomSpawnInterval;
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _mapModel.RotateTimeScaleLine(deltaTime);
            SpawnEnemies();
            UpdateEnemies(deltaTime);
        }

        private void SpawnEnemies()
        {
            var time = Time.time;
            if(time < _nextEnemySpawnTime) return;
            
            var newEnemy = _enemiesModel.SpawnEnemy(Vector3.zero);
            _nextEnemySpawnTime = time + _enemySpawnConfig.RandomSpawnInterval;
        }

        private void UpdateEnemies(float deltaTime)
        {
            // this is temp behavior just to get the enemies moving randomly
            
            var travelDistance = 1f * deltaTime;
            
            foreach (var enemy in _enemiesModel.ActiveEnemies)
            {
                var targetOffset = enemy.TargetPosition - enemy.transform.position;
                var targetDirection = targetOffset.normalized;
                var targetDistance = targetOffset.magnitude;

                if (targetDistance > travelDistance)
                {
                    enemy.transform.position += targetDirection * travelDistance;
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