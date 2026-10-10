using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

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

    [Header("Luz de visibilidad al morir")]
    [Tooltip("Tag de la luz que revela al jugador en la oscuridad (la misma que usa LuXVisibilidad). Si esta escena no tiene esa luz, se ignora.")]
    public string tagLuzVisibilidad = "IVisibilidadJugador";
    [Tooltip("Segundos que tarda en apagarse gradualmente esa luz al morir, en vez de cortarse de golpe.")]
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

    // Color real (sin flash de daño) del sprite i del rig. PlayerPhysics lo usa al activar
    // el Phase para no guardar el rojo del impacto como si fuera el color normal.
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
        if (muerto || esInvulnerable || (playerPhysics != null && playerPhysics.EstaEnPhase))
            return;
        //if (muerto || esInvulnerable) return;

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

    public void Morir()
    {
        if (muerto) return;

        cantidadDeVida = 0;
        muerto = true;

        if (anim != null)
            anim.SetTrigger("Muerte");

        if (luzVisibilidad != null && luzVisibilidad.enabled)
            StartCoroutine(ApagarLuzVisibilidadGradualmente());

        StartCoroutine(ReiniciarJuego());
    }

    // Apaga de a poco la luz que revela al jugador en la oscuridad (en vez de
    // que quede prendida o se corte de golpe cuando cae/muere). No toca su
    // "enabled": solo baja la intensidad, así no interfiere con LuXVisibilidad.
    private IEnumerator ApagarLuzVisibilidadGradualmente()
    {
        float intensidadInicial = luzVisibilidad.intensity;
        float t = 0f;

        // Tiempo real: el apagado sigue viéndose aunque el juego se pause
        // (retrasoReinicio) o Time.timeScale cambie mientras se ejecuta.
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

    // ---- Muerte en el aire: el personaje termina de caer ----

    // Distancia máxima al suelo para considerar que el personaje está apoyado.
    private const float ToleranciaApoyo = 0.08f;
    // Alto de la caja fina con la que se detecta el suelo (pies) y el techo (cabeza).
    private const float AltoCajaContacto = 0.05f;

    private readonly RaycastHit2D[] impactosContacto = new RaycastHit2D[8];

    private ContactFilter2D FiltroSuperficies()
    {
        ContactFilter2D filtro = new ContactFilter2D();
        filtro.useTriggers = false;
        filtro.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        return filtro;
    }

    // Distancia hasta la superficie sólida más cercana (suelo hacia abajo, techo hacia arriba),
    // o -1 si no hay ninguna dentro de 'distancia'. Usa una caja fina pegada a los pies o a la cabeza,
    // un poco más angosta que el personaje para no rozar las paredes.
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

            // La superficie tiene que mirar hacia el personaje: un piso hacia arriba, un techo hacia abajo.
            // Si ya lo está tocando o se superpone un poco (distancia 0, como cuando está apoyado) la
            // normal no es confiable y se acepta igual.
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

    // Sigue la caída con la misma gravedad que tiene el personaje (la de subida mientras sube y la de
    // caída después) hasta apoyarse en el suelo, o hasta que se acabe 'tiempoMaximo' (por ejemplo si
    // no hay suelo debajo). Cae derecho: sin avance horizontal.
    private IEnumerator TerminarDeCaer(Bounds limites, float velocidadY, float gravedadSubida, float gravedadCaida, Collider2D propio, float tiempoMaximo)
    {
        float inicio = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - inicio < tiempoMaximo)
        {
            float dt = Time.deltaTime;

            // Con el juego en pausa (Time.timeScale = 0) se queda quieto.
            if (dt > 0f)
            {
                velocidadY += Physics2D.gravity.y * (velocidadY > 0f ? gravedadSubida : gravedadCaida) * dt;
                float desplazamiento = velocidadY * dt;

                if (desplazamiento < 0f)
                {
                    float suelo = DistanciaASuperficie(limites, true, -desplazamiento, propio);

                    if (suelo >= 0f)
                    {
                        // Aterrizó: queda apoyado en el suelo.
                        transform.position += Vector3.down * suelo;
                        yield break;
                    }
                }
                else if (desplazamiento > 0f)
                {
                    float techo = DistanciaASuperficie(limites, false, desplazamiento, propio);

                    if (techo >= 0f)
                    {
                        // Pegó contra un techo: deja de subir y empieza a caer.
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

        // Cómo venía el personaje al morir (se guarda antes de congelarlo): si muere en el aire,
        // termina de caer antes de quedarse quieto.
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

        // Desactiva los demás scripts de control para congelar las acciones del personaje
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
                script.enabled = false;
        }

        float espera = Mathf.Max(retrasoReinicio, 1.5f);
        float inicio = Time.realtimeSinceStartup;

        // Muerte en el aire: termina de caer mientras corre la animación de muerte. Sigue sin collider
        // (como antes), así que mientras cae no activa nada del nivel.
        if (col != null && !ApoyadoEnElSuelo(limites, velocidadY, col))
        {
            float gravedadCaida = playerPhysics != null ? playerPhysics.MultiplicadorCaida : gravedadSubida;

            yield return StartCoroutine(TerminarDeCaer(limites, velocidadY, gravedadSubida, gravedadCaida, col, espera));
        }

        // Espera en tiempo real para que se ejecute la animación de muerte
        // (el tiempo de la caída cuenta dentro de la espera: el panel aparece igual que antes).
        float restante = espera - (Time.realtimeSinceStartup - inicio);

        if (restante > 0f)
            yield return new WaitForSecondsRealtime(restante);

        // Le pasa el control al PanelDerrota de la escena
        PanelDerrota panel = FindFirstObjectByType<PanelDerrota>();
        if (panel != null)
        {
            panel.IniciarSecuenciaDerrota();
        }
        else
        {
            // Respaldo por si una escena no tiene PanelDerrota
            Scene escenaActual = SceneManager.GetActiveScene();
            SceneManager.LoadScene(escenaActual.buildIndex);
        }
    }
}