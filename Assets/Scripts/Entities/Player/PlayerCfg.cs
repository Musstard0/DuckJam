using System.Collections.Generic;
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
        public float minFootstepInterval = 0.1f;
        
        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] footstepClips;
        [SerializeField] private AudioClip[] gunshotClips;
        public AudioClip deathClip;
        public AudioClip hurtClip;
        [Min(0f)] public float minHurtSoundInterval = 0.1f;
        
        public IReadOnlyList<AudioClip> FootstepClips => footstepClips;
        public IReadOnlyList<AudioClip> GunshotClips => gunshotClips;
        public AudioClip RandomFootstepClip => footstepClips[Random.Range(0, footstepClips.Length)];
        public AudioClip RandomGunshotClip => gunshotClips[Random.Range(0, gunshotClips.Length)];
    }
}
