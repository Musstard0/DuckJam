using System;
using UnityEngine;
using UnityEngine.UI;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(Slider))]
    internal sealed class UIMusicVolumeInput : MonoBehaviour
    {
        private Slider _slider;
        
        private void Awake()
        {
            _slider = GetComponent<Slider>();
            
        }

        private void Start()
        {
            _slider.value = MusicManager.Instance.Volume;        
        }

        private void OnEnable() => _slider.onValueChanged.AddListener(HandleInput);
        private void OnDisable() => _slider.onValueChanged.RemoveListener(HandleInput);

        private void HandleInput(float input)
        {
            MusicManager.Instance.Volume = input;
        }
    }
}