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

    private SpriteRenderer[] spriteRenderers;
    private Color[] coloresOriginales;
    private Coroutine parpadeoActual;
    private bool muerto;

    void Start()
    {
        
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

       
        coloresOriginales = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            coloresOriginales[i] = spriteRenderers[i].color;
        }
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

        if (parpadeoActual != null)
            StopCoroutine(parpadeoActual);

        parpadeoActual = StartCoroutine(ParpadeoImpacto());
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

    private IEnumerator ReiniciarJuego()
    {
        if (parpadeoActual != null)
            StopCoroutine(parpadeoActual);

       
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
                script.enabled = false;
        }

        
        var rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

       
        var col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

       
        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = false;
        }

        yield return new WaitForSeconds(retrasoReinicio);

        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.buildIndex);
    }

    void Update() { }
}