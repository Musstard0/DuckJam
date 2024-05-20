using System.Collections.Generic;
using DuckJam.Entities;
using UnityEngine;

namespace DuckJam.Models
{
    internal sealed class EnemiesModel
    {
        private readonly List<EnemyController> _activeEnemies = new();
        private readonly List<EnemyType> _enemyTypes = new();
        
        public IReadOnlyList<EnemyController> ActiveEnemies => _activeEnemies;
        
        public void AddEnemyType(EnemyType enemyType)
        {
            if(_enemyTypes.Contains(enemyType)) return;
            _enemyTypes.Add(enemyType);
        }
        
        public EnemyController SpawnEnemy(Vector3 position)
        {
            var enemyType = _enemyTypes[Random.Range(0, _enemyTypes.Count)];
            var enemy = Object.Instantiate(enemyType.Prefab, position, Quaternion.identity);
            
            enemy.Speed = enemyType.Speed;
            enemy.Health = enemyType.Health;
            
            _activeEnemies.Add(enemy);
            return enemy;
        }
        
        public void DestroyEnemy(EnemyController enemy)
        {
            if(enemy == null) return;
            _activeEnemies.Remove(enemy);
            Object.Destroy(enemy.gameObject);
        }
    }
}