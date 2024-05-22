using UnityEngine;

namespace DuckJam
{
    [System.Serializable]
    public class PlayerModel
    {
        public Transform Transform;
        public float Health;
        public float MaxHealth;
        public float Speed;
        public float Damage;
        public GameObject BulletPrefab;
        public float BulletSpeed;
        public Transform FirePoint;
        public float FireRate;
        public float Inertia;
        public float SwaySpeed; // Speed of the sway animation
        public float SwayAmount; // Amount of sway
        public float horizontalInput;
        public float verticalInput;
    }
}
