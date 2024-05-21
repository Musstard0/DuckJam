using UnityEngine;

namespace DuckJam
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerModel playerModel;
        public PlayerCfg playerCfg;

        private float lastShotTime;


        private void Awake()
        {
            InitializeModel();
            GameModel.Register(playerModel);
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
                FirePoint = transform.Find("FirePoint"),  // Assuming there's a child named "FirePoint"
                FireRate = playerCfg.FireRate,
                Inertia = playerCfg.Inertia
            };
        }

        private void Update()
        {
            HandleMovement();
            HandleFlip();
            HandleShooting();
        }

        private void HandleMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0).normalized;
            Vector3 newPosition = transform.position + moveDirection * playerModel.Speed * Time.deltaTime;

            transform.position = newPosition;
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mousePosition - playerModel.FirePoint.position).normalized;

            GameObject bullet = Instantiate(playerModel.BulletPrefab, playerModel.FirePoint.position, Quaternion.identity);
            Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
            bulletRB.velocity = direction * playerModel.BulletSpeed;
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
