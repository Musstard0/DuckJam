using System;
using System.Collections.Generic;
using UnityEngine;

namespace DuckJam.Modules
{
    [RequireComponent(typeof(SpriteRenderer))]
    internal sealed class SpriteAnimator : MonoBehaviour
    {
        private const string InvokeMethodName = nameof(NextFrame);
        
        private SpriteRenderer _spriteRenderer;
        
        private bool _enabled;
        private float _framesPerSecond = 30f;
        private int _currentFrameIndex;
        
        public Action<SpriteAnimator> OnAnimationComplete { get; set; }
        
        public IReadOnlyList<Sprite> Frames { get; set; } = Array.Empty<Sprite>();
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        public float FramesPerSecond
        {
            get => _framesPerSecond;
            set => _framesPerSecond = Mathf.Max(0f, value);
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
        }

        private void NextFrame()
        {
            if (_currentFrameIndex >= Frames.Count)
            {
                OnAnimationComplete?.Invoke(this);
                CancelInvoke(InvokeMethodName);
                return;
            }
            
            var frame = Frames[_currentFrameIndex];
            _spriteRenderer.sprite = frame;

            _currentFrameIndex++;
        }
    }
}