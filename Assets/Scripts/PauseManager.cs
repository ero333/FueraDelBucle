using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool CerrandoEscena = false;

    public GameObject pauseMenuUI;

    [Header("Nombre exacto de la escena del menú principal")]
    public string nombreEscenaMenu = "MenuPrincipal";

    [Header("Teclas para abrir y cerrar la pausa")]
    public KeyCode teclaPausa = KeyCode.Escape;
    public KeyCode teclaPausaAlternativa = KeyCode.Tab;

    [Header("Cartel de Objetivo")]
    [Tooltip("Arrastrá aquí el GameObject del cartel de objetivos si lo usas en este nivel")]
    public GameObject cartelObjetivoUI;

    void Awake()
    {
        ResetearPausa();
    }

    void Start()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        if (escenaActual != "MapaNiveles")
        {
            PlayerPrefs.SetString("UltimoNivelJugado", escenaActual);
        }
    }

    void Update()
    {
        if (!Input.GetKeyDown(teclaPausa) && !Input.GetKeyDown(teclaPausaAlternativa)) return;

    
        if (GameIsPaused)
        {
            Resume();
            return;
        }

       
        if (cartelObjetivoUI != null && cartelObjetivoUI.activeInHierarchy) return;

    
        Pause();
    }

    public void Pause()
    {
        if (GameIsPaused) return;

        if (pauseMenuUI == null)
        {
            Debug.LogWarning("PauseManager: falta asignar el menú de pausa en " + gameObject.name, this);
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

    private void ResetearPausa()
    {
        CerrandoEscena = false;
        GameIsPaused = false;
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        ResetearPausa();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Reiniciar()
    {
        ResetearPausa();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        ResetearPausa();
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void CargarNivelDesdePausa(string nombreEscena)
    {
        ResetearPausa();
        SceneManager.LoadScene(nombreEscena);
    }
}