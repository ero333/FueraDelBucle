using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private SpriteRenderer[] spriteRenderers;
    private Color[] coloresOriginales;
    private Coroutine parpadeoActual;
    private Coroutine invulnerabilidadActual;
    private bool muerto;

    private Animator anim;
    private PlayerPhysics playerPhysics;

    public bool EstaParpadeando => parpadeoActual != null;

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

        StartCoroutine(ReiniciarJuego());
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

    private IEnumerator ReiniciarJuego()
    {
        if (parpadeoActual != null)
            StopCoroutine(parpadeoActual);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = coloresOriginales[i];
        }

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Desactiva los demás scripts de control para congelar las acciones del personaje
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
                script.enabled = false;
        }

        // Espera en tiempo real para que se ejecute la animación de muerte
        yield return new WaitForSecondsRealtime(Mathf.Max(retrasoReinicio, 1.5f));

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