using System;
using System.Collections;
using System.Collections.Generic;
using DuckJam.Entities;
using DuckJam.PersistentSystems;
using DuckJam.Utilities;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace DuckJam.Modules
{
    internal sealed class EnemiesModel : IReadOnlyList<EnemyController>, IDisposable
    {
        private readonly EnemiesConfig _enemyConfig;
        private readonly List<EnemyController> _activeEnemies = new();
        private readonly Dictionary<int2, int> _gridCellEnemyCount = new();

        private readonly float2 _gridCellSize;
        
        private float _lastDeathSoundTime;
        
        public int Count => _activeEnemies.Count;
        public EnemyController this[int index] => _activeEnemies[index];
        public int DeadEnemyCount { get; private set; }
        
        public IReadOnlyDictionary<int2, int> GridCellEnemyCount => _gridCellEnemyCount;
        
        public EnemiesModel(EnemiesConfig config)
        {
            _enemyConfig = config;
            _gridCellSize = new int2(config.GridCellSize);
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
            enemy.Color = _enemyConfig.FillSpriteColor;
            
            GridUtils.PositionToCoordinates(enemy.Position2D, _gridCellSize, out var coordinates);
            enemy.GridPosition = coordinates;
            AddEnemyToGridCell(coordinates);
            
            _activeEnemies.Add(enemy);
        }
        
        public void DestroyEnemy(EnemyController enemy)
        {
            if(enemy == null) return;
            
            RemoveEnemyFromGridCell(enemy.GridPosition);
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
            
            AudioFXManager.Instance.PlayClip(_enemyConfig.DeathSound, 0.6f);
            _lastDeathSoundTime = Time.time;
        }

        public void UpdateGridCell(EnemyController enemy)
        {
            GridUtils.PositionToCoordinates(enemy.Position2D,_gridCellSize, out var coordinates);

            if (coordinates.Equals(enemy.GridPosition)) return;
            
            RemoveEnemyFromGridCell(enemy.GridPosition);
            AddEnemyToGridCell(coordinates);
            
            enemy.GridPosition = coordinates;
        }

        private static readonly int2 Left = new(-1, 0);
        private static readonly int2 Right = new(1, 0);
        private static readonly int2 Up = new(0, 1);
        private static readonly int2 Down = new(0, -1);
        private static readonly int2 UpLeft = new(-1, 1);
        private static readonly int2 UpRight = new(1, 1);
        private static readonly int2 DownLeft = new(-1, -1);
        private static readonly int2 DownRight = new(1, -1);
        
        private static readonly Vector2 UpLeftDirection = new Vector2(-1f, 1f).normalized;
        private static readonly Vector2 UpRightDirection = new Vector2(1f, 1f).normalized;
        private static readonly Vector2 DownLeftDirection = new Vector2(-1f, -1f).normalized;
        private static readonly Vector2 DownRightDirection = new Vector2(1f, -1f).normalized;
        
        
        public Vector2 GetLocalAvoidanceVector(EnemyController enemy)
        {
            var currentPosition = enemy.GridPosition;
            var result = Vector2.zero;
            
            if(_gridCellEnemyCount.TryGetValue(currentPosition + Left, out var count))
            {
                result += Vector2.right * count;
            }
            
            if(_gridCellEnemyCount.TryGetValue(currentPosition + Right, out count))
            {
                result += Vector2.left * count;
            }
            
            if(_gridCellEnemyCount.TryGetValue(currentPosition + Up, out count))
            {
                result += Vector2.down * count;
            }
            
            if(_gridCellEnemyCount.TryGetValue(currentPosition + Down, out count))
            {
                result += Vector2.up * count;
            }
            
            if(_gridCellEnemyCount.TryGetValue(currentPosition + UpLeft, out count))
            {
                result += DownRightDirection * count;
            }
            
            if(_gridCellEnemyCount.TryGetValue(currentPosition + UpRight, out count))
            {
                result += DownLeftDirection * count;
            }
            
            if(_gridCellEnemyCount.TryGetValue(currentPosition + DownLeft, out count))
            {
                result += UpRightDirection * count;
            }
            
            if(_gridCellEnemyCount.TryGetValue(currentPosition + DownRight, out count))
            {
                result += UpLeftDirection * count;
            }
            
            return result.normalized;
        }
        

        private void RemoveEnemyFromGridCell(int2 cell)
        {
            if (!_gridCellEnemyCount.TryGetValue(cell, out var count)) return;
            
            count--;

            if (count <= 0) _gridCellEnemyCount.Remove(cell);
            else _gridCellEnemyCount[cell] = count;
        }

        private void AddEnemyToGridCell(int2 cell)
        {
            _gridCellEnemyCount.TryGetValue(cell, out var count);
            _gridCellEnemyCount[cell] = count + 1;
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