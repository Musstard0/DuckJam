using System;
using DG.Tweening;
using UnityEngine;

namespace DuckJam.Entities.MainMenuCharacter
{
    internal sealed class MainMenuCharacterController : MonoBehaviour
    {
        [SerializeField] private Transform fallFromTransform;
        [SerializeField] private Transform fallToTransform;
        
        [SerializeField] private float fallInDuration = 1f;
        [SerializeField] private Ease fallInEase = Ease.OutBack;
        
        [SerializeField] private float fallOutDuration = 1f;
        [SerializeField] private Ease fallOutEase = Ease.InBack;
        
        private Sequence _sequence;
        private Vector3 _defaultPosition;

        private void Awake()
        {
            _defaultPosition = transform.position;
        }

        private void Start()
        {
            _sequence = DOTween.Sequence()
                .Append
                (
                    transform.DOMove(_defaultPosition, fallInDuration)
                        .From(fallFromTransform.position)
                        .SetEase(fallInEase)
                );
        }


        private void OnDestroy()
        {
            _sequence?.Kill();
        }


        public void FallOut()
        {
            _sequence?.Complete();
            
            _sequence = DOTween.Sequence()
                .Append
                (
                    transform.DOMove(fallToTransform.position, fallOutDuration)
                        .From(_defaultPosition)
                        .SetEase(fallOutEase)
                );
        }
    }
}