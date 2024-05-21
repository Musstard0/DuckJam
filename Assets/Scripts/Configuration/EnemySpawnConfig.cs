using UnityEngine;

namespace DuckJam.Configuration
{
    [CreateAssetMenu(fileName = nameof(EnemySpawnConfig), menuName = "DuckJam/" + nameof(EnemySpawnConfig))]
    internal sealed class EnemySpawnConfig : ScriptableObject
    {
        [Tooltip("Time between enemy spawns in seconds")]
        [SerializeField, Min(0f)] private float spawnInterval = 1f;
        
        [Tooltip("Random deviation from spawn interval in seconds")]
        [SerializeField, Min(0f)] private float spawnIntervalDeviation = 0.1f;
        
        [Tooltip("Maximum number of enemies that can be active at once")]
        [SerializeField, Min(0)] private int maxEnemies = 32;
        
        [Tooltip("How far from the map edge to spawn enemies")]
        [SerializeField, Min(0f)] private float mapEdgeSpawnBuffer = 1f;
        
        [SerializeField] private float spawnPositionZ = 0f;
        
        public float SpawnInterval => spawnInterval;
        public float SpawnIntervalDeviation => spawnIntervalDeviation;
        public int MaxEnemies => maxEnemies;
        public float MapEdgeSpawnBuffer => mapEdgeSpawnBuffer;
        public float RandomSpawnInterval => spawnInterval + Random.Range(-spawnIntervalDeviation, spawnIntervalDeviation);
        public float SpawnPositionZ => spawnPositionZ;
    }
}