using UnityEngine;

namespace DuckJam.PersistentSystems
{
    internal sealed class UIAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip buttonHoverSound;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField, Min(0f)] private float minSoundRepeatInterval = 0.1f;
        
        private float _lastHoverSoundTime;
        private float _lastClickSoundTime;
        
        public static UIAudioManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        
        private void OnDestroy()
        {
            if (Instance != this) return;
            Instance = null;
        }
        
        public void PlayButtonHoverSound()
        {
            if(Time.unscaledTime - _lastHoverSoundTime < minSoundRepeatInterval) return;
            AudioFXManager.Instance.PlayClip(buttonHoverSound);
            _lastHoverSoundTime = Time.unscaledTime;
        }
        
        public void PlayButtonClickSound()
        {
            if(Time.unscaledTime - _lastClickSoundTime < minSoundRepeatInterval) return;
            AudioFXManager.Instance.PlayClip(buttonClickSound);
            _lastClickSoundTime = Time.unscaledTime;
        }
    }
}