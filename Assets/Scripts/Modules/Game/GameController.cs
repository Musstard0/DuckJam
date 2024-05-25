using System;
using DuckJam.PersistentSystems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DuckJam.Modules
{
    internal sealed class GameController : MonoBehaviour
    {
        [SerializeField] private EnemySpawnConfig enemySpawnConfig;
        [SerializeField] private Transform[] enemySpawnPointsTop;
        [SerializeField] private Transform[] enemySpawnPointsBottom;
        [SerializeField] private Transform[] enemySpawnPointsLeft;
        [SerializeField] private Transform[] enemySpawnPointsRight;
        
        private MapModel _mapModel;
        private EnemiesModel _enemiesModel;
        private PlayerModel _playerModel;
        
        private float _nextEnemySpawnTime;
        private float _nextMaxEnemiesIncrementTime;
        
        private int _maxEnemies;
        
        private bool _gameOver;
        
        private void Start()
        {
            _mapModel = GameModel.Get<MapModel>();
            _enemiesModel = GameModel.Get<EnemiesModel>();
            _playerModel = GameModel.Get<PlayerModel>();
            
            _nextEnemySpawnTime = Time.time + enemySpawnConfig.RandomSpawnInterval;
            _maxEnemies = enemySpawnConfig.InitialMaxEnemies;
            _nextMaxEnemiesIncrementTime = Time.time + enemySpawnConfig.MaxEnemiesIncrementRate;
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            
            _mapModel.RotateTimeScaleLine(deltaTime);
            
            if(_gameOver) return;
            
            
            
            SpawnEnemies();


            if (Time.time > _nextMaxEnemiesIncrementTime)
            {
                _maxEnemies = Mathf.Min(_maxEnemies + 1, enemySpawnConfig.MaxEnemies);
                _nextMaxEnemiesIncrementTime = Time.time + enemySpawnConfig.MaxEnemiesIncrementRate;
            }
            
            
            if(_playerModel.Health > 0) return;
            EndGame();
        }

        private void SpawnEnemies()
        {
            if(_enemiesModel.Count >= _maxEnemies) return;
            
            var time = Time.time;
            if (time < _nextEnemySpawnTime) return;
            _nextEnemySpawnTime = time + enemySpawnConfig.RandomSpawnInterval;
            
            Vector3 spawnPosition;
            switch (Random.Range(0, 4))
            {
                case 0:
                    spawnPosition = enemySpawnPointsTop[Random.Range(0, enemySpawnPointsTop.Length)].position;
                    break;
                
                case 1:
                    spawnPosition = enemySpawnPointsBottom[Random.Range(0, enemySpawnPointsBottom.Length)].position;
                    break;
                
                case 2:
                    spawnPosition = enemySpawnPointsLeft[Random.Range(0, enemySpawnPointsLeft.Length)].position;
                    break;
                
                case 3:
                    spawnPosition = enemySpawnPointsRight[Random.Range(0, enemySpawnPointsRight.Length)].position;
                    break;
                
                default:
                    throw new Exception();
            }
            
            spawnPosition.z = enemySpawnConfig.SpawnPositionZ;
            _enemiesModel.SpawnEnemy(spawnPosition);

        }

        public void EndGame()
        {
            _gameOver = true;
            CanvasManager.Instance.ShowGameOverMenu(_enemiesModel.DeadEnemyCount);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            
            if (enemySpawnPointsTop != null)
            {
                foreach (var spawnPoint in enemySpawnPointsTop)
                {
                    Gizmos.DrawSphere(spawnPoint.position, 1f);
                }
            }
            
            if (enemySpawnPointsBottom != null)
            {
                foreach (var spawnPoint in enemySpawnPointsBottom)
                {
                    Gizmos.DrawSphere(spawnPoint.position, 1f);
                }
            }
            
            if (enemySpawnPointsLeft != null)
            {
                foreach (var spawnPoint in enemySpawnPointsLeft)
                {
                    Gizmos.DrawSphere(spawnPoint.position, 1f);
                }
            }
            
            if (enemySpawnPointsRight != null)
            {
                foreach (var spawnPoint in enemySpawnPointsRight)
                {
                    Gizmos.DrawSphere(spawnPoint.position, 1f);
                }
            }
        }
    }
}
