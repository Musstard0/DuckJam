using System;
using DuckJam.Entities;
using DuckJam.Modules;
using DuckJam.Modules.Projectiles;
using DuckJam.SharedConfiguration;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        private PlayerModel playerModel;
        public PlayerCfg playerCfg;

        // Centralized location for creating and managing projectiles
        private ProjectileManager _projectileManager;
        private Rigidbody2D _rigidbody2D;
        private Transform visuals; // Reference to the child object representing visuals
        private Quaternion initialVisualsRotation; // Initial rotation of the visuals

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            visuals = transform.Find("Visuals"); // Assuming the child object is named "Visuals"

            if (visuals != null)
            {
                initialVisualsRotation = visuals.localRotation;
            }

            InitializeModel();
            GameModel.Register(playerModel);
        }

        private void Start()
        {
            _projectileManager = GameModel.Get<ProjectileManager>();
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
                FirePoint = transform.Find("Gun").Find("FirePoint"),  // Assuming there's a child named "FirePoint"
                FireRate = playerCfg.FireRate,
                Inertia = playerCfg.Inertia,
                SwaySpeed = playerCfg.SwaySpeed,
                SwayAmount = playerCfg.SwayAmount
            };
        }

        private void Update()
        {
            if (playerModel.Health <= 0) return;

            var deltaTime = Time.deltaTime;

            // body sprite is symmetrical, so currently not needed
            //HandleFlip();
            HandleNextShotCountDown(deltaTime);
            HandleShooting();
            AnimateVisuals();
            HandleColor();
        }

        private void FixedUpdate()
        {
            if (playerModel.Health <= 0) return;
            HandleMovement(Time.fixedDeltaTime);
        }



        
        
        private void HandleMovement(float deltaTime)
        {
            playerModel.horizontalInput = Input.GetAxis("Horizontal");
            playerModel.verticalInput = Input.GetAxis("Vertical");

            var moveDirection = new Vector2(playerModel.horizontalInput, playerModel.verticalInput).normalized;
            var maxSpeed = playerCfg.timeScaleEffectsMovementSpeed 
                ? playerModel.Speed * playerModel.TimeScale 
                : playerModel.Speed;
            
            var immediateGoalVelocity = moveDirection * maxSpeed;
            var velDot = Vector2.Dot(moveDirection, playerModel.goalVelocity.normalized);
            var accel = playerCfg.acceleration * playerCfg.accelerationFactorFromDot.Evaluate(velDot);
            playerModel.goalVelocity = Vector2.MoveTowards(playerModel.goalVelocity, immediateGoalVelocity, accel * deltaTime);
            
            var neededAcceleration = (playerModel.goalVelocity - _rigidbody2D.velocity) / deltaTime;
            var maxAcceleration = playerCfg.maxAccelerationForce * playerCfg.maxAccelerationForceFactorFromDot.Evaluate(velDot);
            neededAcceleration = Vector2.ClampMagnitude(neededAcceleration, maxAcceleration);
            
            _rigidbody2D.AddForce(neededAcceleration * _rigidbody2D.mass);
        }
        
        // private void HandleMovement(float deltaTime)
        // {
        //     playerModel.horizontalInput = Input.GetAxis("Horizontal");
        //     playerModel.verticalInput = Input.GetAxis("Vertical");
        //
        //     var moveDirection = new Vector2(playerModel.horizontalInput, playerModel.verticalInput).normalized;
        //     var moveDistance = playerModel.Speed * deltaTime;
        //
        //     if (playerCfg.timeScaleEffectsMovementSpeed)
        //     {
        //         moveDistance *= playerModel.TimeScale;
        //     }
        //
        //     var delta = moveDirection * moveDistance;
        //     var newPosition = _rigidbody2D.position + delta;
        //
        //     //_mapModel.ClampMovementToMapBounds(_rigidbody2D.position, ref newPosition);
        //     _rigidbody2D.MovePosition(newPosition);
        // }

        private void HandleFlip()
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            Vector3 direction = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(angle) < 90)
            {
                visuals.localScale = Vector3.one;
            }
            else
            {
                visuals.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane xyPlane = new Plane(Vector3.forward, new Vector3(0, 0, transform.position.z));
            if (xyPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
            return Vector3.zero;
        }

        private void AnimateVisuals()
        {
            //if (visuals != null && (playerModel.horizontalInput != 0 || playerModel.verticalInput != 0))
            if (visuals != null && _rigidbody2D.velocity.sqrMagnitude > 0.01f)
            {
                // swing amount effected by unscaled speed, swing speed effected by time scale
                
                // TODO: Speed must affect it
                float swayAmount = Mathf.Sin(Time.time * playerModel.SwaySpeed) * playerModel.SwayAmount;
                visuals.localRotation = initialVisualsRotation * Quaternion.Euler(0, 0, swayAmount);
            }
            else
            {
                visuals.localRotation = initialVisualsRotation;
            }
        }

        private void HandleColor()
        {
            // This is unlikely to remain - for now allows to see the effect of time scale on enemies
            var color = Color.white;
            if (playerModel.TimeScale > 1f)
            {
                color = Color.Lerp(Color.white, Color.red, (playerModel.TimeScale - 1f) / (GameModel.Get<TimeScaleConfig>().MaxTimeScale - 1f));
            }
            else if (playerModel.TimeScale < 1f)
            {
                color = Color.Lerp(Color.white, Color.blue, (1f - playerModel.TimeScale) / (1f - GameModel.Get<TimeScaleConfig>().MinTimeScale));
            }
            visuals.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
        }

        private void HandleNextShotCountDown(float deltaTime)
        {
            if (playerModel.NextShotCountDown <= 0f) return;
            var countDownDelta = deltaTime;

            if (playerCfg.timeScaleEffectsShootingRate)
            {
                countDownDelta *= playerModel.TimeScale;
            }

            playerModel.NextShotCountDown = Mathf.Max(playerModel.NextShotCountDown - countDownDelta, 0f);
        }

        private void HandleShooting()
        {
            if (playerModel.NextShotCountDown > 0f) return;
            if (!Input.GetButton("Fire1")) return;

            Shoot();
            playerModel.NextShotCountDown = playerModel.FireRate;
        }

        private void Shoot()
        {
            _projectileManager.ShootBullet
            (
                playerModel.FirePoint.position.XY(),
                GetMouseWorldPosition().XY(),
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
            // Debug.Log("Player has died");
            // Destroy(gameObject);
        }
    }
}
