using UnityEngine;

namespace DuckJam.Entities
{
    public interface IDamageable
    {
        public void TakeDamage(float damage, Vector2 blowbackDirection);
    }
}