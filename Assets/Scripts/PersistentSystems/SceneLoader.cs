using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DuckJam.PersistentSystems
{
    internal sealed class SceneLoader : MonoBehaviour
    {
        private const string PersistantSceneName = "Scene_Persistant";
        private const string MainMenuSceneName = "Scene_MainMenu";
        private const string GameSceneName = "Scene_Main";
        
        private readonly List<AsyncOperation> _loadOperations = new();
        
        public static SceneLoader Instance { get; private set; }
        
        public SceneId CurrentScene { get; private set; }

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init() 
        {
            if(SceneManager.GetSceneByName(PersistantSceneName).isLoaded) return;
            SceneManager.LoadScene(PersistantSceneName, LoadSceneMode.Additive);
        }
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;

            
            var isMainMenuSceneLoaded = SceneManager.GetSceneByName(MainMenuSceneName).isLoaded;
            var isGameSceneLoaded = SceneManager.GetSceneByName(GameSceneName).isLoaded;

#if UNITY_EDITOR
            if (isMainMenuSceneLoaded && isGameSceneLoaded)
            {
                Debug.LogError("MainMenu and Game scenes are both loaded. Unload one to before playing.");
                EditorApplication.isPlaying = false;
                return;
            }
#endif
            
            if(!isMainMenuSceneLoaded && !isGameSceneLoaded)
            {
                SceneManager.LoadScene(MainMenuSceneName, LoadSceneMode.Additive);
                CurrentScene = SceneId.MainMenu;
                return;
            }
            
            CurrentScene = isMainMenuSceneLoaded ? SceneId.MainMenu : SceneId.Game;
        }

        private void Start()
        {
            if (CurrentScene == SceneId.MainMenu)
            {
                MusicManager.Instance.PlayMenuMusic();
            }
            else
            {
                MusicManager.Instance.PlayGameMusic();
            }
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            Instance = null;
        }

        public void LoadMainMenu()
        {
            if(CurrentScene == SceneId.MainMenu) return;
            StartCoroutine(LoadSceneAsync(MainMenuSceneName, () =>
            {
                CanvasManager.Instance.ShowMainMenu();
                MusicManager.Instance.PlayMenuMusic();
            }));
            CurrentScene = SceneId.MainMenu;
        }
        
        public void LoadGame()
        {
            StartCoroutine(LoadSceneAsync(GameSceneName, MusicManager.Instance.PlayGameMusic));
            CurrentScene = SceneId.Game;
        }

        private IEnumerator LoadSceneAsync(string sceneName, Action onComplete)
        {
            CurrentScene = SceneId.Loading;
            GameModel.Reset();
            
            MusicManager.Instance.PlayTransitionMusic();
            var transitionMusicEndTime = Time.time + MusicManager.Instance.TransitionClipDuration;
            
            
            CanvasManager.Instance.ShowLoadingScreen();
            var loadingScreenEndTime = Time.time + CanvasManager.Instance.LoadingScreenFadeDuration;

            while (Time.time < loadingScreenEndTime)
            {
                yield return null;
            }
            
            
            for (var i = 0; i < SceneManager.loadedSceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if(scene.name == PersistantSceneName) continue;
                _loadOperations.Add(SceneManager.UnloadSceneAsync(scene));
            }
            
            while (_loadOperations.Count > 0)
            {
                var operation = _loadOperations[0];
                if (operation.isDone)
                {
                    _loadOperations.RemoveAt(0);
                    continue;
                }
                
                yield return null;
            }
            
            GameModel.Reset();
            
            var loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loadOperation.allowSceneActivation = false;
            
            while (!loadOperation.isDone)
            {
                if (loadOperation.progress >= 0.9f && Time.time >= transitionMusicEndTime)
                {
                    loadOperation.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            onComplete?.Invoke();
            CanvasManager.Instance.HideLoadingScreen();
        }
    }
    
    internal enum SceneId
    {
        Loading,
        MainMenu,
        Game
    }
}

