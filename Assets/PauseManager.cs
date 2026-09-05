using UnityEngine;
using UnityEngine.SceneManagement; 
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void ExitGame()
    {
    
        Time.timeScale = 1f;
        GameIsPaused = false;

      
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Para que funcione también en el Editor
#else
            Application.Quit();
#endif
    }
}
