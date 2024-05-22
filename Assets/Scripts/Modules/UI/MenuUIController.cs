using UnityEngine;
using UnityEngine.SceneManagement;

namespace DuckJam.Modules
{
    internal sealed class MenuUIController : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("Scene_Main", LoadSceneMode.Single);
        }
        
        public void MainMenu()
        {
            SceneManager.LoadScene("Scene_MainMenu", LoadSceneMode.Single);
        }
        
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}