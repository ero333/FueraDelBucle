using UnityEngine;

public class DialogoAparicion : MonoBehaviour
{
    [Header("Que dialogo se abre")]
    public DialogoLog dialogo;

    [Header("Personaje que aparece")]
    public GameObject personaje;
    public string tagJugador = "Player";

    [Header("Cuando se dispara")]
    [Tooltip("Distancia a la que el jugador tiene que acercarse al personaje.")]
    public float distancia = 12f;

    [Tooltip("Segundos de espera antes de poder dispararse, para que el personaje termine de aparecer.")]
    public float esperaInicial = 1f;

    public bool soloUnaVez = true;

    private Transform jugador;
    private bool yaSeMostro;
    private float tiempoInicio;

    private void Start()
    {
        tiempoInicio = Time.unscaledTime;
        BuscarJugador();
    }

    private void Update()
    {
        if (yaSeMostro && soloUnaVez) return;
        if (dialogo == null || personaje == null) return;
        if (NotaDeObjetivoAbierta() || PauseManager.GameIsPaused || Time.timeScale == 0f)
        {
            tiempoInicio = Time.unscaledTime;
            return;
        }

        if (Time.unscaledTime < tiempoInicio + esperaInicial) return;
        if (!personaje.activeInHierarchy) return;

        if (jugador == null)
        {
            BuscarJugador();
            if (jugador == null) return;
        }

        float separacion = Vector2.Distance(jugador.position, personaje.transform.position);
        if (separacion > distancia) return;

        yaSeMostro = true;
        dialogo.Interactuar();
    }

    private bool NotaDeObjetivoAbierta()
    {
        return LevelUIManager.PlacaAbierta;
    }

    private void BuscarJugador()
    {
        GameObject objetivo = GameObject.FindGameObjectWithTag(tagJugador);
        if (objetivo != null) jugador = objetivo.transform;
    }

    private void OnDrawGizmosSelected()
    {
        if (personaje == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(personaje.transform.position, distancia);
    }
}
