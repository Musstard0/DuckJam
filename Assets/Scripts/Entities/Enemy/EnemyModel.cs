using DuckJam.Modules;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam.Entities
{
    internal sealed class EnemyModel : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
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
    }
}