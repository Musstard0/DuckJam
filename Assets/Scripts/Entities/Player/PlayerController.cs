using System;
using DuckJam.Entities;
using DuckJam.Modules;
using DuckJam.Modules.Projectiles;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        private PlayerModel playerModel;
        public PlayerCfg playerCfg;



        // this is centralized location for creating and managing projectiles.
        // kind of pointless right now, but will allow for object pooling if needed - and also allow for changing the time scale of bullets if we want to
        private ProjectileManager _projectileManager;
        private Rigidbody2D _rigidbody2D;
        private MapModel _mapModel;
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            InitializeModel();
            GameModel.Register(playerModel);
        }

        private void Start()
        {
            _projectileManager = GameModel.Get<ProjectileManager>();
            _mapModel = GameModel.Get<MapModel>();
        }

        private void InitializeModel()
        {
            playerModel = new PlayerModel
            {
                Transform = transform,
                Health = playerCfg.Health,
                MaxHealth = playerCfg.Health,
                Speed = playerCfg.Speed,
                Damage = playerCfg.Damage,
                BulletSpeed = playerCfg.BulletSpeed,
                FirePoint = transform.Find("FirePoint"),  // Assuming there's a child named "FirePoint"
                FireRate = playerCfg.FireRate,
                Inertia = playerCfg.Inertia,
            };
        }

        private void Update()
        {
            if(playerModel.Health <= 0) return;
            
            var deltaTime = Time.deltaTime;
            
            HandleFlip();
            HandleNextShotCountDown(deltaTime);
            HandleShooting();
        }

        private void FixedUpdate()
        {
            if(playerModel.Health <= 0) return;
            HandleMovement(Time.fixedDeltaTime);
        }


        
        private void HandleMovement(float deltaTime)
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");
            
            var moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
            var moveDistance = playerModel.Speed * deltaTime;
            
            if (playerCfg.timeScaleEffectsMovementSpeed)
            {
                moveDistance *= playerModel.TimeScale;
            }
            
            var delta = moveDirection * moveDistance;
            var newPosition = _rigidbody2D.position + delta;
            
            _mapModel.ClampMovementToMapBounds(_rigidbody2D.position, ref newPosition);
            _rigidbody2D.MovePosition(newPosition);
        }

        private void HandleFlip()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle) < 90)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }


        private void HandleNextShotCountDown(float deltaTime)
        {
            if(playerModel.NextShotCountDown <= 0f) return;
            var countDownDelta = deltaTime;

            if (playerCfg.timeScaleEffectsShootingRate)
            {
                countDownDelta *= playerModel.TimeScale;
            }
            
            playerModel.NextShotCountDown = Mathf.Max(playerModel.NextShotCountDown - countDownDelta, 0f);
        }
        
        private void HandleShooting()
        {
            if(playerModel.NextShotCountDown > 0f) return;
            if(!Input.GetButton("Fire1")) return;
            
            Shoot();
            playerModel.NextShotCountDown = playerModel.FireRate;
        }

        private void Shoot()
        {
            _projectileManager.ShootBullet
            (
                playerModel.FirePoint.position.XY(), 
                Camera.main.ScreenToWorldPoint(Input.mousePosition).XY(), 
                LayerUtils.EnemyLayer, 
                playerModel.Damage, 
                playerModel.BulletSpeed, 
                playerModel.TimeScale
            );
        }

        public void TakeDamage(float damage)
        {
            playerModel.Health -= damage;
            if (playerModel.Health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            
            
            // Handle player death
            //Debug.Log("Player has died");
            // Destroy(gameObject);
        }
    }
}
