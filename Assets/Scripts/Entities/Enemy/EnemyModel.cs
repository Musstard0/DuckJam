using UnityEngine;

namespace DuckJam.Entities
{
    internal sealed class EnemyModel : MonoBehaviour
    {
        public float Health { get; set; }
        public float TimeScale { get; set; }
        public float LastAttackTime { get; set; }
        public bool Ranged { get; set; }
        public bool IsDead => Health <= 0f;
        
        public Color Color 
        {
            set
            {
                var meshRenderer = GetComponent<MeshRenderer>();
                if (meshRenderer == null) return;
                meshRenderer.material.color = value;
            }
        }
    }
}