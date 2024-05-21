using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuckJam.Entities
{
    internal sealed class EnemiesModel : IReadOnlyList<EnemyModel>
    {
        private readonly EnemyCfg _enemyConfig;
        private readonly List<EnemyModel> _activeEnemies = new();
        
        public int Count => _activeEnemies.Count;
        public EnemyModel this[int index] => _activeEnemies[index];
        
        public EnemiesModel(EnemyCfg config)
        {
            _enemyConfig = config;
        }
        
        public void SpawnMeleeEnemy(Vector3 position)
        {
            var enemy = Object.Instantiate(_enemyConfig.MeleeEnemyPrefab, position, Quaternion.identity);
            
            enemy.Ranged = false;
            enemy.TimeScale = 1f;
            enemy.LastAttackTime = 0f;
            enemy.Health = _enemyConfig.MeleeEnemyMaxHealth;
            
            enemy.Color = Color.white;
            
            _activeEnemies.Add(enemy);
        }
        
        public void SpawnRangedEnemy(Vector3 position)
        {
            var enemy = Object.Instantiate(_enemyConfig.RangedEnemyPrefab, position, Quaternion.identity);
            
            enemy.Ranged = true;
            enemy.TimeScale = 1f;
            enemy.LastAttackTime = 0f;
            enemy.Health = _enemyConfig.RangedEnemyMaxHealth;
            
            enemy.Color = Color.white;
            
            _activeEnemies.Add(enemy);
        }
        
        public void DestroyEnemy(EnemyModel enemy)
        {
            if(enemy == null) return;
            _activeEnemies.Remove(enemy);
            Object.Destroy(enemy.gameObject);
        }
        
        public IEnumerator<EnemyModel> GetEnumerator() => _activeEnemies.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _activeEnemies.GetEnumerator();
    }
}