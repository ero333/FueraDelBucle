using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class BotonFinal : MonoBehaviour
{
    [Header("Escena")]
    public string nombreEscenaSiguiente = "Menu";

    [Header("Jugador")]
    public MonoBehaviour controlJugador;

    [Header("Cartel de la tecla")]
    public GameObject carteldeE;

    private bool yaInteractuo = false;
    private bool jugadorCerca = false;
    private float tiempoHabilitado;

    private void Start()
    {
        MostrarCartel(false);
    }

    private void Update()
    {
        bool disponible = PuedeInteractuar();

        if (!disponible)
        {
            tiempoHabilitado = Time.unscaledTime + 0.15f;
        }

        MostrarCartel(disponible && jugadorCerca && !yaInteractuo);

        if (!disponible) return;

        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Interactuar();
        }
    }

    private void MostrarCartel(bool visible)
    {
        if (carteldeE != null && carteldeE.activeSelf != visible)
        {
            carteldeE.SetActive(visible);
        }
    }

    private bool PuedeInteractuar()
    {
        if (PauseManager.GameIsPaused || LevelUIManager.PlacaAbierta || DialogoLog.HayDialogoActivo) return false;

        if (Time.timeScale == 0f) return false;

        return Time.unscaledTime >= tiempoHabilitado;
    }

    private void OnMouseDown()
    {
        Interactuar();
    }

    private void Interactuar()
    {
        if (yaInteractuo) return;

        yaInteractuo = true;

        MostrarCartel(false);

        if (controlJugador != null)
        {
            controlJugador.enabled = false;
        }

        CargarEscenaSiguiente();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    public void CargarEscenaSiguiente()
    {
        if (!string.IsNullOrEmpty(nombreEscenaSiguiente))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nombreEscenaSiguiente);
        }
        else
        {
            Debug.LogError("No especificaste el nombre de la escena en el Inspector.");
        }
    }
}
