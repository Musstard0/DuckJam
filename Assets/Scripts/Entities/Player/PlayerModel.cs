using UnityEngine;

namespace DuckJam
{
    [System.Serializable]
    public class PlayerModel
    {
        public float Health;
        public float MaxHealth;
        public float Speed;
        public float Damage;
        public GameObject BulletPrefab;
        public float BulletSpeed;
        public Transform FirePoint;
        public float FireRate;
        public float Inertia;
    }
}
