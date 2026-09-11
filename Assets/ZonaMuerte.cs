using UnityEngine;
using UnityEngine.SceneManagement;

public class ZonaMuerte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        VidaJugador vida = collision.GetComponentInParent<VidaJugador>();

        if (vida != null)
        {
            // Mismo flujo que morir por daño: animación de muerte + PanelDerrota.
            vida.Morir();
        }
        else
        {
            // Respaldo por si el jugador no tiene VidaJugador.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
