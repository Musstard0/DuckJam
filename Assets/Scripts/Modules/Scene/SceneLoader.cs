using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DuckJam.Modules
{
    internal sealed class SceneLoader : MonoBehaviour
    {
        private const string PersistantSceneName = "Scene_Persistant";
        private const string MainMenuSceneName = "Scene_MainMenu";
        private const string GameSceneName = "Scene_Main";
        
        public static SceneLoader Instance { get; private set; }
        
        public SceneId CurrentScene { get; private set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
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

        private void OnDestroy()
        {
            if (Instance != this) return;
            Instance = null;
        }

        public void LoadMainMenu()
        {
            if(CurrentScene == SceneId.MainMenu) return;

            StartCoroutine(LoadSceneAsync(GameSceneName, MainMenuSceneName));
            CurrentScene = SceneId.MainMenu;
        }
        
        public void LoadGame()
        {
            if (CurrentScene == SceneId.Game)
            {
                StartCoroutine(LoadSceneAsync(GameSceneName, GameSceneName));
                return;
            }
            
            StartCoroutine(LoadSceneAsync(MainMenuSceneName, GameSceneName));
            
            CurrentScene = SceneId.Game;
        }
        
        private static IEnumerator LoadSceneAsync(string unloadSceneName, string loadSceneName)
        {
            CanvasManager.Instance.ShowLoadingScreen();
            
            var unloadOperation = SceneManager.UnloadSceneAsync(unloadSceneName);
            while (!unloadOperation.isDone)
            {
                yield return null;
            }
            
            GameModel.Reset();
            
            var loadOperation = SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
            loadOperation.allowSceneActivation = false;
            
            while (!loadOperation.isDone)
            {
                if (loadOperation.progress >= 0.9f)
                {
                    loadOperation.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            CanvasManager.Instance.HideLoadingScreen();
        }
    }
    
    internal enum SceneId
    {
        MainMenu,
        Game
    }
}

