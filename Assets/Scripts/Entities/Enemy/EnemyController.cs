using System;
using DuckJam.Modules;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam.Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    internal sealed class EnemyController : MonoBehaviour, IDamageable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public Rigidbody2D Rigidbody2D { get; private set;}
        public float Health { get; set; }
        public float Speed { get; set; }
        public EnemiesConfig.Attack Attack { get; set; }
        public float TimeScale { get; set; }
        public float LastAttackTime { get; set; }
        
        public bool IsDead => Health <= 0f;
        public Vector2 Position2D => transform.position.XY();
        
        public Color Color 
        {
            set
            {
                if(spriteRenderer == null) return;
                spriteRenderer.color = value;
            }
        }

        private void Awake()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void TakeDamage(float damage)
        {
            if(IsDead) return;

            Health = Mathf.Max(Health - damage, 0f);
            if (IsDead) OnDeath();
        }

        private void OnDeath()
        {
            
        }
    }
}