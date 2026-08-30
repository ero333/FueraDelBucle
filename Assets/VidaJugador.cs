using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    public int cantidadDeVida;

    [Header("Feedback de impacto")]
    public Color colorImpacto = Color.red;
    public float duracionImpacto = 1f;  

    [Header("Game Over")]
    [Tooltip("Segundos de espera antes de reiniciar la escena al quedarse sin vida.")]
    public float retrasoReinicio = 1f;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;
    private Coroutine parpadeoActual;
    private bool muerto;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            colorOriginal = spriteRenderer.color;
    }

    public void TomarDaño(int daño)
    {
        if (muerto) return;

        cantidadDeVida -= daño;

        if (cantidadDeVida <= 0)
        {
            cantidadDeVida = 0;
            muerto = true;
            StartCoroutine(ReiniciarJuego());
            return;
        }

        if (spriteRenderer != null)
        {
            if (parpadeoActual != null)
                StopCoroutine(parpadeoActual);  
            parpadeoActual = StartCoroutine(ParpadeoImpacto());
        }
    }

    private IEnumerator ParpadeoImpacto()
    {
        spriteRenderer.color = colorImpacto;
        yield return new WaitForSeconds(duracionImpacto);
        spriteRenderer.color = colorOriginal;
        parpadeoActual = null;
    }

    private IEnumerator ReiniciarJuego()
    {
       
        if (parpadeoActual != null)
            StopCoroutine(parpadeoActual);

       
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
                script.enabled = false;
        }

        // ...y congela la física para que no siga desplazándose ni cayendo.
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        yield return new WaitForSeconds(retrasoReinicio);

        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.buildIndex);
    }

    void Update() { }
}
