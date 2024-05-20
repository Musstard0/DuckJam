using System.Collections.Generic;
using DuckJam.Entities;
using UnityEngine;

namespace DuckJam.Models
{
    internal sealed class EnemiesModel
    {
        public readonly List<Enemy> ActiveEnemies = new();
        private readonly List<EnemyType> _enemyTypes = new();
        
        public void AddEnemyType(EnemyType enemyType)
        {
            if(_enemyTypes.Contains(enemyType)) return;
            _enemyTypes.Add(enemyType);
        }
        
        public Enemy SpawnEnemy(Vector3 position)
        {
            var enemyType = _enemyTypes[Random.Range(0, _enemyTypes.Count)];
            var enemy = Object.Instantiate(enemyType.Prefab, position, Quaternion.identity);
            ActiveEnemies.Add(enemy);
            return enemy;
        }
    }
}