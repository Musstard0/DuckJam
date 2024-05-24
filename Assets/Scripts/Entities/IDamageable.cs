using UnityEngine;

namespace DuckJam.Entities
{
    public interface IDamageable
    {
        public float Health { get; }
        
        public void TakeDamage(float damage, Vector2 blowbackDirection);
    }
}