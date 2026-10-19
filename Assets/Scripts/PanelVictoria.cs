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

    [Header("Animación Final del Personaje (Solo Nivel 10)")]
    [Tooltip("Marca esta casilla SOLO en el Nivel 10 para que haga la animación de disolución antes de mostrar la victoria.")]
    public bool animarAnimacionEnEsteNivel = false;

    [Tooltip("Arrastra el Transform del centro de la puerta o portal.")]
    public Transform centroPortal;

    [Tooltip("Duración en segundos de la disolución/movimiento hacia la puerta.")]
    public float duracionAnimacionFinal = 1.8f;

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

        rutinaEsperando = StartCoroutine(EsperarReposoYMostrar(other.gameObject));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagJugador)) tiempoEnLaPuerta = 0f;
    }

    private bool EstaApoyado(Collider2D other)
    {
        if (!esperarAterrizaje) return true;

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

    private IEnumerator EsperarReposoYMostrar(GameObject jugadorObj)
    {
        // 1. Si hay diálogo, esperamos a que el jugador lo termine
        if (dialogoFinal != null)
        {
            bool dialogoTerminado = false;
            System.Action alTerminar = () => dialogoTerminado = true;

            dialogoFinal.AlTerminarDialogo += alTerminar;
            dialogoFinal.Interactuar();

            yield return new WaitUntil(() => dialogoTerminado);

            dialogoFinal.AlTerminarDialogo -= alTerminar;
        }

        // 2. SOLO SI ESTÁ ACTIVADA LA CASILLA (Nivel 10), hace la animación
        if (animarAnimacionEnEsteNivel)
        {
            yield return StartCoroutine(AnimarDesaparicionJugador(jugadorObj));
        }

        esperando = false;

        // 3. Mostramos la victoria
        MostrarVictoria();
    }

    private IEnumerator AnimarDesaparicionJugador(GameObject jugador)
    {
        if (jugador == null) yield break;

        // Desactivar controles del jugador
        var controller = jugador.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        var rb2d = jugador.GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.simulated = false;

        Renderer rend = jugador.GetComponentInChildren<Renderer>();
        Material mat = rend != null ? rend.material : null;

        Vector3 posInicial = jugador.transform.position;
        Vector3 escalaInicial = jugador.transform.localScale;
        Vector3 posDestino = centroPortal != null ? centroPortal.position : posInicial;

        float t = 0f;

        while (t < duracionAnimacionFinal)
        {
            t += Time.deltaTime;
            float progreso = Mathf.Clamp01(t / duracionAnimacionFinal);

            // Mover al centro de la puerta
            jugador.transform.position = Vector3.Lerp(posInicial, posDestino, progreso);

            // Reducir escala (encoger)
            jugador.transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, progreso);

            // Disolver Shader
            if (mat != null)
            {
                mat.SetFloat("_DissolveAmount", progreso);
            }

            yield return null;
        }

        // Ocultar al jugador al finalizar la animación
        jugador.SetActive(false);
    }

    /// <summary>
    /// Función ejecutada al hacer clic en el botón SKIP
    /// </summary>
    public void SaltarDialogoYMostrarVictoria()
    {
        if (rutinaEsperando != null)
        {
            StopCoroutine(rutinaEsperando);
            rutinaEsperando = null;
        }

        if (dialogoFinal != null)
        {
            dialogoFinal.TerminarDialog();
        }

        esperando = false;
        MostrarVictoria();
    }

    public void MostrarVictoria()
    {
        if (activado) return;

        activado = true;
        MetaAlcanzada = true;

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
        if (respetarMute && Volumen.Efectos <= 0f) yield break;

        float arranque = Mathf.Clamp(inicioClip, 0f, Mathf.Max(0f, sonidoVictoria.length - 0.05f));
        float desvanece = Mathf.Max(0.01f, desvanecido);
        float total = Mathf.Min(duracionMaxima, sonidoVictoria.length - arranque);

        GameObject objeto = new GameObject("SFX Victoria");
        AudioSource fuente = objeto.AddComponent<AudioSource>();

        fuente.clip = sonidoVictoria;
        fuente.volume = Volumen.DeEfecto(volumenVictoria);
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