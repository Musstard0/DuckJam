using System;
using DG.Tweening;
using UnityEngine;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(Canvas))]
    internal sealed class CanvasManager : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float loadingScreenFadeDuration = 0.5f;
        [SerializeField] private Ease loadingScreenFadeInEase = Ease.Linear;
        [SerializeField] private Ease loadingScreenFadeOutEase = Ease.Linear;
        
        [SerializeField] private CanvasGroup loadingScreen;
        
        [SerializeField] private MenuPanel mainMenu;
        [SerializeField] private MenuPanel pauseMenu;
        [SerializeField] private MenuPanel gameOverMenu;
        [SerializeField] private MenuPanel controlsMenu;
        [SerializeField] private MenuPanel creditsMenu;
        
        public static CanvasManager Instance { get; private set; }
        
        private bool _isPaused;
        private MenuPanel _currentPanel;
        
        private Tween _loadingScreenFadeTween;
        
        private MenuPanel ParentPanel => SceneLoader.Instance.CurrentScene == SceneId.MainMenu ? mainMenu : pauseMenu;
        
        public float LoadingScreenFadeDuration => loadingScreenFadeDuration;
        
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
            loadingScreen.interactable = false;
            loadingScreen.blocksRaycasts = false;
            loadingScreen.alpha = 0f;
            
            if (SceneLoader.Instance.CurrentScene == SceneId.MainMenu)
            {
                mainMenu.Show();
                _currentPanel = mainMenu;
            }
        }

        private void Update()
        {
            if(!Input.GetKeyDown(KeyCode.Escape)) return;
            OnEscape();
        }

        private void OnDestroy()
        {
            _loadingScreenFadeTween?.Kill();
            
            if (Instance != this) return;
            Instance = null;
        }

        private void OnEscape()
        {
            var currentScene = SceneLoader.Instance.CurrentScene;

            if (currentScene != SceneId.Game)
            {
                if (_isPaused)
                {
                    Time.timeScale = 1f;
                    _isPaused = false;
                }
                
                return;
            }

            
            
            
            if (!_isPaused)
            {
                Time.timeScale = 0f;
                _isPaused = true;
                
                pauseMenu.Show();
                _currentPanel = pauseMenu;

                return;
            }

            if (_currentPanel == null)
            {
                Time.timeScale = 1f;
                _isPaused = false;
                
                return;
            }


            switch (_currentPanel.EscapeAction)
            {
                case EscapeAction.None:
                    break;
                
                case EscapeAction.Close:
                    
                    _currentPanel.Hide();
                    _currentPanel = null;
                    
                    Time.timeScale = 1f;
                    _isPaused = false;
                    
                    break;
                case EscapeAction.NavigateToParent:
                    NavigateTo(ParentPanel.Panel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        


        public void NavigateToParent()
        {
            NavigateTo(ParentPanel.Panel);
        }
        
        public void NavigateTo(UIPanel panel)
        {
            if(_currentPanel != null && _currentPanel.Panel == panel) return;
            
            if (_currentPanel != null)
            {
                _currentPanel.Hide();
            }
            
            var panelToShow = panel switch
            {
                UIPanel.None => null,
                UIPanel.MainMenu => mainMenu,
                UIPanel.PauseMenu => pauseMenu,
                UIPanel.GameOverMenu => gameOverMenu,
                UIPanel.ExplanationMenu => controlsMenu,
                UIPanel.CreditsMenu => creditsMenu,
                _ => null
            };
            
            if(panelToShow == null) return;
            
            _currentPanel = panelToShow;
            _currentPanel.Show();
        }
        
        public void LoadMainMenu()
        {
            if (_isPaused)
            {
                Time.timeScale = 1f;
                _isPaused = false;
            }

            if (_currentPanel != null)
            {
                _currentPanel.Hide();
                _currentPanel = null;
            }
            
            SceneLoader.Instance.LoadMainMenu();

            if (SceneLoader.Instance.CurrentScene == SceneId.MainMenu)
            {
                mainMenu.Show();
                _currentPanel = mainMenu;
            }
        }
        
        public void LoadGame()
        {
            if (_isPaused)
            {
                Time.timeScale = 1f;
                _isPaused = false;
            }
            
            if (_currentPanel != null)
            {
                _currentPanel.Hide();
                _currentPanel = null;
            }
            
            SceneLoader.Instance.LoadGame();
        }
        
        
        
        public void ShowLoadingScreen()
        {
            loadingScreen.blocksRaycasts = true;

            _loadingScreenFadeTween?.Complete();
            
            _loadingScreenFadeTween = loadingScreen.DOFade(1f, loadingScreenFadeDuration)
                .From(0f)
                .SetEase(loadingScreenFadeInEase);
        }
        
        public void HideLoadingScreen()
        {
            loadingScreen.blocksRaycasts = false;
            
            _loadingScreenFadeTween?.Complete();
            
            _loadingScreenFadeTween = loadingScreen.DOFade(0f, loadingScreenFadeDuration)
                .From(1f)
                .SetEase(loadingScreenFadeInEase);
        }
        
        public void ShowGameOverMenu(int score)
        {
            NavigateTo(UIPanel.GameOverMenu);
            _currentPanel = gameOverMenu;
            _currentPanel.OptionalText = $"Score: {score}";
        }
        
        private static void HidePanel(CanvasGroup canvasGroup)
        {
            if(canvasGroup == null) return;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        private static void ShowPanel(CanvasGroup canvasGroup)
        {
            if(canvasGroup == null) return;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
    
    internal enum UIPanel : byte
    {
        None,
        MainMenu,
        PauseMenu,
        GameOverMenu,
        ExplanationMenu,
        CreditsMenu
    }
    
    internal enum EscapeAction : byte
    {
        None,
        Close,
        NavigateToParent
    }
}