using UnityEngine;

namespace DuckJam.Configuration
{
    [CreateAssetMenu(fileName = nameof(EnemySpawnConfig), menuName = "DuckJam/" + nameof(EnemySpawnConfig))]
    internal sealed class EnemySpawnConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float spawnInterval = 1f;
        [SerializeField, Min(0f)] private float spawnIntervalDeviation = 0.1f;
        [SerializeField, Min(0)] private int maxEnemies = 32;
        
        public float SpawnInterval => spawnInterval;
        public float SpawnIntervalDeviation => spawnIntervalDeviation;
        public int MaxEnemies => maxEnemies;
        public float RandomSpawnInterval => spawnInterval + Random.Range(-spawnIntervalDeviation, spawnIntervalDeviation);
    }
}