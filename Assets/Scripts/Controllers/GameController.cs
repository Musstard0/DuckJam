using DuckJam.Configuration;
using DuckJam.Entities;
using DuckJam.Models;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam.Controllers
{
    internal sealed class GameController : MonoBehaviour
    {
        [SerializeField] private EnemySpawnConfig enemySpawnConfig;
        
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
            
            var spawnPosition2D = _nextSpawnAtLineStart ? _mapModel.TimeScaleLineStartPosition2D : _mapModel.TimeScaleLineEndPosition2D;
            var directionToCenter = (_mapModel.CenterPosition2D - spawnPosition2D).normalized;
            spawnPosition2D += directionToCenter * enemySpawnConfig.MapEdgeSpawnBuffer;
            
            var spawnPosition = spawnPosition2D.XY0(enemySpawnConfig.SpawnPositionZ);
            
            _enemiesModel.SpawnMeleeEnemy(spawnPosition);
            _nextEnemySpawnTime = time + enemySpawnConfig.RandomSpawnInterval;
            _nextSpawnAtLineStart = !_nextSpawnAtLineStart;
        }
    }
}
