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
        private TimeScaleConfig _timeScaleConfig;
        private MapModel _mapModel;
        
        private Rigidbody2D _rigidbody2D;
        private Transform visuals; // Reference to the child object representing visuals
        private Quaternion initialVisualsRotation; // Initial rotation of the visuals
        private float _swayTime; // timescale dynamic so need to keep track of sway time outside of Time.time
        
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
            _timeScaleConfig = GameModel.Get<TimeScaleConfig>();
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
                FirePoint = transform.Find("Gun").Find("FirePoint"),  // Assuming there's a child named "FirePoint"
                FireRate = playerCfg.FireRate,
                Inertia = playerCfg.Inertia,
                SwaySpeed = playerCfg.SwaySpeed,
                SwayAmount = playerCfg.SwayAmount,
                MuzzleFlashAnimator = transform.Find("Gun").Find("FirePoint").Find("MuzzleFlash").GetComponent<Animator>() // lol
            };
            
            _lastTimeScale = playerModel.TimeScale;
        }

        private void Update()
        {
            if (playerModel.Health <= 0) return;

            var deltaTime = Time.deltaTime;
            
            //HandleFlip(); // body sprite is symmetrical, so currently not needed
            SetTimeScale(deltaTime);
            HandleNextShotCountDown(deltaTime);
            HandleShooting();
            AnimateVisuals(deltaTime);
            HandleColor();
        }

        private void FixedUpdate()
        {
            if (playerModel.Health <= 0) return;
            HandleMovement(Time.fixedDeltaTime);
        }
        
        private float _lastTimeScale;
        
        private void HandleMovement(float deltaTime)
        {
            var maxSpeed = playerModel.Speed * playerModel.TimeScale;
            var scaleFactor = playerModel.TimeScale / _lastTimeScale;
            _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity * scaleFactor, maxSpeed);
            playerModel.goalVelocity = Vector2.ClampMagnitude(playerModel.goalVelocity * scaleFactor, maxSpeed);
            _lastTimeScale = playerModel.TimeScale;
            
            
            playerModel.horizontalInput = Input.GetAxis("Horizontal");
            playerModel.verticalInput = Input.GetAxis("Vertical");

            var moveDirection = new Vector2(playerModel.horizontalInput, playerModel.verticalInput).normalized;

            
            var immediateGoalVelocity = moveDirection * maxSpeed;
            var velDot = Vector2.Dot(moveDirection, playerModel.goalVelocity.normalized);
            var accel = playerCfg.acceleration * playerCfg.accelerationFactorFromDot.Evaluate(velDot) * playerModel.TimeScale;
            playerModel.goalVelocity = Vector2.MoveTowards(playerModel.goalVelocity, immediateGoalVelocity, accel * deltaTime);
            
            var neededAcceleration = (playerModel.goalVelocity - _rigidbody2D.velocity) / deltaTime;
            var maxAcceleration = playerCfg.maxAccelerationForce * playerCfg.maxAccelerationForceFactorFromDot.Evaluate(velDot);
            neededAcceleration = Vector2.ClampMagnitude(neededAcceleration, maxAcceleration);
            
            _rigidbody2D.AddForce(neededAcceleration * _rigidbody2D.mass);
        }

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
        
        private void AnimateVisuals(float deltaTime)
        {
            if (visuals != null && _rigidbody2D.velocity.sqrMagnitude > 0.001f)
            {
                // sway amount greater when closer to max speed.
                // Time scale has no effect as even though player is slower/faster, max speed is also slower/faster
                var maxSpeed = playerModel.Speed * playerModel.TimeScale;
                var speed = _rigidbody2D.velocity.magnitude;
                var swaySize = Mathf.Clamp01(speed / maxSpeed) * playerModel.SwayAmount;
                
                // sway speed directly effected by time scale
                var swaySpeed = playerModel.SwaySpeed * playerModel.TimeScale;
                _swayTime += swaySpeed * deltaTime;
                
                var swayAmount = Mathf.Sin(_swayTime) * swaySize;
                visuals.localRotation = initialVisualsRotation * Quaternion.Euler(0, 0, swayAmount);
            }
            else
            {
                _swayTime = 0f;
                visuals.localRotation = initialVisualsRotation;
            }
        }

        private void SetTimeScale(float deltaTime)
        {
            var timeScaleDeltaAbs = _timeScaleConfig.TimeScaleChangeSpeed * deltaTime;
            var timeScaleDelta = _mapModel.GetTimeScaleSignAtPosition(transform.position.XY()) * timeScaleDeltaAbs;
            playerModel.TimeScale = Mathf.Clamp
            (
                playerModel.TimeScale + timeScaleDelta,
                _timeScaleConfig.MinTimeScale,
                _timeScaleConfig.MaxTimeScale
            );
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
            var countDownDelta = deltaTime * playerModel.TimeScale;
            playerModel.NextShotCountDown = Mathf.Max(playerModel.NextShotCountDown - countDownDelta, 0f);
        }

        private void HandleShooting()
        {
            if (playerModel.NextShotCountDown > 0f) return;
            if (!Input.GetButton("Fire1")) return;

            Shoot();
            playerModel.NextShotCountDown = playerModel.FireRate;
        }

        private static readonly int MuzzleFlashAnimationHash = Animator.StringToHash("PlayerMuzzleFlashAnimation");
        
        
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
            
            
            playerModel.MuzzleFlashAnimator.Play(MuzzleFlashAnimationHash);
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
