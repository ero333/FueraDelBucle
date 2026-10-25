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

    [Header("Nombre exacto de las escenas de navegación")]
    public string nombreEscenaMenu = "MenuPrincipal";

    [Header("Teclas para abrir y cerrar la pausa")]
    public KeyCode teclaPausa = KeyCode.Escape;
    public KeyCode teclaPausaAlternativa = KeyCode.Tab;

    [Header("Cartel de Objetivo (Opcional)")]
    [Tooltip("Arrastrá aquí el GameObject del cartel de objetivos si lo usas en este nivel")]
    public GameObject cartelObjetivoUI;

    
    private bool cartelEstabaActivo;
    private bool dialogoEstabaActivo;

    void Awake()
    {
        ResetearPausa();
    }

    void Start()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        if (escenaActual != "MapaNiveles" && escenaActual != "Creditos")
        {
            PlayerPrefs.SetString("UltimoNivelJugado", escenaActual);
        }
    }

    void Update()
    {
        HELPMENU ayuda = FindFirstObjectByType<HELPMENU>();
        if (ayuda != null && ayuda.helpPanel != null && ayuda.helpPanel.activeSelf)
        {
            return; // No lee la tecla ESC para que se no salga del menu de pausa
        }

        bool dialogoActivoAhora = DialogoLog.HayDialogoActivo;
        bool cartelActivoAhora = EsCartelVisible();

        bool seAcabaDeCerrarAlgo = (dialogoEstabaActivo && !dialogoActivoAhora) || (cartelEstabaActivo && !cartelActivoAhora);

        
        dialogoEstabaActivo = dialogoActivoAhora;
        cartelEstabaActivo = cartelActivoAhora;

     
        if (!Input.GetKeyDown(teclaPausa) && !Input.GetKeyDown(teclaPausaAlternativa)) return;

       
        if (GameIsPaused)
        {
            Resume();
            return;
        }

        
        if (cartelActivoAhora || dialogoActivoAhora || seAcabaDeCerrarAlgo)
        {
            return;
        }

       
        Pause();
    }

   
    private bool EsCartelVisible()
    {
        if (cartelObjetivoUI != null && cartelObjetivoUI.activeInHierarchy)
        {
            return true;
        }

        PopupController[] popups = FindObjectsByType<PopupController>(FindObjectsSortMode.None);
        for (int i = 0; i < popups.Length; i++)
        {
            if (popups[i] != null && popups[i].enabled && popups[i].EstaVisible())
            {
                return true;
            }
        }

        return false;
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
        cartelEstabaActivo = false;
        dialogoEstabaActivo = false;
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