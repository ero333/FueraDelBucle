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

    [Header("Aparición")]
    [Tooltip("Segundos de espera tras morir, para que se vea la animación de muerte.")]
    public float retrasoAparicion = 1.2f;

    [Header("Gameplay")]
    [Tooltip("Congela el juego mientras el panel está en pantalla.")]
    public bool pausarAlPerder = true;

    private bool activado;

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

    private void Update()
    {
        if (activado || vidaJugador == null) return;

        if (vidaJugador.cantidadDeVida <= 0)
        {
            activado = true;
            StartCoroutine(MostrarConRetraso());
        }
    }

    private IEnumerator MostrarConRetraso()
    {
        if (retrasoAparicion > 0f) yield return new WaitForSeconds(retrasoAparicion);

        MostrarDerrota();
    }

    public void MostrarDerrota()
    {
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
