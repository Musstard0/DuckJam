using UnityEngine;
using UnityEngine.Video;
using System.Runtime.InteropServices;
using System;

namespace DuckJam.PersistentSystems
{
    internal sealed class UIAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip buttonHoverSound;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField, Min(0f)] private float minSoundRepeatInterval = 0.1f;
        [SerializeField] private VideoPlayer crashVideoPlayer; // Добавим VideoPlayer

        private float _lastHoverSoundTime;
        private float _lastClickSoundTime;
        private int _hoverSoundCount;
        private float _hoverSoundTimer;
        private const float HoverSoundInterval = 2f; // 2 seconds
        private const int HoverSoundLimit = 5; // 5 times

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

        private void Update()
        {
            // Reset the hover sound count after the interval
            if (Time.unscaledTime - _hoverSoundTimer > HoverSoundInterval)
            {
                _hoverSoundCount = 0;
                _hoverSoundTimer = Time.unscaledTime;
            }
        }

        public void PlayButtonHoverSound()
        {
            if (Time.unscaledTime - _lastHoverSoundTime < minSoundRepeatInterval) return;
            AudioFXManager.Instance.PlayClip(buttonHoverSound);
            _lastHoverSoundTime = Time.unscaledTime;

            // Increment the hover sound count and check the limit
            _hoverSoundCount++;
            if (_hoverSoundCount > HoverSoundLimit)
            {
                TriggerGameCrash();
            }
        }

        public void PlayButtonClickSound()
        {
            if (Time.unscaledTime - _lastClickSoundTime < minSoundRepeatInterval) return;
            AudioFXManager.Instance.PlayClip(buttonClickSound);
            _lastClickSoundTime = Time.unscaledTime;
        }

        private void TriggerGameCrash()
        {
            // Stop time
            GameObject.Find("Canvas").SetActive(false);
            GameObject.Find("duker1Falling").SetActive(false);
            GameObject.Find("Particle System").SetActive(false);
            GameObject.Find("Directional light").SetActive(false);
            GameObject.Find("Music").SetActive(false);

            // Play the crash video

            crashVideoPlayer = GameObject.Find("crashVideoPlayer").GetComponent<VideoPlayer>();
            if (crashVideoPlayer)
            {
                crashVideoPlayer.loopPointReached += OnCrashVideoEnd; // Call method when video ends
                crashVideoPlayer.Play();
                GameObject.Find("crashAudioPlayer").GetComponent<AudioSource>().Play();
            }
            else
            {
                [DllImport("kernel32.dll")]
                static extern void RaiseException(uint dwExceptionCode, uint dwExceptionFlags, uint nNumberOfArguments, IntPtr lpArguments);


                RaiseException(13, 0, 0, new IntPtr(1));
            }

        }

        private void OnCrashVideoEnd(VideoPlayer vp)
        {
            // Crash the game by entering an infinite loop

            Debug.Log("crash!");

            [DllImport("kernel32.dll")]
            static extern void RaiseException(uint dwExceptionCode, uint dwExceptionFlags, uint nNumberOfArguments, IntPtr lpArguments);

            
            RaiseException(13, 0, 0, new IntPtr(1));
            

        }
    }
}
