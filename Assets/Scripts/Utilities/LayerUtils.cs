using UnityEngine;

namespace DuckJam.Utilities
{
    internal static class LayerUtils
    {
        public static readonly int PlayerLayer = LayerMask.NameToLayer("Player");
        public static readonly int EnemyLayer = LayerMask.NameToLayer("Enemy");
        public static readonly int ProjectileLayer = LayerMask.NameToLayer("Projectile");
        public static readonly int DisabledLayer = LayerMask.NameToLayer("Disabled");
        public static readonly int TerrainLayer = LayerMask.NameToLayer("Terrain");
    }
}