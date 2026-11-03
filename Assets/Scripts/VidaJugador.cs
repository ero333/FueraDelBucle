using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using Unity.Services.Analytics;

public class VidaJugador : MonoBehaviour
{
    public int cantidadDeVida = 3;

    [Header("Feedback de impacto")]
    public Color colorImpacto = Color.red;
    public float duracionImpacto = 1f;
    [Tooltip("Opacidad del flash rojo cuando te golpean mientras estás en modo Phase.")]
    [Range(0f, 1f)]
    public float opacidadImpactoPhase = 0.5f;

    [Header("Invulnerabilidad")]
    public float duraciondeinvulnerabilidad = 1.5f;
    private bool esInvulnerable = false;

    [Header("Game Over")]
    [Tooltip("Segundos de espera antes de mostrar el panel/diálogo de derrota al quedarse sin vida.")]
    public float retrasoReinicio = 1.5f;

    [Header("Sonido")]
    public AudioClip sonidoMuerte;
    [Range(0f, 1f)] public float volumenMuerte = 1f;
    private AudioSource audioSource;

    [Header("Luz de visibilidad al morir")]
    public string tagLuzVisibilidad = "IVisibilidadJugador";
    public float duracionApagadoLuz = 1f;

    private Light2D luzVisibilidad;

    private SpriteRenderer[] spriteRenderers;
    private Color[] coloresOriginales;
    private Coroutine parpadeoActual;
    private Coroutine invulnerabilidadActual;
    private bool muerto;

    private Animator anim;
    private PlayerPhysics playerPhysics;

    public bool EstaParpadeando => parpadeoActual != null;

    public Color ColorOriginal(int indice, Color porDefecto)
    {
        if (coloresOriginales == null || indice < 0 || indice >= coloresOriginales.Length)
            return porDefecto;

        return coloresOriginales[indice];
    }

    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        playerPhysics = GetComponent<PlayerPhysics>();
        audioSource = GetComponent<AudioSource>();

        coloresOriginales = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            coloresOriginales[i] = spriteRenderers[i].color;
        }

        GameObject objLuz = GameObject.FindGameObjectWithTag(tagLuzVisibilidad);
        if (objLuz != null)
        {
            luzVisibilidad = objLuz.GetComponent<Light2D>();
        }
    }

    public void TomarDaño(int daño)
    {
        if (PlayerPhysics.godModeActivo) return;
        if (muerto || esInvulnerable || (playerPhysics != null && playerPhysics.EstaEnPhase))
            return;

        cantidadDeVida -= daño;

        if (cantidadDeVida <= 0)
        {
            Morir();
            return;
        }

        if (parpadeoActual != null)
            StopCoroutine(parpadeoActual);

        Color colorFlash = colorImpacto;
        if (playerPhysics != null && playerPhysics.EstaEnPhase)
        {
            colorFlash.a = opacidadImpactoPhase;
        }

        parpadeoActual = StartCoroutine(ParpadeoImpacto(colorFlash));
        if (invulnerabilidadActual != null) StopCoroutine(invulnerabilidadActual);
        invulnerabilidadActual = StartCoroutine(Invulnerabilidad());
    }

    public static event System.Action JugadorMurio;

 public void Morir()
    {
        if (muerto) return;

        cantidadDeVida = 0;
        muerto = true;

        System.Action aviso = JugadorMurio;
        if (aviso != null)
        {
            aviso();
        }

        CustomEvent gameOverEvent = new CustomEvent("GameOver")
        {
            { "Level", 1 },
            { "MusicActive", true },
            { "SoundActive", true },
            { "TimeElapsed", 120.5f },
            { "PhaseAvoidances", 0 },
            { "PushCount", 0 },
            { "KillCount", 0 },
            { "KillerEntity", "Pinchos" }
        };

        AnalyticsService.Instance.RecordEvent(gameOverEvent);
        AnalyticsService.Instance.Flush();

        if (sonidoMuerte != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(sonidoMuerte, Volumen.DeEfecto(volumenMuerte));
            else
                Volumen.SonarEfecto(sonidoMuerte, volumenMuerte);
        }

        if (anim != null)
            anim.SetTrigger("Muerte");

        if (luzVisibilidad != null && luzVisibilidad.enabled)
            StartCoroutine(ApagarLuzVisibilidadGradualmente());

        StartCoroutine(ReiniciarJuego());
    }
    private IEnumerator ApagarLuzVisibilidadGradualmente()
    {
        float intensidadInicial = luzVisibilidad.intensity;
        float t = 0f;

        while (t < duracionApagadoLuz)
        {
            t += Time.unscaledDeltaTime;

            if (luzVisibilidad == null) yield break;

            luzVisibilidad.intensity = Mathf.Lerp(intensidadInicial, 0f, t / duracionApagadoLuz);

            yield return null;
        }

        luzVisibilidad.intensity = 0f;
    }

    private IEnumerator ParpadeoImpacto(Color colorFlash)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = colorFlash;
        }

        yield return new WaitForSeconds(duracionImpacto);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = coloresOriginales[i];
        }

        parpadeoActual = null;
    }

    private IEnumerator Invulnerabilidad()
    {
        esInvulnerable = true;
        yield return new WaitForSeconds(duraciondeinvulnerabilidad);
        esInvulnerable = false;
        invulnerabilidadActual = null;
    }

    private const float ToleranciaApoyo = 0.08f;
    private const float AltoCajaContacto = 0.05f;
    private const float MargenSuperficie = 0.15f;
    private readonly RaycastHit2D[] impactosContacto = new RaycastHit2D[8];

    private ContactFilter2D FiltroSuperficies()
    {
        ContactFilter2D filtro = new ContactFilter2D();
        filtro.useTriggers = false;

        bool hayCapaPlataformas = playerPhysics != null && playerPhysics.CapaPlataformas.value != 0;
        filtro.SetLayerMask(hayCapaPlataformas ? playerPhysics.CapaPlataformas : Physics2D.GetLayerCollisionMask(gameObject.layer));
        return filtro;
    }

    private float DistanciaASuperficie(Bounds limites, bool haciaAbajo, float distancia, Collider2D propio)
    {
        Vector2 tamano = new Vector2(limites.size.x * 0.9f, AltoCajaContacto);
        float y = haciaAbajo ? limites.min.y + AltoCajaContacto * 0.5f : limites.max.y - AltoCajaContacto * 0.5f;
        Vector2 centro = new Vector2(limites.center.x, y);
        Vector2 direccion = haciaAbajo ? Vector2.down : Vector2.up;

        int cantidad = Physics2D.BoxCast(centro, tamano, 0f, direccion, FiltroSuperficies(), impactosContacto, distancia);

        float masCercana = -1f;

        for (int i = 0; i < cantidad; i++)
        {
            RaycastHit2D impacto = impactosContacto[i];

            if (impacto.collider == null || impacto.collider == propio || impacto.collider.transform.IsChildOf(transform))
                continue;

            Bounds otro = impacto.collider.bounds;

            if (haciaAbajo ? otro.max.y > limites.min.y + MargenSuperficie : otro.min.y < limites.max.y - MargenSuperficie)
                continue;

            bool yaTocando = impacto.distance <= 0.001f;

            if (!yaTocando && (haciaAbajo ? impacto.normal.y < 0.3f : impacto.normal.y > -0.3f))
                continue;

            if (masCercana < 0f || impacto.distance < masCercana)
                masCercana = impacto.distance;
        }

        return masCercana;
    }

    private bool ApoyadoEnElSuelo(Bounds limites, float velocidadY, Collider2D propio)
    {
        return velocidadY <= 0.1f && DistanciaASuperficie(limites, true, ToleranciaApoyo, propio) >= 0f;
    }

    private IEnumerator TerminarDeCaer(Bounds limites, float velocidadY, float gravedadSubida, float gravedadCaida, Collider2D propio, float tiempoMaximo)
    {
        float inicio = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - inicio < tiempoMaximo)
        {
            float dt = Time.deltaTime;

            if (dt > 0f)
            {
                velocidadY += Physics2D.gravity.y * (velocidadY > 0f ? gravedadSubida : gravedadCaida) * dt;
                float desplazamiento = velocidadY * dt;

                if (desplazamiento < 0f)
                {
                    float suelo = DistanciaASuperficie(limites, true, -desplazamiento, propio);

                    if (suelo >= 0f)
                    {
                        transform.position += Vector3.down * suelo;
                        yield break;
                    }
                }
                else if (desplazamiento > 0f)
                {
                    float techo = DistanciaASuperficie(limites, false, desplazamiento, propio);

                    if (techo >= 0f)
                    {
                        desplazamiento = techo;
                        velocidadY = 0f;
                    }
                }

                transform.position += Vector3.up * desplazamiento;
                limites.center += Vector3.up * desplazamiento;
            }

            yield return null;
        }
    }

    private IEnumerator ReiniciarJuego()
    {
        if (parpadeoActual != null)
            StopCoroutine(parpadeoActual);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = coloresOriginales[i];
        }

        var rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();

        float velocidadY = rb != null ? rb.linearVelocity.y : 0f;
        float gravedadSubida = rb != null ? rb.gravityScale : 1f;
        Bounds limites = col != null ? col.bounds : new Bounds();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (col != null)
            col.enabled = false;

        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
                script.enabled = false;
        }

        float espera = Mathf.Max(retrasoReinicio, 1.5f);
        float inicio = Time.realtimeSinceStartup;

        if (col != null && !ApoyadoEnElSuelo(limites, velocidadY, col))
        {
            float gravedadCaida = playerPhysics != null ? playerPhysics.MultiplicadorCaida : gravedadSubida;

            yield return StartCoroutine(TerminarDeCaer(limites, velocidadY, gravedadSubida, gravedadCaida, col, espera));
        }

        float restante = espera - (Time.realtimeSinceStartup - inicio);

        if (restante > 0f)
            yield return new WaitForSecondsRealtime(restante);

        PanelDerrota panel = FindFirstObjectByType<PanelDerrota>();
        if (panel != null)
        {
            panel.IniciarSecuenciaDerrota();
        }
        else
        {
            Scene escenaActual = SceneManager.GetActiveScene();
            Time.timeScale = 1f;
            SceneManager.LoadScene(escenaActual.buildIndex);
        }
    }
}