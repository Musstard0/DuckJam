using UnityEngine;

namespace DuckJam.Modules
{
    [CreateAssetMenu(fileName = nameof(EnemySpawnConfig), menuName = "DuckJam/" + nameof(EnemySpawnConfig))]
    internal sealed class EnemySpawnConfig : ScriptableObject
    {
        [Tooltip("Time between enemy spawns in seconds")]
        [SerializeField, Min(0f)] private float spawnInterval = 1f;
        
        [Tooltip("Random deviation from spawn interval in seconds")]
        [SerializeField, Min(0f)] private float spawnIntervalDeviation = 0.1f;
        
        [SerializeField, Min(0)] private int initialMaxEnemies = 16;
        
        [Tooltip("Maximum number of enemies that can be active at once")]
        [SerializeField, Min(0)] private int maxEnemies = 48;
        
        [SerializeField, Min(0)] private float maxEnemesIncrementRate = 8f;
        
        [Tooltip("How far from the map edge to spawn enemies")]
        [SerializeField, Min(0f)] private float mapEdgeSpawnBuffer = 1f;
        
        [SerializeField] private float spawnPositionZ = 0f;
        
        public float SpawnInterval => spawnInterval;
        public float SpawnIntervalDeviation => spawnIntervalDeviation;
        public int MaxEnemies => maxEnemies;
        public int InitialMaxEnemies => initialMaxEnemies;
        public float MaxEnemiesIncrementRate => maxEnemesIncrementRate;
        public float MapEdgeSpawnBuffer => mapEdgeSpawnBuffer;
        public float RandomSpawnInterval => spawnInterval + Random.Range(-spawnIntervalDeviation, spawnIntervalDeviation);
        public float SpawnPositionZ => spawnPositionZ;
    }
}