using System;
using System.Collections;
using System.Collections.Generic;
using DuckJam.Entities;
using DuckJam.PersistentSystems;
using DuckJam.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace DuckJam.Modules
{
    internal sealed class EnemiesModel : IReadOnlyList<EnemyController>, IDisposable
    {
        private readonly EnemiesConfig _enemyConfig;
        private readonly List<EnemyController> _activeEnemies = new();
        
        private float _lastDeathSoundTime;
        
        public int Count => _activeEnemies.Count;
        public EnemyController this[int index] => _activeEnemies[index];
        public int DeadEnemyCount { get; private set; }
        
        public EnemiesModel(EnemiesConfig config)
        {
            _enemyConfig = config;
        }
        
        public void SpawnEnemy(Vector3 position)
        {
            var typeIndex = Random.Range(0, _enemyConfig.EnemyTypes.Count);
            SpawnEnemy(position, typeIndex);
        }
        
        public void SpawnEnemy(Vector3 position, int enemyTypeIndex)
        {
            var frames = SpriteAnimationManager.Instance.ImpactFXSpriteArr[_enemyConfig.SpawnSpriteEffectIndex]
                .GetFramesForColor(ImpactFXColor.White);
            SpriteAnimationManager.Instance.CreateAnimation
            (
                frames, 
                position.XY(), 
                _enemyConfig.SpawnEffectSizeScale, 
                1f
            );
            
            
            var type = _enemyConfig.EnemyTypes[enemyTypeIndex];
            var enemy = Object.Instantiate(type.Prefab, position, Quaternion.identity);

            enemy.Health = type.MaxHealth;
            enemy.Attack = type.Attack;
            enemy.Speed = type.Speed;
            enemy.TimeScale = 1f;
            enemy.AttackCooldownCountdown = 0f;
            enemy.Color = Color.white;
            
            _activeEnemies.Add(enemy);
        }
        
        public void DestroyEnemy(EnemyController enemy)
        {
            if(enemy == null) return;
            _activeEnemies.Remove(enemy);
            Object.Destroy(enemy.gameObject);
            DeadEnemyCount++;
            
            
            
            var frames = SpriteAnimationManager.Instance.ImpactFXSpriteArr[_enemyConfig.DeathSpriteEffectIndex]
                .GetFramesForColor(enemy.Attack.Color);
            SpriteAnimationManager.Instance.CreateAnimation
            (
                frames, 
                enemy.Position2D, 
                _enemyConfig.DeathEffectSizeScale, 
                enemy.TimeScale
            );
            
            
            if(Time.time - _lastDeathSoundTime < _enemyConfig.DeathSoundMinInterval) return;
            
            AudioFXManager.Instance.PlayClip(_enemyConfig.DeathSound, enemy.TimeScale, 0.6f);
            _lastDeathSoundTime = Time.time;
        }
        
        public IEnumerator<EnemyController> GetEnumerator() => _activeEnemies.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _activeEnemies.GetEnumerator();
        public void Dispose()
        {
            foreach (var enemy in _activeEnemies)
            {
                if(enemy == null) continue;
                Object.Destroy(enemy.gameObject);
            }
            
            _activeEnemies.Clear();
        }
    }
}