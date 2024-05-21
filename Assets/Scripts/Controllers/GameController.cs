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
            _mapModel.RotateTimeScaleLine(Time.deltaTime);
            SpawnEnemies();
        }
        
        private void SpawnEnemies()
        {
            if(_enemiesModel.Count >= enemySpawnConfig.MaxEnemies) return;
            
            var time = Time.time;
            if(time < _nextEnemySpawnTime) return;
            
            var spawnPosition = _nextSpawnAtLineStart ? _mapModel.TimeScaleLineStart : _mapModel.TimeScaleLineEnd;
            var directionToCenter = (_mapModel.CenterPosition - spawnPosition).normalized;
            spawnPosition += directionToCenter * enemySpawnConfig.MapEdgeSpawnBuffer;
            
            _enemiesModel.SpawnMeleeEnemy(spawnPosition);
            _nextEnemySpawnTime = time + enemySpawnConfig.RandomSpawnInterval;
        }
    }
}
