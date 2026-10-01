using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelDerrota : MonoBehaviour
{
    [Header("Panel")]
    [Tooltip("Arrastra aquí el panel de derrota desde la jerarquía.")]
    public GameObject panelDerrota;

    [Tooltip("Boton de reintentar. Se conecta solo al iniciar.")]
    public Button botonReintentar;

    [Header("Resultados")]
    [Tooltip("Texto donde se muestra el tiempo que sobreviviste.")]
    public Text textoTiempo;

    [Tooltip("Texto donde se muestran las estrellas obtenidas.")]
    public Text textoEstrellas;

    [Tooltip("Estrellas obtenidas al perder.")]
    public int cantidadEstrellas = 0;

    [Header("Jugador")]
    [Tooltip("Arrastra aquí el jugador para leer sus vidas.")]
    public VidaJugador vidaJugador;

    [Tooltip("Si se deja vacío, se busca por el tag Player al iniciar.")]
    public string tagJugador = "Player";

    [Header("Diálogos del Nivel (Opcionales)")]
    [Tooltip("Diálogo principal de burla al perder (dejar vacío si el nivel no tiene).")]
    public DialogoLog dialogoPrincipal;

    [Tooltip("Diálogo secundario si cumple cierta condición (dejar vacío si no se usa).")]
    public DialogoLog dialogoAlternativo;

    [Header("Condición para Diálogo Alternativo")]
    public bool usarCondicionEstrellas = false;
    public int estrellasRequeridas = 1;

    [Header("Gameplay")]
    [Tooltip("Congela el juego mientras el panel está en pantalla.")]
    public bool pausarAlPerder = true;

    private bool activado;
    private bool esperando;
    private Coroutine rutinaEsperando;
    private DialogoLog dialogoEnUso;

    private void Start()
    {
        if (panelDerrota != null)
        {
            panelDerrota.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el panel de derrota en el Inspector de PanelDerrota.");
        }

        if (botonReintentar != null)
        {
            botonReintentar.onClick.AddListener(Reintentar);
        }

        if (vidaJugador == null)
        {
            GameObject j = GameObject.FindGameObjectWithTag(tagJugador);
            if (j != null) vidaJugador = j.GetComponent<VidaJugador>();
        }
    }

    /// <summary>
    /// Llamado desde VidaJugador cuando finaliza la animación de muerte
    /// </summary>
    public void IniciarSecuenciaDerrota()
    {
        if (activado || esperando) return;

        esperando = true;
        rutinaEsperando = StartCoroutine(EsperarYMostrarDerrota());
    }

    private IEnumerator EsperarYMostrarDerrota()
    {
        // 1. Determina si en este nivel hay un diálogo configurado
        ElegirDialogo();

        // 2. Si HAY diálogo en el nivel, lo ejecuta y espera a que termine
        if (dialogoEnUso != null)
        {
            bool dialogoTerminado = false;
            System.Action alTerminar = () => dialogoTerminado = true;

            dialogoEnUso.AlTerminarDialogo += alTerminar;
            dialogoEnUso.Interactuar();

            yield return new WaitUntil(() => dialogoTerminado);

            dialogoEnUso.AlTerminarDialogo -= alTerminar;
        }

        // 3. Si NO hay diálogo (o ya terminó), muestra la UI de derrota
        esperando = false;
        MostrarDerrota();
    }

    private void ElegirDialogo()
    {
        if (usarCondicionEstrellas && cantidadEstrellas >= estrellasRequeridas && dialogoAlternativo != null)
        {
            dialogoEnUso = dialogoAlternativo;
        }
        else
        {
            dialogoEnUso = dialogoPrincipal;
        }
    }

    public void SaltarDialogoYMostrarDerrota()
    {
        if (rutinaEsperando != null)
        {
            StopCoroutine(rutinaEsperando);
            rutinaEsperando = null;
        }

        if (dialogoEnUso != null)
        {
            dialogoEnUso.TerminarDialog();
        }

        esperando = false;
        MostrarDerrota();
    }

    public void MostrarDerrota()
    {
        if (activado) return;

        activado = true;

        MostrarResultados();

        if (panelDerrota != null)
        {
            panelDerrota.SetActive(true);
        }

        if (pausarAlPerder)
        {
            Time.timeScale = 0f;
        }
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void IrAEscena(string nombreEscena)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }

    public bool YaSeActivo()
    {
        return activado;
    }

    private void MostrarResultados()
    {
        if (textoTiempo != null)
        {
            float total = Time.timeSinceLevelLoad;
            int minutos = Mathf.FloorToInt(total / 60f);
            int segundos = Mathf.FloorToInt(total % 60f);

            textoTiempo.text = string.Format("Tiempo: {0:00}:{1:00}", minutos, segundos);
        }

        if (textoEstrellas != null)
        {
            textoEstrellas.text = "Estrellas restantes: " + cantidadEstrellas;
        }
    }

    private void OnDestroy()
    {
        if (pausarAlPerder && activado)
        {
            Time.timeScale = 1f;
        }
    }
}