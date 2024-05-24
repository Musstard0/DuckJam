using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace DuckJam.Modules
{
    [RequireComponent(typeof(SpriteRenderer))]
    internal sealed class SpriteAnimator : MonoBehaviour
    {
        private const string InvokeMethodName = nameof(NextFrame);
        
        private SpriteRenderer _spriteRenderer;
        private Tween _fadeOutTween;
        
        private bool _enabled;
        private float _framesPerSecond = 30f;
        private int _currentFrameIndex;
        private float _fadeOutTime;
        
        public Action<SpriteAnimator> OnAnimationComplete { get; set; }
        
        public IReadOnlyList<Sprite> Frames { get; set; } = Array.Empty<Sprite>();
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        public float FramesPerSecond
        {
            get => _framesPerSecond;
            set => _framesPerSecond = Mathf.Max(0f, value);
        }
        
        public float FadeOutTime
        {
            get => _fadeOutTime;
            set => _fadeOutTime = Mathf.Max(0f, value);
        }

        public void StartAnimation()
        {
            _currentFrameIndex = 0;
            InvokeRepeating(InvokeMethodName, 1 / _framesPerSecond, 1 / _framesPerSecond);
        }
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.enabled = false;
        }

        private void OnDestroy()
        {
            CancelInvoke(InvokeMethodName);
            _fadeOutTween?.Kill();
        }

        private void NextFrame()
        {
            if (_currentFrameIndex >= Frames.Count)
            {
                CancelInvoke(InvokeMethodName);
                
                _spriteRenderer
                    .DOFade(0f, FadeOutTime)
                    .OnComplete(() => OnAnimationComplete?.Invoke(this));
                
                return;
            }
            
            var frame = Frames[_currentFrameIndex];
            _spriteRenderer.sprite = frame;

            _currentFrameIndex++;
        }
    }
}