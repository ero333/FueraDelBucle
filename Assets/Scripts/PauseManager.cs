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

    // --- NUEVO: Casilla para arrastrar el panel MapadeNiveles desde el Inspector ---
   // [Header("Paneles de la Pausa")]
    //[Tooltip("Arrastra aquí el panel 'MapadeNiveles'")]
    //public GameObject mapadeNiveles;

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

    void Start()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        // Solo evitamos guardar "MapaNiveles" para que sí guarde "MenuPrincipal" o cualquier nivel
        if (escenaActual != "MapaNiveles")
        {
            PlayerPrefs.SetString("UltimoNivelJugado", escenaActual);
        }
    }

    void Update()
    {
        if (!Input.GetKeyDown(teclaPausa) && !Input.GetKeyDown(teclaPausaAlternativa)) return;

        if (!GameIsPaused && DialogoLog.HayDialogoActivo) return;

        if (GameIsPaused)
            Resume();
        else
            Pause();
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

        
       // if (mapadeNiveles != null) mapadeNiveles.SetActive(false);

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

    public void CargarNivelDesdePausa(string nombreEscena)
    {
        CerrandoEscena = true;
        Time.timeScale = 1f;
        GameIsPaused = false;

        if (nombreEscena == "MapaNiveles")
        {
            PlayerPrefs.SetInt("MapaDesdeMenu", 0);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene(nombreEscena);
    }
}
