using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZonaMuerte : MonoBehaviour
{
    [Tooltip("Segundos máximos a esperar a que el jugador salga de cámara antes de matarlo igual (por si algo impide que se oculte).")]
    public float esperaMaximaFueraDePantalla = 3f;

    private bool procesandoMuerte;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (procesandoMuerte || !collision.CompareTag("Player")) return;

        VidaJugador vida = collision.GetComponentInParent<VidaJugador>();

        if (vida == null)
        {
            // Respaldo por si el jugador no tiene VidaJugador.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        procesandoMuerte = true;
        StartCoroutine(EsperarFueraDePantallaYMorir(vida, collision));
    }

    // No mata al toque: espera a que ningún sprite del jugador se esté
    // renderizando en ninguna cámara (ya se cayó fuera de la vista) antes de
    // ejecutar Morir(). Así la animación de muerte nunca pasa en pantalla, sin
    // depender de ajustar distancias a mano por nivel. Tiene un tope de espera
    // por si algo impidiera que se oculte (cámara sin seguimiento, etc.).
    private IEnumerator EsperarFueraDePantallaYMorir(VidaJugador vida, Collider2D colisionJugador)
    {
        SpriteRenderer[] sprites = colisionJugador.GetComponentsInChildren<SpriteRenderer>();

        float tiempoEsperado = 0f;

        while (tiempoEsperado < esperaMaximaFueraDePantalla)
        {
            if (!EstaEnPantalla(sprites)) break;

            tiempoEsperado += Time.deltaTime;
            yield return null;
        }

        vida.Morir();
    }

    // Mira solo la cámara del juego (Camera.main). SpriteRenderer.isVisible cuenta también
    // la vista Scene del Editor y cualquier otra cámara, y eso hacía esperar el máximo de segundos.
    private bool EstaEnPantalla(SpriteRenderer[] sprites)
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                if (sprite != null && sprite.isVisible) return true;
            }

            return false;
        }

        bool hayLimites = false;
        Bounds limites = new Bounds();

        foreach (SpriteRenderer sprite in sprites)
        {
            if (sprite == null) continue;

            if (!hayLimites)
            {
                limites = sprite.bounds;
                hayLimites = true;
            }
            else
            {
                limites.Encapsulate(sprite.bounds);
            }
        }

        if (!hayLimites) return false;

        Vector3 min = cam.WorldToViewportPoint(limites.min);
        Vector3 max = cam.WorldToViewportPoint(limites.max);

        return max.x > 0f && min.x < 1f && max.y > 0f && min.y < 1f;
    }
}
