using System;
using DuckJam.Entities;
using UnityEngine;

namespace DuckJam.Modules
{
    [CreateAssetMenu(fileName = nameof(EnemiesConfig), menuName = "DuckJam/" + nameof(EnemiesConfig))]
    internal sealed class EnemiesConfig : ScriptableObject
    { 
        [Header("Melee Enemy")] 
        [SerializeField] private EnemyModel meleeEnemyPrefab;
        [Tooltip("The range at which the enemy can attack from")]
        [SerializeField] private Range meleeEnemyAttackRange;
        [Tooltip("The range the enemy will try to maintain from its target. Must be within attack range.")]
        [SerializeField] private Range meleeEnemyTargetRange;
        [SerializeField, Min(0f)] private float meleeEnemyMaxHealth = 10f;
        [SerializeField, Min(0f)] private float meleeEnemySpeed = 5f;
        [SerializeField, Min(0f)] private float meleeEnemyDamage = 3f;
        
        
        [Header("Ranged Enemy")]
        [SerializeField] private EnemyModel rangedEnemyPrefab;
        [SerializeField] private BulletController rangedEnemyBulletPrefab;
        [Tooltip("The range at which the enemy can attack from")]
        [SerializeField] private Range rangedEnemyAttackRange;
        [Tooltip("The range the enemy will try to maintain from its target. Must be within attack range.")]
        [SerializeField] private Range rangedEnemyTargetRange;
        [SerializeField, Min(0f)] private float rangedEnemyMaxHealth = 10f;
        [SerializeField, Min(0f)] private float rangedEnemySpeed = 5f;
        [SerializeField, Min(0f)] private float rangedEnemyBulletDamage = 2f;
        [SerializeField, Min(0f)] private float rangedEnemyBulletSpeed = 16f;
        [SerializeField, Min(0f)] private float rangedEnemyFireRate = 1f;


        private void OnValidate()
        {
            // ensure target range is within attack range
            
            // melee enemy
            meleeEnemyAttackRange.Min = Mathf.Clamp(meleeEnemyAttackRange.Min, 0f, meleeEnemyAttackRange.Max);
            meleeEnemyTargetRange.Max = Mathf.Clamp(meleeEnemyTargetRange.Max, meleeEnemyAttackRange.Min, meleeEnemyAttackRange.Max);
            meleeEnemyTargetRange.Min = Mathf.Clamp(meleeEnemyTargetRange.Min, meleeEnemyAttackRange.Min, meleeEnemyTargetRange.Max);
            
            // ranged enemy
            rangedEnemyAttackRange.Min = Mathf.Clamp(rangedEnemyAttackRange.Min, 0f, rangedEnemyAttackRange.Max);
            rangedEnemyTargetRange.Max = Mathf.Clamp(rangedEnemyTargetRange.Max, rangedEnemyAttackRange.Min, rangedEnemyAttackRange.Max);
            rangedEnemyTargetRange.Min = Mathf.Clamp(rangedEnemyTargetRange.Min, rangedEnemyAttackRange.Min, rangedEnemyTargetRange.Max);
        }

        public EnemyModel MeleeEnemyPrefab => meleeEnemyPrefab;
        public Range MeleeEnemyAttackRange => meleeEnemyAttackRange;
        public Range MeleeEnemyTargetRange => meleeEnemyTargetRange;
        public float MeleeEnemyMaxHealth => meleeEnemyMaxHealth;
        public float MeleeEnemySpeed => meleeEnemySpeed;
        public float MeleeEnemyDamage => meleeEnemyDamage;

        
        
        public EnemyModel RangedEnemyPrefab => rangedEnemyPrefab;
        public BulletController RangedEnemyBulletPrefab => rangedEnemyBulletPrefab;
        public Range RangedEnemyAttackRange => rangedEnemyAttackRange;
        public Range RangedEnemyTargetRange => rangedEnemyTargetRange;
        public float RangedEnemyMaxHealth => rangedEnemyMaxHealth;
        public float RangedEnemySpeed => rangedEnemySpeed;
        public float RangedEnemyBulletDamage => rangedEnemyBulletDamage;
        public float RangedEnemyBulletSpeed => rangedEnemyBulletSpeed;
        public float RangedEnemyFireRate => rangedEnemyFireRate;

        [Serializable]
        public struct Range
        {
            [SerializeField, Min(0f)] public float Min;
            [SerializeField, Min(0f)] public float Max;
        }
    }
}