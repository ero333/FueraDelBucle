using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonFinal : MonoBehaviour
{
    [Header("Escena")]
    public string nombreEscenaSiguiente = "AnimFinal";

    [Header("Jugador")]
    public Transform jugador;
    public MonoBehaviour controlJugador;
    public float distanciaInteraccion = 2.5f;

    [Header("Cartel de la tecla")]
    public GameObject carteldeE;

    private bool yaInteractuo = false;

    private void Start()
    {
        MostrarCartel(false);

        if (jugador == null && controlJugador != null)
        {
            jugador = controlJugador.transform;
        }
    }

    private void Update()
    {
        bool disponible = PuedeInteractuar();

        bool jugadorCerca = false;
        if (jugador != null)
        {
            float distancia = Vector2.Distance(transform.position, jugador.position);
            jugadorCerca = distancia <= distanciaInteraccion;
        }

        MostrarCartel(disponible && jugadorCerca && !yaInteractuo);

        if (disponible && jugadorCerca && !yaInteractuo && Input.GetKeyDown(KeyCode.E))
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
        return true;
    }

    private void Interactuar()
    {
        yaInteractuo = true;
        MostrarCartel(false);

        if (controlJugador != null)
        {
            controlJugador.enabled = false;
        }

        CargarEscenaSiguiente();
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