using System;
using DG.Tweening;
using DuckJam.Entities.MainMenuCharacter;
using UnityEngine;
using UnityEngine.Video;

namespace DuckJam.PersistentSystems
{
    [RequireComponent(typeof(Canvas))]
    internal sealed class CanvasManager : MonoBehaviour
    {
        // hackity hack hack
        [SerializeField] private Camera tempCamera;
        [SerializeField] private VideoPlayer introCutscene;
        
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
        private Sequence _introVideoSequence;
        private Sequence _mainMenuDuckFallSequence;
        
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
            loadingScreen.alpha = 1f;
            
            var currentScene = SceneLoader.Instance.CurrentScene;

#if UNITY_WEBGL
            
#else
            if (currentScene == SceneId.Loading)
            {
                // intro cutscene
                
                HideLoadingScreen();

                var pauseBeforeFade = Mathf.Max((float)introCutscene.length - loadingScreenFadeDuration, 0f);
                
                _introVideoSequence = DOTween.Sequence()
                    .AppendInterval(pauseBeforeFade)
                    .AppendCallback(() => ShowLoadingScreen())
                    .AppendInterval(loadingScreenFadeDuration)
                    .AppendCallback(() =>
                    {
                        Destroy(introCutscene.gameObject);
                        Destroy(tempCamera.gameObject);
                        LoadMainMenu(false);
                    });
                
                // introCutscene.loopPointReached += source =>
                // {
                //     ShowLoadingScreen(() =>
                //     {
                //         Destroy(introCutscene.gameObject);
                //         Destroy(tempCamera.gameObject);
                //         LoadMainMenu(false);
                //     });
                // };
                introCutscene.Play();
                
                return;
            }
#endif
            
            Destroy(introCutscene.gameObject);
            Destroy(tempCamera.gameObject);
            HideLoadingScreen();
            
            if (currentScene == SceneId.MainMenu)
            {
                mainMenu.Show();
                _currentPanel = mainMenu;
                MusicManager.Instance.PlayMenuMusic();
            }
            else
            {
                MusicManager.Instance.PlayGameMusic();
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
            _introVideoSequence?.Kill();
            _mainMenuDuckFallSequence?.Kill();
            
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
                    
                    _currentPanel.Hide(() =>
                    {
                        Time.timeScale = 1f;
                        _isPaused = false;
                    });
                    _currentPanel = null;
                    

                    
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
            
            if (_currentPanel != null)
            {
                _currentPanel.Hide(() =>
                {
                    if (panelToShow == null) return;
                    _currentPanel = panelToShow;
                    _currentPanel.Show();
                });
                
                _currentPanel = null;
                return;
            }
            
            if(panelToShow == null) return;
            
            _currentPanel = panelToShow;
            _currentPanel.Show();
        }
        
        public void LoadMainMenu(bool playTransitionMusic = true)
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
            
            SceneLoader.Instance.LoadMainMenu(playTransitionMusic);
        }

        public void ShowMainMenu()
        {
            mainMenu.Show();
            _currentPanel = mainMenu;
        }
        
        public void LoadGame(bool playTransitionMusic = true)
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

            // hackity hack hack - make the main menu character fall out of frame before game loads
            if (SceneLoader.Instance.CurrentScene == SceneId.MainMenu)
            {
                var mainMenuCharacterController = FindAnyObjectByType<MainMenuCharacterController>();
                if (mainMenuCharacterController != null)
                {
                    mainMenuCharacterController.FallOut();
                    
                    _mainMenuDuckFallSequence?.Kill();
                    _mainMenuDuckFallSequence = DOTween.Sequence()
                        .AppendInterval(0.7f)
                        .AppendCallback(() => SceneLoader.Instance.LoadGame(playTransitionMusic));
                    
                    return;
                }
            }
            
            SceneLoader.Instance.LoadGame(playTransitionMusic);
        }
        
        public float ShowLoadingScreen()
        {
            loadingScreen.blocksRaycasts = true;
            
            if (loadingScreen.alpha >= 1f)
            {
                return 0f;
            }
            
            _loadingScreenFadeTween?.Complete();
            
            _loadingScreenFadeTween = loadingScreen.DOFade(1f, loadingScreenFadeDuration)
                .From(0f)
                .SetEase(loadingScreenFadeInEase);

            return loadingScreenFadeDuration;
        }
        
        public void HideLoadingScreen()
        {
            loadingScreen.blocksRaycasts = false;
            
            _loadingScreenFadeTween?.Complete();
            
            _loadingScreenFadeTween = loadingScreen.DOFade(0f, loadingScreenFadeDuration)
                .From(1f)
                .SetEase(loadingScreenFadeOutEase);
        }
        
        public void ShowGameOverMenu(int score)
        {
            NavigateTo(UIPanel.GameOverMenu);
            _currentPanel = gameOverMenu;
            _currentPanel.OptionalText = $"Score: {score}";
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