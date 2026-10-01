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
            bool visible = false;

            foreach (SpriteRenderer sprite in sprites)
            {
                if (sprite != null && sprite.isVisible)
                {
                    visible = true;
                    break;
                }
            }

            if (!visible) break;

            tiempoEsperado += Time.deltaTime;
            yield return null;
        }

        vida.Morir();
    }
}
