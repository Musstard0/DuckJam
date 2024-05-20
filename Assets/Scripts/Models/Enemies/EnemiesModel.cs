using System.Collections.Generic;
using DuckJam.Entities;
using UnityEngine;

namespace DuckJam.Models
{
    internal sealed class EnemiesModel
    {
        private readonly List<Enemy> _activeEnemies = new();
        private readonly List<EnemyType> _enemyTypes = new();
        
        public void AddEnemyType(EnemyType enemyType)
        {
            if(_enemyTypes.Contains(enemyType)) return;
            _enemyTypes.Add(enemyType);
        }
        
        public void SpawnEnemy(Vector3 position)
        {
            var enemyType = _enemyTypes[Random.Range(0, _enemyTypes.Count)];
            var enemy = Object.Instantiate(enemyType.Prefab, position, Quaternion.identity);
            _activeEnemies.Add(enemy);
        }
    }
}