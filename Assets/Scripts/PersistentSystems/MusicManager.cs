using UnityEngine;

namespace DuckJam.PersistentSystems
{
    internal sealed class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }
        
        [SerializeField] private AudioClip menuMusic;
        [SerializeField, Range(0f,1f)] private float menuMusicVolume = 1f;
        [SerializeField] private AudioClip transitionMusic;
        [SerializeField, Range(0f,1f)] private float transitionMusicVolume = 1f;
        [SerializeField] private AudioClip gameMusic;
        [SerializeField, Range(0f,1f)] private float gameMusicVolume = 1f;
     
        private AudioSource _audioSource;
        
        private float _volume = 1f;
        private CurrentClip _currentClip = CurrentClip.None;
        
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = Mathf.Clamp01(value);
                _audioSource.volume = GetClipVolume(_currentClip) * _volume;
            }
        }

        public float TransitionClipDuration => transitionMusic.length;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.volume = _volume * GetClipVolume(_currentClip);
            _audioSource.playOnAwake = false;
        }
        
        private void OnDestroy()
        {
            if (Instance != this) return;
            Instance = null;
        }

        public void PlayMenuMusic()
        {
            if (_currentClip == CurrentClip.Menu) return;
            StopCurrentClip();
            
            _currentClip = CurrentClip.Menu;
            _audioSource.volume = GetClipVolume(_currentClip) * _volume;
            _audioSource.loop = true;
            _audioSource.clip = menuMusic;
            _audioSource.Play();
        }
        
        public void PlayTransitionMusic()
        {
            if (_currentClip == CurrentClip.Transition) return;
            StopCurrentClip();
            
            _currentClip = CurrentClip.Transition;
            _audioSource.volume = GetClipVolume(_currentClip) * _volume;
            _audioSource.loop = false;
            _audioSource.clip = transitionMusic;
            _audioSource.Play();
        }
        
        public void PlayGameMusic()
        {
            if (_currentClip == CurrentClip.Game) return;
            StopCurrentClip();
            
            _currentClip = CurrentClip.Game;
            _audioSource.volume = GetClipVolume(_currentClip) * _volume;
            _audioSource.loop = true;
            _audioSource.clip = gameMusic;
            _audioSource.Play();
        }

        private void StopCurrentClip()
        {
            if(_currentClip == CurrentClip.None) return;
            
            _currentClip = CurrentClip.None;
            _audioSource.Stop();
        }
        
        private float GetClipVolume(CurrentClip clip)
        {
            return clip switch
            {
                CurrentClip.Menu => menuMusicVolume,
                CurrentClip.Transition => transitionMusicVolume,
                CurrentClip.Game => gameMusicVolume,
                _ => 0f
            };
        }
        
        private enum CurrentClip : byte
        {
            None,
            Menu,
            Transition,
            Game
        }
    }
}