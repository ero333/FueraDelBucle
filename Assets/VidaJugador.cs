using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    public int cantidadDeVida;

    [Header("Feedback de impacto")]
    public Color colorImpacto = Color.red;
    public float duracionImpacto = 1f;
    [Tooltip("Opacidad del flash rojo cuando te golpean mientras estás en modo Phase (para diferenciarlo del golpe normal, que usa el rojo a opacidad completa).")]
    [Range(0f, 1f)]
    public float opacidadImpactoPhase = 0.5f;

    [Header("Invulnerabilidad")]
    public float duraciondeinvulnerabilidad = 1.5f;
    private bool esInvulnerable = false;
   
    [Header("Game Over")]
    [Tooltip("Segundos de espera antes de reiniciar la escena al quedarse sin vida (mínimo 1.5s).")]
    public float retrasoReinicio = 1.5f;

    private SpriteRenderer[] spriteRenderers;
    private Color[] coloresOriginales;
    private Coroutine parpadeoActual;
    private Coroutine invulnerabilidadActual;
    private bool muerto;

    private Animator anim;
    private PlayerPhysics playerPhysics;

    // Expuesto para que PlayerPhysics sepa cuándo hay un flash de daño en
    // curso y no lo pise con el pulso del glow de Phase.
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
        if (muerto || esInvulnerable) return;

        cantidadDeVida -= daño;

       
        

        if (cantidadDeVida <= 0)
        {
            Morir();
            return;
        }

        if (parpadeoActual != null)
            StopCoroutine(parpadeoActual);

        // En modo Phase el flash es rojo pero semi-transparente, para
        // distinguirlo del golpe normal (rojo a opacidad completa).
        Color colorFlash = colorImpacto;
        if (playerPhysics != null && playerPhysics.EstaEnPhase)
        {
            colorFlash.a = opacidadImpactoPhase;
        }

        parpadeoActual = StartCoroutine(ParpadeoImpacto(colorFlash));
        if (invulnerabilidadActual != null) StopCoroutine(invulnerabilidadActual);
        invulnerabilidadActual = StartCoroutine(Invulnerabilidad());
    }

    // Muerte instantánea, ignora la invulnerabilidad. La usa ZonaMuerte cuando
    // el jugador se cae del mapa, para que pase por el mismo flujo (animación de
    // muerte + PanelDerrota) en vez de recargar la escena de golpe.
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

        // Desactiva TODOS los demás scripts de una (movimiento, dash, disparo...)
        // para que el jugador quede congelado apenas muere.
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
                script.enabled = false;
        }

        // Recién ahora se espera (una sola vez) para que se vea la animación de
        // muerte, y después se reinicia. Mínimo 1.5s aunque el campo serializado
        // en las escenas haya quedado en 0.5.
        yield return new WaitForSeconds(Mathf.Max(retrasoReinicio, 1.5f));

        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.buildIndex);
    }

   
    void Update() { }
}