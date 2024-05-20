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
            _mapModel.RotateTimeScaleLine(Time.deltaTime);
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            var time = Time.time;
            if(time < _nextEnemySpawnTime) return;
            
            _enemiesModel.SpawnEnemy(Vector3.zero);
            _nextEnemySpawnTime = time + _enemySpawnConfig.RandomSpawnInterval;
        }
    }
}
