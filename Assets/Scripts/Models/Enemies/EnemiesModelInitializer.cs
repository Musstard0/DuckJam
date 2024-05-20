using System;
using DuckJam.Entities;
using UnityEngine;

namespace DuckJam.Models
{
    internal sealed class EnemiesModelInitializer : MonoBehaviour
    {
        [SerializeField] private EnemyType[] enemyTypes;
        
        private void Awake()
        {
            var enemiesModel = new EnemiesModel();
            foreach (var enemyType in enemyTypes) enemiesModel.AddEnemyType(enemyType);
            
            GameModel.Register(enemiesModel);
        }
    }
    
    [Serializable]
    internal sealed class EnemyType
    {
        [SerializeField] private string name;
        [SerializeField] private Enemy prefab;
        [SerializeField, Min(0f)] private float speed;
            
        public string Name => name;
        public Enemy Prefab => prefab;
        public float Speed => speed;
    }
}