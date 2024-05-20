using UnityEngine;

namespace DuckJam.Entities
{
    internal sealed class EnemyController : MonoBehaviour
    {
        public float Speed { get; set; }
        public int Health { get; set; }
        public float CurrentTimeScale { get; set; } = 1f;
        public float ScaledSpeed => Speed * CurrentTimeScale;
        
        public Vector3 TargetPosition { get; set; }

        public Color Color 
        {
            set
            {
                var meshRenderer = GetComponent<MeshRenderer>();
                if (meshRenderer == null) return;
                meshRenderer.material.color = value;
            }
        }

        private void Awake()
        {
            TargetPosition = transform.position;
        }
    }
}