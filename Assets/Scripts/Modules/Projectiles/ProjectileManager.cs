using System;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam.Modules.Projectiles
{
    internal sealed class ProjectileManager : MonoBehaviour
    {
        [SerializeField] private BulletController prefab;
        
        private void Awake()
        {
            // this is not a model, but regestering it here will make it easier to access
            GameModel.Register(this);
        }

        public BulletController GetBullet(Vector3 position)
        {
            return Instantiate(prefab, position, Quaternion.identity);
        }
        
        public BulletController GetBullet(Vector2 position)
        {
            return GetBullet(position.XY0());
        }
    }
}