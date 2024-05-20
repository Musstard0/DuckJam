using UnityEngine;

namespace DuckJam.Models
{
    [CreateAssetMenu(fileName = nameof(EnemySpawnConfig), menuName = "DuckJam/" + nameof(EnemySpawnConfig))]
    internal sealed class EnemySpawnConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float spawnInterval = 1f;
        [SerializeField, Min(0f)] private float spawnIntervalDeviation = 0.1f;
        
        public float SpawnInterval => spawnInterval;
        public float SpawnIntervalDeviation => spawnIntervalDeviation;
        public float RandomSpawnInterval => spawnInterval + Random.Range(-spawnIntervalDeviation, spawnIntervalDeviation);
    }
}