using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioNivel : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Nivel2")]
    public string nombreEscenaSiguiente;

    [Tooltip("Tag que debe tener el jugador para activar el cambio de nivel.")]
    public string tagJugador = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagJugador))
        {
            SceneManager.LoadScene(nombreEscenaSiguiente);
        }
    }
}
