using UnityEngine;

namespace DuckJam
{
    [CreateAssetMenu(fileName = "PlayerCfg", menuName = "Game/PlayerCfg")]
    public class PlayerCfg : ScriptableObject
    {
        public float Health = 100f;
        public float Speed = 5f;
        public float Damage = 10f;
        public float BulletSpeed = 10f;
        public float FireRate = 0.5f;
        public float Inertia = 0.1f;
        
        public float acceleration = 200f;
        public AnimationCurve accelerationFactorFromDot;
        public float maxAccelerationForce = 150f;
        public AnimationCurve maxAccelerationForceFactorFromDot;

        public float SwaySpeed; // Speed of the sway animation
        public float SwayAmount; // Amount of sway
    }
}
