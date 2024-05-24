using System;
using System.Collections.Generic;
using DuckJam.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace DuckJam.Modules
{
    internal sealed class SpriteAnimationManager : MonoBehaviour
    {
        private const string AnimatorGameObjectName = "[SpriteAnimator]";
        private static readonly Type[] AnimatorComponents = {typeof(SpriteRenderer), typeof(SpriteAnimator)};
        public static SpriteAnimationManager Instance { get; private set; }

        [SerializeField] private int sortingOrder = 1;
        [SerializeField] private float zPosition = 0f;
        
        [SerializeField, Min(0f)] private float defaultFramesPerSecond = 30f;
        
        [SerializeField, Min(0)] private int initialPoolSize = 128;
        [SerializeField, Min(0)] private int maxPoolSize = 1024;
        [SerializeField] private SpriteImpactEffectDB impactFXSpriteDB;
       // [SerializeField] private ImpactFXSprites[] impactFXSprites;
        
        private ObjectPool<SpriteAnimator> _pool;
        private ImpactFXSprites[] _impactFXSprites;
        
        public IReadOnlyList<ImpactFXSprites> ImpactFXSpriteArr => _impactFXSprites;
        
        private SpriteAnimator OnCreateBullet()
        {
            var go = new GameObject(AnimatorGameObjectName, AnimatorComponents);
            var animator = go.GetComponent<SpriteAnimator>();
            animator.OnAnimationComplete = ReleaseAnimator;
            animator.SpriteRenderer.sortingOrder = sortingOrder;
            
            return animator;
        }
        
        private static void OnGetBullet(SpriteAnimator animator)
        {
            animator.SpriteRenderer.enabled = true;
            animator.SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
        
        private static void OnReleaseBullet(SpriteAnimator animator)
        {
            animator.SpriteRenderer.sprite = null;
            animator.SpriteRenderer.enabled = false;
            animator.Frames = Array.Empty<Sprite>();
        }
        
        private static void OnDestroyBullet(SpriteAnimator animator)
        {
            if(animator == null) return;
            Destroy(animator.gameObject);
        }
        
        private void ReleaseAnimator(SpriteAnimator animator)
        {
            _pool.Release(animator);
        }
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            _pool = new ObjectPool<SpriteAnimator>
            (
                OnCreateBullet, 
                OnGetBullet, 
                OnReleaseBullet,
                OnDestroyBullet,
                false,
                initialPoolSize,
                maxPoolSize
            );
            
            _impactFXSprites = impactFXSpriteDB.CreateEffects();
        }
        
        private void OnDestroy()
        {
            _pool?.Clear();
            _pool?.Dispose();
            
            if (Instance != this) return;
            Instance = null;
        }
        
        public SpriteAnimator CreateImpactAnimationEffect
        (
            IReadOnlyList<Sprite> frames,
            Vector2 position, 
            float sizeScale,
            float timeScale
        )
        {
            var spriteAnimator  = _pool.Get();
            spriteAnimator.transform.position = position.XY0(zPosition);
            spriteAnimator.transform.localScale = Vector3.one * sizeScale;
            spriteAnimator.Frames = frames;
            spriteAnimator.FramesPerSecond = defaultFramesPerSecond * timeScale;
            spriteAnimator.StartAnimation();

            return spriteAnimator;
        }
    }
    

    
    public enum ImpactFXColor : byte
    {
        Orange = 0,
        Purple = 1,
        Blue = 2,
        White = 3
    }
}