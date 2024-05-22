using System;
using DuckJam.Entities;
using DuckJam.Modules.Projectiles;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        private PlayerModel playerModel;
        public PlayerCfg playerCfg;

        private float lastShotTime;
        private Transform gun; // Reference to the child object representing the gun
        private Transform visuals; // Reference to the child object representing visuals
        private Quaternion initialVisualsRotation; // Initial rotation of the visuals

        // Centralized location for creating and managing projectiles
        private ProjectileManager _projectileManager;
        private Rigidbody2D rb;
        private void Awake()
        {
            visuals = transform.Find("Visuals"); // Assuming the child object is named "Visuals"
            gun = transform.Find("Gun");
            rb = GetComponent<Rigidbody2D>();
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
                BulletPrefab = playerCfg.BulletPrefab,
                BulletSpeed = playerCfg.BulletSpeed,
                FirePoint = gun.transform.Find("FirePoint"),  // Assuming there's a child named "FirePoint"
                FireRate = playerCfg.FireRate,
                SwaySpeed = playerCfg.SwaySpeed,
                SwayAmount = playerCfg.SwayAmount
            };
        }

        private void Update()
        {
            HandleMovement();
            //HandleFlip();
            HandleShooting();
            AnimateVisuals();
        }

        private void HandleMovement()
        {
            playerModel.horizontalInput = Input.GetAxisRaw("Horizontal");
            playerModel.verticalInput = Input.GetAxisRaw("Vertical");

            rb.velocity = new Vector2(playerModel.horizontalInput, playerModel.verticalInput).normalized * playerModel.Speed;

        }

        private void HandleFlip()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mousePosition - gun.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(angle) < 90)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, -180, 0);
            }
        }


        private void AnimateVisuals()
        {
            if (visuals != null && (playerModel.horizontalInput != 0 || playerModel.verticalInput != 0))
            {
                float swayAmount = Mathf.Sin(Time.time * playerModel.SwaySpeed) * playerModel.SwayAmount;
                visuals.localRotation = initialVisualsRotation * Quaternion.Euler(0, 0, swayAmount);
            }
            else
            {
                visuals.localRotation = initialVisualsRotation;
            }
        }

        private void HandleShooting()
        {
            if (Input.GetButton("Fire1") && Time.time - lastShotTime >= playerModel.FireRate)
            {
                Shoot();
                lastShotTime = Time.time;
            }
        }

        private void Shoot()
        {
            BulletController bullet = _projectileManager.GetBullet(playerModel.FirePoint.position);
            bullet.TargetLayer = LayerUtils.EnemyLayer;
            bullet.Damage = playerModel.Damage;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - playerModel.FirePoint.position).normalized;
            bullet.GetComponent<Rigidbody2D>().velocity = direction * playerModel.BulletSpeed;
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
            Debug.Log("Player has died");
            // Destroy(gameObject);
        }
    }
}
