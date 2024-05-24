using System.Collections.Generic;
using DuckJam.Entities;
using DuckJam.Modules.Projectiles;
using DuckJam.PersistentSystems;
using DuckJam.SharedConfiguration;
using DuckJam.Utilities;
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
            
            // this is unlikely to remain - for now allows to see the effect of time scale on enemies
            var color = Color.white;
            if (enemy.TimeScale > 1f)
            {
                color = Color.Lerp(Color.white, Color.red, (enemy.TimeScale - 1f) / (_timeScaleConfig.MaxTimeScale - 1f));
            }
            else if(enemy.TimeScale < 1f)
            {
                color = Color.Lerp(Color.white, Color.blue, (1f - enemy.TimeScale) / (1f - _timeScaleConfig.MinTimeScale));
            }
            enemy.Color = color;
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
            
            var frames = SpriteAnimationManager.Instance.ImpactFXSpriteArr[enemy.Attack.AttackFXIndex]
                .GetFramesForColor(enemy.Attack.Color);
            var position = enemy.Position2D + attackDirection * enemy.Attack.AttackFXOffsetPositionDistance;
            var spriteAnimator = SpriteAnimationManager.Instance.CreateImpactAnimationEffect
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
        
        private static void HandleMovement(EnemyController enemy, Vector2 targetPosition, float deltaTime)
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

            var moveDirection = enemy.NavMeshAgent.velocity.XY().normalized;
            
            enemy.NavMeshAgent.nextPosition = enemy.transform.position;
            
            // move towards/away from target
            //var moveDirection = distance > enemy.Attack.MaxDistance ? offset.normalized : -offset.normalized;
            var deltaDistance = enemy.Speed * enemy.TimeScale * deltaTime;
            
            enemy.Rigidbody2D.MovePosition(enemy.transform.position + moveDirection.XY0() * deltaDistance);
        }
    }
}