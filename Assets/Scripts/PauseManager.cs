using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool CerrandoEscena = false;

    private static float tiempoFinBloqueoConfirmar;

    public static void BloquearConfirmar()
    {
        tiempoFinBloqueoConfirmar = Time.unscaledTime + 0.25f;
    }

    public GameObject pauseMenuUI;

    [Header("Nombre exacto de la escena del menú principal")]
    public string nombreEscenaMenu = "MenuPrincipal"; // Cambiá esto por el nombre real de tu escena

    [Header("Teclas para abrir y cerrar la pausa")]
    public KeyCode teclaPausa = KeyCode.Escape;
    public KeyCode teclaPausaAlternativa = KeyCode.Tab;

    void Awake()
    {
        CerrandoEscena = false;
        GameIsPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaPausa) || Input.GetKeyDown(teclaPausaAlternativa))
        {
            if (!GameIsPaused && DialogoLog.HayDialogoActivo) return;

            Alternar();
            return;
        }

        if (!SeConfirmo()) return;
        if (!PuedeAlternarConConfirmar()) return;

        Alternar();
    }

    private void Alternar()
    {
        if (GameIsPaused)
            Resume();
        else
            Pause();
    }

    private bool SeConfirmo()
    {
        return Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetKeyDown(KeyCode.E);
    }

    private bool PuedeAlternarConConfirmar()
    {
        if (Time.unscaledTime < tiempoFinBloqueoConfirmar) return false;

        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null) return false;

        if (LevelUIManager.PlacaAbierta) return false;
        if (DialogoLog.HayDialogoActivo) return false;

        if (!GameIsPaused && InterruptorTeclado.HayInterruptorCerca) return false;

        return true;
    }

    public void Pause()
    {
        if (GameIsPaused) return;
        if (LevelUIManager.PlacaAbierta) return;

        if (pauseMenuUI == null)
        {
            Debug.LogWarning("PauseManager: falta asignar el menu de pausa en " + gameObject.name, this);
            return;
        }

        pauseMenuUI.SetActive(true);
        pauseMenuUI.transform.SetAsLastSibling();
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        if (pauseMenuUI == null) return;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void ExitGame()
    {
        CerrandoEscena = true;
        Time.timeScale = 1f;
        GameIsPaused = false;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Reiniciar()
    {
        CerrandoEscena = true;
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        CerrandoEscena = true;
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}
