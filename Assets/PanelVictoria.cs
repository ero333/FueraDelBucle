using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelVictoria : MonoBehaviour
{
    [Header("Panel")]
    [Tooltip("Arrastra aquí el panel de victoria desde la jerarquía.")]
    public GameObject panelVictoria;

    [Header("Resultados")]
    [Tooltip("Texto donde se muestra el tiempo del nivel.")]
    public Text textoTiempo;

    [Tooltip("Texto donde se muestran las vidas restantes.")]
    public Text textoVidas;

    [Tooltip("Arrastra aquí el jugador para leer sus vidas.")]
    public VidaJugador vidaJugador;

    [Header("Activación")]
    [Tooltip("Tag que debe tener el jugador.")]
    public string tagJugador = "Player";

    [Tooltip("Se activa solo cuando el jugador toca este objeto.")]
    public bool activarPorContacto = true;

    [Tooltip("Espera a que el jugador aterrice antes de mostrar el panel.")]
    public bool esperarAterrizaje = true;

    [Tooltip("Velocidad vertical por debajo de la cual se considera que ya aterrizó.")]
    public float velocidadVerticalMaxima = 0.5f;

    [Tooltip("Espera a que el Animator entre en el estado de reposo.")]
    public bool esperarIdle = true;

    [Tooltip("Nombre exacto del estado de reposo en el Animator.")]
    public string estadoIdle = "Iddle";

    [Tooltip("Segundos máximos de espera del estado de reposo antes de mostrar igual.")]
    public float esperaMaximaIdle = 1.5f;

    [Header("Gameplay")]
    [Tooltip("Congela el juego al ganar. Corresponde a la tarea de pausar gameplay.")]
    public bool pausarAlGanar = false;

    private bool activado;
    private bool esperando;

    private void Start()
    {
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el panel de victoria en el Inspector de PanelVictoria.");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (activado || esperando || !activarPorContacto) return;
        if (!other.CompareTag(tagJugador)) return;

        if (esperarAterrizaje)
        {
            Rigidbody2D rb = other.attachedRigidbody;

            if (rb != null && Mathf.Abs(rb.linearVelocity.y) > velocidadVerticalMaxima) return;
        }

        esperando = true;
        StartCoroutine(EsperarReposoYMostrar(other));
    }

    private IEnumerator EsperarReposoYMostrar(Collider2D other)
    {
        if (esperarIdle)
        {
            Animator anim = other.GetComponentInChildren<Animator>();

            if (anim != null)
            {
                float t = 0f;

                while (t < esperaMaximaIdle)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName(estadoIdle)) break;

                    t += Time.deltaTime;
                    yield return null;
                }
            }
        }

        esperando = false;

        MostrarVictoria();
    }

    public void MostrarVictoria()
    {
        if (activado) return;

        activado = true;

        MostrarResultados();

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }

        if (pausarAlGanar)
        {
            Time.timeScale = 0f;
        }
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

        if (textoVidas != null && vidaJugador != null)
        {
            textoVidas.text = "Vidas restantes: " + vidaJugador.cantidadDeVida;
        }
    }

    private void OnDestroy()
    {
        if (pausarAlGanar && activado)
        {
            Time.timeScale = 1f;
        }
    }
}
