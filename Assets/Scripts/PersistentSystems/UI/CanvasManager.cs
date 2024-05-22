using System;
using UnityEngine;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(Canvas))]
    internal sealed class CanvasManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup loadingScreen;
        
        [SerializeField] private MenuPanel mainMenu;
        [SerializeField] private MenuPanel pauseMenu;
        [SerializeField] private MenuPanel gameOverMenu;
        [SerializeField] private MenuPanel settingsMenu;
        [SerializeField] private MenuPanel controlsMenu;
        [SerializeField] private MenuPanel creditsMenu;
        
        public static CanvasManager Instance { get; private set; }
        
        private bool _isPaused;
        private MenuPanel _currentPanel;
        
        private MenuPanel ParentPanel => SceneLoader.Instance.CurrentScene == SceneId.MainMenu ? mainMenu : pauseMenu;
        
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
            HidePanel(loadingScreen);
            
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
                UIPanel.SettingsMenu => settingsMenu,
                UIPanel.ControlsMenu => controlsMenu,
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
            ShowPanel(loadingScreen);
        }
        
        public void HideLoadingScreen()
        {
            HidePanel(loadingScreen);
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
        SettingsMenu,
        ControlsMenu,
        CreditsMenu
    }
    
    internal enum EscapeAction : byte
    {
        None,
        Close,
        NavigateToParent
    }
}