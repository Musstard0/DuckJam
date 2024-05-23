using System.Collections.Generic;
using UnityEngine;

namespace DuckJam.PersistentSystems
{
    internal sealed class AudioFXManager : MonoBehaviour
    {
        [SerializeField, Min(1)] private int initialAudioSourceCount = 8;
        
        private readonly Stack<AudioSource> _inactiveAudioSources = new();
        private readonly List<AudioSource> _activeAudioSources = new();
     
        private float _volume = 1f;
        
        public static AudioFXManager Instance { get; private set; }

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = Mathf.Clamp01(value);
                foreach (var source in _activeAudioSources) source.volume = _volume;
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void Start()
        {
            for (var i = 0; i < initialAudioSourceCount; i++)
            {
                _inactiveAudioSources.Push(CreateSource());
            }
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            Instance = null;
        }

        private void Update()
        {
            for (var i = _activeAudioSources.Count - 1; i >= 0; i--)
            {
                var source = _activeAudioSources[i];
                if(source.isPlaying) continue;
                
                _activeAudioSources.RemoveAt(i);
                
                source.clip = null;
                _inactiveAudioSources.Push(source);
            }
        }

        private AudioSource CreateSource()
        {
            var source = gameObject.AddComponent<AudioSource>();

            source.loop = false;
            source.playOnAwake = false;
            source.volume = Volume;
            
            return source;
        }
        
        public void PlayClip(AudioClip clip, float pitch = 1f)
        {
            if (!_inactiveAudioSources.TryPop(out var source))
            {
                source = CreateSource();
            }
            
            source.clip = clip;
            source.volume = Volume;
            source.pitch = pitch;
            
            _activeAudioSources.Add(source);
            source.Play();
        }
    }
}