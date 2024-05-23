using System;
using DuckJam.Modules;
using DuckJam.Utilities;
using UnityEngine;
using UnityEngine.AI;

namespace DuckJam.Entities
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(NavMeshAgent))]
    internal sealed class EnemyController : MonoBehaviour, IDamageable
    {
        [SerializeField] private SpriteRenderer bodySpriteRenderer;
        [SerializeField] private SpriteRenderer fillSpriteRenderer;
        [SerializeField] private Transform visuals;
        
        public Transform VisualsTransform => visuals;
        public Rigidbody2D Rigidbody2D { get; private set;}
        public NavMeshAgent NavMeshAgent { get; private set; }
        
        public float Health { get; set; }
        public float Speed { get; set; }
        public EnemiesConfig.Attack Attack { get; set; }
        public float TimeScale { get; set; }

        public float AttackCooldownCountdown { get; set; }
        
        public bool Moving { get; set; }
        public float SwayTime { get; set; }
        
        public bool IsDead => Health <= 0f;
        public Vector2 Position2D => transform.position.XY();
        
        public Color Color 
        {
            set => fillSpriteRenderer.color = value;
        }

        private void Awake()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            NavMeshAgent.updateRotation = false;
            NavMeshAgent.updatePosition = false;
            NavMeshAgent.updateUpAxis = false;
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