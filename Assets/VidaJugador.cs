using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    public int cantidadDeVida;

    [Header("Feedback de impacto")]
    public Color colorImpacto = Color.red;
    public float duracionImpacto = 1f;

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

    void Start()
    {
        
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        anim = GetComponent<Animator>();
       
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

        parpadeoActual = StartCoroutine(ParpadeoImpacto());
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

    private IEnumerator ParpadeoImpacto()
    {
        
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = colorImpacto;
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