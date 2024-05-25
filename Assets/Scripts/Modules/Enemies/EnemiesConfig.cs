using System;
using System.Collections.Generic;
using DuckJam.Entities;
using DuckJam.PersistentSystems;
using DuckJam.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DuckJam.Modules
{
    [CreateAssetMenu(fileName = nameof(EnemiesConfig), menuName = "DuckJam/" + nameof(EnemiesConfig))]
    internal sealed class EnemiesConfig : ScriptableObject
    { 
        [SerializeField] private EnemyType[] enemyTypes;
        
        [SerializeField] private AudioClip deathSound;
        [SerializeField, Min(0f)] private float deathSoundMinInterval = 0.1f;
        
        [Header("Visuals")]
        [SerializeField, Min(0f)] private float swaySpeed;
        [SerializeField, Min(0f)] private float swayAmount;
        
        [Header("Death Effect")]
        [SerializeField] private int[] deathEffectIndices;
        [SerializeField, Min(0f)] private float deathEffectSizeScale = 10f;
        
        [Header("Spawn Effect")]
        [SerializeField] private int[] spawnEffectIndices;
        [SerializeField, Min(0f)] private float spawnEffectSizeScale = 10f;

        public IReadOnlyList<EnemyType> EnemyTypes => enemyTypes ?? Array.Empty<EnemyType>();
        public AudioClip DeathSound => deathSound;
        public float DeathSoundMinInterval => deathSoundMinInterval;
        public float SwaySpeed => swaySpeed;
        public float SwayAmount => swayAmount;
        public int DeathSpriteEffectIndex => deathEffectIndices[Random.Range(0, deathEffectIndices.Length)];
        public float DeathEffectSizeScale => deathEffectSizeScale;
        
        public int SpawnSpriteEffectIndex => spawnEffectIndices[Random.Range(0, spawnEffectIndices.Length)];
        public float SpawnEffectSizeScale => spawnEffectSizeScale;  
        
        private void OnValidate()
        {
#if UNITY_EDITOR
            foreach (var enemyType in EnemyTypes) enemyType.Attack.Validate(); 
#endif
        }
        
        [Serializable]
        public class EnemyType
        {
            [SerializeField] private string name;
            [SerializeField] private EnemyController prefab;
            [SerializeField, Min(0f)] private float maxHealth;
            [SerializeField, Min(0f)] private float speed;
            [SerializeField] private Attack attack;
            
            public string Name => name;
            public EnemyController Prefab => prefab;
            public float MaxHealth => maxHealth;
            public float Speed => speed;
            public Attack Attack => attack;
        }
        
        [Serializable]
        public class Attack
        {
            [SerializeField] private bool isRanged;
            [SerializeField, Min(0f)] private float damage;
            [SerializeField, Min(0f)] private float minDistance;
            [SerializeField, Min(0f)] private float maxDistance;
            [SerializeField, Min(0f)] private float cooldown;
            [SerializeField] private AudioClip attackSound;
            
            [Tooltip("If attack is ranged - the speed of the projectile")]
            [SerializeField, Min(0f)] private float speed;
            [SerializeField] private ImpactFXColor color = ImpactFXColor.Purple;
            [SerializeField, Min(0f)] private float attackFXScale = 10f;
            [SerializeField, Min(0f)] private float attackFXOffsetPositionDistance = 1f;
            [SerializeField, Min(0)] private int attackFXIndex;
            
            public bool IsRanged => isRanged;
            public float Damage => damage;
            public float MinDistance => minDistance;
            public float MaxDistance => maxDistance;
            public float Cooldown => cooldown;
            public float Speed => speed;
            public AudioClip AttackSound => attackSound;
            public ImpactFXColor Color => color;
            public float AttackFXScale => attackFXScale;
            public float AttackFXOffsetPositionDistance => attackFXOffsetPositionDistance;
            public int AttackFXIndex => attackFXIndex;
            
#if UNITY_EDITOR
            internal void Validate()
            {
                if (minDistance < maxDistance) return;
                minDistance = Mathf.Clamp(minDistance, 0f, maxDistance);
            }
#endif
        }
    }
}