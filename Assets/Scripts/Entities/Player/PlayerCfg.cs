using UnityEngine;

namespace DuckJam
{
    [CreateAssetMenu(fileName = "PlayerCfg", menuName = "Game/PlayerCfg")]
    public class PlayerCfg : ScriptableObject
    {
        public float Health = 100f;
        public float Speed = 5f;
        public float Damage = 10f;
        public GameObject BulletPrefab;
        public float BulletSpeed = 10f;
        public float FireRate = 1f;
        public float SwaySpeed = 5f; // Speed of the sway animation
        public float SwayAmount = 10f; // Amount of sway
    }
}
