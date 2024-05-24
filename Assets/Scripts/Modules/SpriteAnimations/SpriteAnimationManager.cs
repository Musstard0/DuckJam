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

        [SerializeField] private float zPosition = 0f;
        
        [SerializeField, Min(0f)] private float defaultFramesPerSecond = 30f;
        [SerializeField, Min(0f)] private float defaultFadeOutTime = 0.5f;
        
        [SerializeField, Min(0)] private int initialPoolSize = 128;
        [SerializeField, Min(0)] private int maxPoolSize = 1024;
        [SerializeField] private ImpactFXSprites[] impactFXSprites;
        
        
        private ObjectPool<SpriteAnimator> _pool;
        
        
        public IReadOnlyList<ImpactFXSprites> ImpactFXSpriteArr => impactFXSprites;
        
        
        private SpriteAnimator OnCreateBullet()
        {
            var go = new GameObject(AnimatorGameObjectName, AnimatorComponents);
            var animator = go.GetComponent<SpriteAnimator>();
            animator.OnAnimationComplete = ReleaseAnimator;
            
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
        }
        
        private void OnDestroy()
        {
            _pool?.Clear();
            _pool?.Dispose();
            
            if (Instance != this) return;
            Instance = null;
        }


        // private IReadOnlyList<Sprite> GetImpactFXSprites(int effectIndex, ImpactFXColor color)
        // {
        //     return color switch 
        //     {
        //         ImpactFXColor.Orange => impactFXSprites[effectIndex].Orange,
        //         ImpactFXColor.Purple => impactFXSprites[effectIndex].Purple,
        //         ImpactFXColor.Blue => impactFXSprites[effectIndex].Blue,
        //         ImpactFXColor.Green => impactFXSprites[effectIndex].Green,
        //         ImpactFXColor.Pink => impactFXSprites[effectIndex].Pink,
        //         ImpactFXColor.White => impactFXSprites[effectIndex].White,
        //         _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        //     };
        // }


        public void CreateImpactAnimationEffect(IReadOnlyList<Sprite> frames, Vector2 position, float scale)
        {
            CreateImpactAnimationEffect(frames, position, scale, defaultFramesPerSecond, defaultFadeOutTime);
        }
        
        public SpriteAnimator CreateImpactAnimationEffect
        (
            IReadOnlyList<Sprite> frames,
            Vector2 position, 
            float scale,
            float framesPerSecond, 
            float fadeOutTime
        )
        {
            var spriteAnimator  = _pool.Get();
            spriteAnimator.transform.position = position.XY0(zPosition);
            spriteAnimator.transform.localScale = Vector3.one * scale;
            spriteAnimator.Frames = frames;
            spriteAnimator.FramesPerSecond = framesPerSecond;
            spriteAnimator.FadeOutTime = fadeOutTime;
            spriteAnimator.StartAnimation();

            return spriteAnimator;
        }
        
        [Serializable]
        internal sealed class ImpactFXSprites
        {
            [SerializeField] private Sprite[] orange;
            [SerializeField] private Sprite[] purple;
            [SerializeField] private Sprite[] blue;
            [SerializeField] private Sprite[] green;
            [SerializeField] private Sprite[] pink;
            [SerializeField] private Sprite[] white;
            
            public IReadOnlyList<Sprite> Orange => orange;
            public IReadOnlyList<Sprite> Purple => purple;
            public IReadOnlyList<Sprite> Blue => blue;
            public IReadOnlyList<Sprite> Green => green;
            public IReadOnlyList<Sprite> Pink => pink;
            public IReadOnlyList<Sprite> White => white;

            public IReadOnlyList<Sprite> GetFramesForColor(ImpactFXColor color)
            {
                return color switch 
                {
                    ImpactFXColor.Orange => Orange,
                    ImpactFXColor.Purple => Purple,
                    ImpactFXColor.Blue => Blue,
                    ImpactFXColor.Green => Green,
                    ImpactFXColor.Pink => Pink,
                    ImpactFXColor.White => White,
                    _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
                };
            }
        }
    }
    
    public enum ImpactFXColor : byte
    {
        Orange = 0,
        Purple = 1,
        Blue = 2,
        Green = 3,
        Pink = 4,
        White = 5
    }
}