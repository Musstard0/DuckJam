using UnityEngine;

namespace DuckJam
{
    [CreateAssetMenu(fileName = "PlayerCfg", menuName = "Game/PlayerCfg")]
    public class PlayerCfg : ScriptableObject
    {
        public float Health = 100f;
        public float Speed = 5f;
        public float Damage = 10f;
        public BulletController BulletPrefab;
        public float BulletSpeed = 10f;
        public float FireRate = 0.5f;
        public float Inertia = 0.1f;
    }
}
