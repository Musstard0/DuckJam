using System;
using System.Collections.Generic;
using DuckJam.PersistentSystems;
using DuckJam.SharedConfiguration;
using DuckJam.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace DuckJam.Modules.Projectiles
{
    internal sealed class ProjectileManager : MonoBehaviour
    {
        [SerializeField] private Sprite orangeBulletSprite;
        [SerializeField] private Sprite purpleBulletSprite;
        [SerializeField] private Sprite blueBulletSprite;
        [SerializeField] private Sprite greenBulletSprite;
        [SerializeField] private Sprite pinkBulletSprite;
        [SerializeField] private Sprite whiteBulletSprite;
        
        [SerializeField] private BulletController prefab;
        [SerializeField, Min(1f)] private float boundsPaddingFactor = 2f;
        [SerializeField] private float zPosition = 0f;
        
        [SerializeField, Min(0)] private int initialPoolSize = 128;
        [SerializeField, Min(0)] private int maxPoolSize = 1024;
        
        private readonly List<BulletController> _activeBullets = new();

        private ObjectPool<BulletController> _bulletPool;
        
        private MapModel _mapModel;
        private TimeScaleConfig _timeScaleConfig;
        
        private Rect _bulletBounds;
        
        private void Awake()
        {
            _bulletPool = new ObjectPool<BulletController>
            (
                OnCreateBullet, 
                OnGetBullet, 
                OnReleaseBullet,
                OnDestroyBullet,
                false,
                initialPoolSize,
                maxPoolSize
            );
            
            GameModel.Register(this);
        }

        private BulletController OnCreateBullet()
        {
            var bullet = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            bullet.DisposeAction = DisposeBullet;
            bullet.gameObject.layer = LayerUtils.DisabledLayer;
            bullet.SpriteRenderer.enabled = false;
            return bullet;
        }
        
        private static void OnGetBullet(BulletController bullet)
        {
            bullet.gameObject.layer = LayerUtils.ProjectileLayer;
            bullet.Rigidbody2D.WakeUp();
            bullet.SpriteRenderer.enabled = true;
            bullet.Exploded = false;
        }
        
        private static void OnReleaseBullet(BulletController bullet)
        {
            bullet.gameObject.layer = LayerUtils.DisabledLayer;
            bullet.Rigidbody2D.Sleep();
            bullet.Rigidbody2D.velocity = Vector2.zero;
            bullet.SpriteRenderer.enabled = false;
        }
        
        private static void OnDestroyBullet(BulletController bullet)
        {
            if(bullet == null) return;
            Destroy(bullet.gameObject);
        }

        private void Start()
        {
            _mapModel = GameModel.Get<MapModel>();
            _timeScaleConfig = GameModel.Get<TimeScaleConfig>();
            
            var size = _mapModel.Size * boundsPaddingFactor;
            var minPosition = _mapModel.CenterPosition2D - size / 2;
            _bulletBounds = new Rect(minPosition, size);
        }

        private void LateUpdate()
        {
            var timeScaleDeltaAbs = _timeScaleConfig.TimeScaleChangeSpeed * Time.deltaTime;

            for (var i = _activeBullets.Count - 1; i >= 0; i--)
            {
                var bullet = _activeBullets[i];
                var position = bullet.Position2D;
                
                if (!_bulletBounds.Contains(position))
                {
                    _bulletPool.Release(bullet);
                    _activeBullets.RemoveAt(i);
                    continue;
                }
             
                var timeScaleDelta = _mapModel.GetTimeScaleSignAtPosition(position) * timeScaleDeltaAbs;
                bullet.TimeScale = Mathf.Clamp
                (
                    bullet.TimeScale + timeScaleDelta, 
                    _timeScaleConfig.MinTimeScale, 
                    _timeScaleConfig.MaxTimeScale
                );
                
                bullet.Rigidbody2D.velocity = bullet.Direction * (bullet.Speed * bullet.TimeScale);
                
            }
        }

        private void OnDestroy()
        {
            _bulletPool.Clear();
            _bulletPool.Dispose();
            _activeBullets.Clear();
        }

        private Sprite GetBulletSprite(ImpactFXColor color)
        {
            return color switch
            {
                ImpactFXColor.Orange => orangeBulletSprite,
                ImpactFXColor.Purple => purpleBulletSprite,
                ImpactFXColor.Blue => blueBulletSprite,
                ImpactFXColor.White => whiteBulletSprite,
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
        }
        

        public void ShootBullet(Vector2 position, Vector2 targetPosition, int targetLayer, float damage, float baseSpeed, float initialTimeScale, ImpactFXColor color)
        {
            var bullet = _bulletPool.Get();
            bullet.transform.position = position.XY0(zPosition);
            
            _activeBullets.Add(bullet);
            
            
            bullet.Direction = (targetPosition - position).normalized;
            bullet.TargetLayer = targetLayer;
            bullet.Damage = Mathf.Max(damage, 0f);
            bullet.Speed = Mathf.Max(baseSpeed, 0f);
            bullet.TimeScale = Mathf.Clamp(initialTimeScale, _timeScaleConfig.MinTimeScale, _timeScaleConfig.MaxTimeScale);
            bullet.SpriteRenderer.sprite = GetBulletSprite(color);
            bullet.ImpactFXColor = color;
            
            bullet.Rigidbody2D.velocity = bullet.Direction * (bullet.Speed * bullet.TimeScale);
        }

        private void DisposeBullet(BulletController bulletController)
        {
            _activeBullets.Remove(bulletController);
            _bulletPool.Release(bulletController);
        }
        
        private void OnDrawGizmos()
        {
            var sw = _bulletBounds.min.XY0();
            var se = new Vector2(_bulletBounds.xMax, _bulletBounds.yMin).XY0();
            var ne = _bulletBounds.max.XY0();
            var nw = new Vector2(_bulletBounds.xMin, _bulletBounds.yMax).XY0();
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(sw, se);
            Gizmos.DrawLine(se, ne);
            Gizmos.DrawLine(ne, nw);
            Gizmos.DrawLine(nw, sw);
        }
    }
}