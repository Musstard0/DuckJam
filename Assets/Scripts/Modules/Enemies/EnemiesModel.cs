using System.Collections;
using System.Collections.Generic;
using DuckJam.Entities;
using UnityEngine;

namespace DuckJam.Modules
{
    internal sealed class EnemiesModel : IReadOnlyList<EnemyController>
    {
        private readonly EnemiesConfig _enemyConfig;
        private readonly List<EnemyController> _activeEnemies = new();
        
        public int Count => _activeEnemies.Count;
        public EnemyController this[int index] => _activeEnemies[index];
        
        public EnemiesModel(EnemiesConfig config)
        {
            _enemyConfig = config;
        }
        
        public void SpawnEnemy(Vector3 position)
        {
            var type = _enemyConfig.EnemyTypes[Random.Range(0, _enemyConfig.EnemyTypes.Count)];
            var enemy = Object.Instantiate(type.Prefab, position, Quaternion.identity);

            enemy.Health = type.MaxHealth;
            enemy.Attack = type.Attack;
            enemy.Speed = type.Speed;
            enemy.TimeScale = 1f;
            enemy.LastAttackTime = 0f;
            enemy.Color = Color.white;
            
            _activeEnemies.Add(enemy);
        }
        
        // public void SpawnRangedEnemy(Vector3 position)
        // {
        //     var enemy = Object.Instantiate(_enemyConfig.RangedEnemyPrefab, position, Quaternion.identity);
        //     
        //     enemy.Ranged = true;
        //     enemy.TimeScale = 1f;
        //     enemy.LastAttackTime = 0f;
        //     enemy.Health = _enemyConfig.RangedEnemyMaxHealth;
        //     
        //     enemy.Color = Color.white;
        //     
        //     _activeEnemies.Add(enemy);
        // }
        
        public void DestroyEnemy(EnemyController enemy)
        {
            if(enemy == null) return;
            _activeEnemies.Remove(enemy);
            Object.Destroy(enemy.gameObject);
        }
        
        public IEnumerator<EnemyController> GetEnumerator() => _activeEnemies.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _activeEnemies.GetEnumerator();
    }
}