using System;
using System.Collections.Generic;
using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam.Modules.Projectiles
{
    internal sealed class ProjectileManager : MonoBehaviour
    {
        [SerializeField] private BulletController prefab;
        
        private readonly List<BulletController> _activeBullets = new();
        
        private void Awake()
        {
            // this is not a model, but regestering it here will make it easier to access
            GameModel.Register(this);
        }

        private void OnDestroy()
        {
            foreach (var bullet in _activeBullets)
            {
                if(bullet == null) continue;
                Destroy(bullet.gameObject);
            }
            
            _activeBullets.Clear();
        }

        public BulletController GetBullet(Vector3 position)
        {
            var bullet = Instantiate(prefab, position, Quaternion.identity);

            bullet.DisposeAction = DisposeBullet;
            
            _activeBullets.Add(bullet);
            
            return bullet;
        }
        
        public BulletController GetBullet(Vector2 position)
        {
            return GetBullet(position.XY0());
        }

        private void DisposeBullet(BulletController bulletController)
        {
            _activeBullets.Remove(bulletController);
            
            if(bulletController == null) return;
            Destroy(bulletController.gameObject);
        }
    }
}