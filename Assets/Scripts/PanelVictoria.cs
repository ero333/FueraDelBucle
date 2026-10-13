using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelVictoria : MonoBehaviour
{
    [Header("Configuración de Progreso")]
    public string claveNivel = "Nivel1";

    [Header("Audio")]
    [Tooltip("Arrastra aquí el clip de audio que sonará al tocar la puerta.")]
    public AudioClip sonidoVictoria;

    [Range(0f, 1f)]
    [Tooltip("El clip está normalizado a 0 dB: en 1 tapa la música.")]
    public float volumenVictoria = 0.6f;

    [Tooltip("Silencio que el clip trae al principio y que se saltea para que suene junto con el cartel.")]
    public float inicioClip = 0.2f;

    [Tooltip("Cuánto suena como máximo, contando desde inicioClip.")]
    public float duracionMaxima = 2.5f;

    [Tooltip("Segundos de desvanecido final para que no corte de golpe.")]
    public float desvanecido = 0.4f;

    [Tooltip("Si el jugador apagó el sonido con el botón del altavoz, la victoria tampoco suena.")]
    public bool respetarMute = true;

    [Header("Panel")]
    public GameObject panelVictoria;

    [Header("Resultados")]
    public Text textoTiempo;
    public Text textoVidas;
    public VidaJugador vidaJugador;

    [Header("Diálogo final")]
    public DialogoLog dialogoFinal;

    [Header("Activación")]
    public string tagJugador = "Player";
    public bool activarPorContacto = true;

    [Header("Espera para Lectura Normal (Sin Skip)")]
    public bool esperarAterrizaje = true;
    public float velocidadVerticalMaxima = 0.5f;
    public bool esperarIdle = true;
    public string estadoIdle = "Iddle";
    public float esperaMaximaIdle = 1.5f;

    [Header("Gameplay")]
    public bool pausarAlGanar = false;

    private bool activado;
    private bool esperando;
    private Coroutine rutinaEsperando;
    private float tiempoEnLaPuerta;

    // PlayerPhysics lo lee para bloquear el movimiento del jugador desde que llega a la meta.
    public static bool MetaAlcanzada { get; private set; }

    private void Start()
    {
        MetaAlcanzada = false;

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (activado || esperando || !activarPorContacto) return;
        if (!other.CompareTag(tagJugador)) return;

        if (!EstaApoyado(other))
        {
            tiempoEnLaPuerta = 0f;
            return;
        }

        tiempoEnLaPuerta += Time.deltaTime;

        if (!EstaEnReposo(other) && tiempoEnLaPuerta < esperaMaximaIdle) return;

        esperando = true;
        MetaAlcanzada = true;

       

        rutinaEsperando = StartCoroutine(EsperarReposoYMostrar());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagJugador)) tiempoEnLaPuerta = 0f;
    }

    private bool EstaApoyado(Collider2D other)
    {
        if (!esperarAterrizaje) return true;

        // La velocidad vertical tambien pasa por cero en el pico del salto, asi que
        // cuando el jugador tiene PlayerPhysics se le pregunta si toca el piso de verdad.
        PlayerPhysics fisica = other.GetComponentInParent<PlayerPhysics>();

        if (fisica != null) return fisica.EstaEnElSuelo;

        Rigidbody2D rb = other.attachedRigidbody;

        return rb == null || Mathf.Abs(rb.linearVelocity.y) <= velocidadVerticalMaxima;
    }

    private bool EstaEnReposo(Collider2D other)
    {
        if (!esperarIdle) return true;

        Animator anim = other.GetComponentInChildren<Animator>();

        return anim == null || anim.GetCurrentAnimatorStateInfo(0).IsName(estadoIdle);
    }

    private IEnumerator EsperarReposoYMostrar()
    {
        if (dialogoFinal != null)
        {
            bool dialogoTerminado = false;
            System.Action alTerminar = () => dialogoTerminado = true;

            dialogoFinal.AlTerminarDialogo += alTerminar;

            dialogoFinal.Interactuar();

            yield return new WaitUntil(() => dialogoTerminado);

            dialogoFinal.AlTerminarDialogo -= alTerminar;
        }

        esperando = false;
        MostrarVictoria();
    }

    /// <summary>
    /// Función ejecutada al hacer clic en el botón SKIP
    /// </summary>
    public void SaltarDialogoYMostrarVictoria()
    {
        // 1. Detiene la corrutina en ejecución si existía
        if (rutinaEsperando != null)
        {
            StopCoroutine(rutinaEsperando);
            rutinaEsperando = null;
        }

        // 2. Apaga el diálogo si está activo
        if (dialogoFinal != null)
        {
            dialogoFinal.TerminarDialog();
        }

        esperando = false;

        // 3. Muestra la pantalla de victoria al instante
        MostrarVictoria();
    }

    public void MostrarVictoria()
    {
        if (activado) return;

        activado = true;
        MetaAlcanzada = true;

        // =========================================================
        // AQUÍ SE EJECUTA EL SONIDO EN CUANTO TOCA LA PUERTA Y ATERRIZA
        // =========================================================
        if (sonidoVictoria != null)
        {
            StartCoroutine(ReproducirSonidoVictoria());
        }

        PlayerPrefs.SetInt(claveNivel, 1);
        PlayerPrefs.Save();

        MostrarResultados();

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);

            // La caja de dialogo esta despues en la jerarquia y le tapaba los botones.
            panelVictoria.transform.SetAsLastSibling();
        }

        NavegacionMenus.SeleccionarDentroDe(panelVictoria);

        if (pausarAlGanar)
        {
            Time.timeScale = 0f;
        }
    }

    private IEnumerator ReproducirSonidoVictoria()
    {
        if (respetarMute && PlayerPrefs.GetInt("MusicMuted", 0) == 1) yield break;

        float arranque = Mathf.Clamp(inicioClip, 0f, Mathf.Max(0f, sonidoVictoria.length - 0.05f));
        float desvanece = Mathf.Max(0.01f, desvanecido);
        float total = Mathf.Min(duracionMaxima, sonidoVictoria.length - arranque);

        GameObject objeto = new GameObject("SFX Victoria");
        AudioSource fuente = objeto.AddComponent<AudioSource>();

        fuente.clip = sonidoVictoria;
        fuente.volume = volumenVictoria;
        fuente.spatialBlend = 0f;
        fuente.playOnAwake = false;
        fuente.time = arranque;
        fuente.Play();

        yield return new WaitForSecondsRealtime(Mathf.Max(0f, total - desvanece));

        float transcurrido = 0f;

        while (transcurrido < desvanece && fuente != null)
        {
            transcurrido += Time.unscaledDeltaTime;
            fuente.volume = Mathf.Lerp(volumenVictoria, 0f, transcurrido / desvanece);
            yield return null;
        }

        if (objeto != null) Destroy(objeto);
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
        MetaAlcanzada = false;

        if (pausarAlGanar && activado)
        {
            Time.timeScale = 1f;
        }
    }
}