using System;
using System.Collections.Generic;
using DuckJam.Entities;
using DuckJam.Modules.Projectiles;
using DuckJam.PersistentSystems;
using DuckJam.SharedConfiguration;
using DuckJam.Utilities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DuckJam.Modules
{
    internal sealed class EnemiesController : MonoBehaviour
    {
        [SerializeField] private EnemiesConfig enemyConfig;
        
        private readonly HashSet<EnemyController> _enemyBuffer = new();
        
        private EnemiesModel _enemiesModel;
        private TimeScaleConfig _timeScaleConfig;
        private MapModel _mapModel;
        private PlayerModel _playerModel;
        private ProjectileManager _projectileManager;
        
        // bit hacky - but whatever
        private IDamageable _playerDamageable;
        
        private void Awake()
        {
            _enemiesModel = new EnemiesModel(enemyConfig);
            GameModel.Register(_enemiesModel);
        }

        private void Start()
        {
            _timeScaleConfig = GameModel.Get<TimeScaleConfig>();
            _mapModel = GameModel.Get<MapModel>();
            _playerModel = GameModel.Get<PlayerModel>();
            _projectileManager = GameModel.Get<ProjectileManager>();

            _playerDamageable = _playerModel.Transform.GetComponent<IDamageable>();
        }

        private void LateUpdate()
        {
            ClearDeadEnemies();
            
            var deltaTime = Time.deltaTime;
            
            var timeScaleDeltaAbs = _timeScaleConfig.TimeScaleChangeSpeed * deltaTime;
            var playerPosition = _playerModel.Transform.position.XY();
            foreach (var enemy in _enemiesModel)
            {
                SetTimeScale(enemy, timeScaleDeltaAbs);
                ProgressAttackCooldownCountdown(enemy, deltaTime);
                AnimateVisuals(enemy, deltaTime);
                HandleAttack(enemy, playerPosition);
                _enemiesModel.UpdateGridCell(enemy);
            }
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            var playerPosition = _playerModel.Transform.position.XY();
            foreach (var enemy in _enemiesModel)
            {
                HandleMovement(enemy, playerPosition, deltaTime);
            }
        }

        private void OnDestroy()
        {
            _enemiesModel.Dispose();
        }

        private void ClearDeadEnemies()
        {
            foreach (var enemy in _enemiesModel)
            {
                if(!enemy.IsDead) continue;
                _enemyBuffer.Add(enemy);
            }

            foreach (var enemy in _enemyBuffer)
            {
                _enemiesModel.DestroyEnemy(enemy);
            }
            
            _enemyBuffer.Clear();
        }
        
        private void SetTimeScale(EnemyController enemy, float timeScaleDeltaAbs)
        {
            var timeScaleDelta = _mapModel.GetTimeScaleSignAtPosition(enemy.Position2D) * timeScaleDeltaAbs;
            enemy.TimeScale = Mathf.Clamp
            (
                enemy.TimeScale + timeScaleDelta, 
                _timeScaleConfig.MinTimeScale, 
                _timeScaleConfig.MaxTimeScale
            );
            
            enemy.BackgroundColor = _timeScaleConfig.GetTimeScaleColor(enemy.TimeScale);
            enemy.Color = _timeScaleConfig.GetTimeScaleColor(enemy.TimeScale);
        }

        private void ProgressAttackCooldownCountdown(EnemyController enemyController, float deltaTime)
        {
            if(enemyController.AttackCooldownCountdown <= 0f) return;
            
            var delta = deltaTime * enemyController.TimeScale;
            
            enemyController.AttackCooldownCountdown = Mathf.Max(enemyController.AttackCooldownCountdown - delta, 0f);
        }
        
        private void AnimateVisuals(EnemyController enemyController, float deltaTime)
        {
            if (!enemyController.Moving)
            {
                enemyController.SwayTime = 0f;
                enemyController.VisualsTransform.rotation = Quaternion.identity;
                return;
            }
            
            var swaySpeed = enemyConfig.SwaySpeed * enemyController.TimeScale;
            enemyController.SwayTime += swaySpeed * deltaTime;
            
            var swayAmount = Mathf.Sin(enemyController.SwayTime) * enemyConfig.SwayAmount;
            
            enemyController.VisualsTransform.rotation = Quaternion.Euler(0f, 0f, swayAmount);
        }

        private void HandleAttack(EnemyController enemy, Vector2 targetPosition)
        {
            if(_playerDamageable.Health <= 0) return;
            if(enemy.AttackCooldownCountdown > 0f) return;
            
            var offset = targetPosition - enemy.Position2D;
            var distance = offset.magnitude;
            
            if(distance < enemy.Attack.MinDistance || distance > enemy.Attack.MaxDistance) return;


            
            var attackDirection = offset.normalized;
            
            if (enemy.Attack.IsRanged)
            {
                _projectileManager.ShootBullet
                (
                    enemy.Position2D, 
                    targetPosition, 
                    LayerUtils.PlayerLayer, 
                    enemy.Attack.Damage, 
                    enemy.Attack.Speed, 
                    enemy.TimeScale,
                    enemy.Attack.Color
                );
            }
            else
            {
                _playerDamageable.TakeDamage(enemy.Attack.Damage, attackDirection);
            }
            
            AudioFXManager.Instance.PlayClip(enemy.Attack.AttackSound, enemy.TimeScale);
            
            var frames = SpriteAnimationManager.Instance.ImpactFXSpriteArr[enemy.Attack.AttackFXIndex]
                .GetFramesForColor(enemy.Attack.Color);
            var position = enemy.Position2D + attackDirection * enemy.Attack.AttackFXOffsetPositionDistance;
            var spriteAnimator = SpriteAnimationManager.Instance.CreateAnimation
            (
                frames, 
                position, 
                enemy.Attack.AttackFXScale,
                enemy.TimeScale
            );
            var angle = Mathf.Atan2(-attackDirection.y, -attackDirection.x);
            spriteAnimator.transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
            
            enemy.AttackCooldownCountdown = enemy.Attack.Cooldown;
        }
        
        private void HandleMovement(EnemyController enemy, Vector2 targetPosition, float deltaTime)
        {
            var offset = targetPosition - enemy.Position2D;
            
            
            // flip sprite to face target
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            enemy.transform.localScale = Mathf.Abs(angle) < 90f ? Vector3.one : new Vector3(-1f, 1f, 1f);
            
            var distance = offset.magnitude;
            
            // if within min/max range don't move
            if (distance > enemy.Attack.MinDistance && distance < enemy.Attack.MaxDistance)
            {
                enemy.Moving = false;
                return;
            }
            
            enemy.Moving = true;
            
            if 
            (
                !enemy.NavMeshAgent.hasPath || 
                (enemy.NavMeshAgent.destination.XY() - targetPosition).magnitude > enemy.Attack.MaxDistance
            )
            {
                enemy.NavMeshAgent.SetDestination(targetPosition.XY0());
            }
            
            enemy.NavMeshAgent.nextPosition = enemy.transform.position;
            
            var deltaDistance = enemy.Speed * enemy.TimeScale * deltaTime;
            var moveDirection = enemy.NavMeshAgent.velocity.XY().normalized;
            var localAvoidanceDirection = _enemiesModel.GetLocalAvoidanceVector(enemy);
            var direction = (moveDirection + localAvoidanceDirection * enemyConfig.LocalAvoidanceWeight).normalized;
            
            enemy.Rigidbody2D.MovePosition(enemy.transform.position.XY() + direction * deltaDistance);
        }

        // private void OnDrawGizmos()
        // {
        //     var cellSize = new Vector2(enemyConfig.GridCellSize, enemyConfig.GridCellSize);
        //     
        //     // draw grid lines
        //
        //     var minX = -20;
        //     var maxX = 20;
        //     var minY = -20;
        //     var maxY = 20;
        //     
        //     Gizmos.color = Color.cyan;
        //     
        //     for (var x = minX; x <= maxX; x++)
        //     {
        //         var start = new Vector2(x, minY) * cellSize;
        //         var end = new Vector2(x, maxY) * cellSize;
        //         Gizmos.DrawLine(start, end);
        //     }
        //     
        //     for (var y = minY; y <= maxY; y++)
        //     {
        //         var start = new Vector2(minX, y) * cellSize;
        //         var end = new Vector2(maxX, y) * cellSize;
        //         Gizmos.DrawLine(start, end);
        //     }
        //     
        //     if(_enemiesModel is null) return;
        //
        //
        //     
        //     
        //     
        //     Handles.BeginGUI();
        //     foreach (var (cell, count) in _enemiesModel.GridCellEnemyCount)
        //     {
        //         GridUtils.CoordinatesToPosition(cell, cellSize, out var position);
        //         position += (float2)(cellSize * 0.5f);
        //         
        //         var p = new Vector3(position.x, position.y, 0f);
        //         
        //         //Gizmos.color = Color.Lerp(Color.white, Color.black, count / 5f);
        //         //Gizmos.DrawCube(p, new Vector3(cellSize.x, cellSize.y, 0f));
        //         
        //         Handles.Label(p, count.ToString());
        //     }
        //     Handles.EndGUI();
        //     
        // }
    }
}