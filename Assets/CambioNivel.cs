using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CambioNivel : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre de la escena a la que se cambiará.")]
    public string nombreEscenaSiguiente;

    [Tooltip("Tag que debe tener el jugador para activar la interacción.")]
    public string tagJugador = "Player";

    [Header("Interfaz de Usuario")]
    [Tooltip("Arrastra aquí el Botón de la UI desde la jerarquía.")]
    public Button botonCambioEscena;

    [Header("Aparición del botón")]
    [Tooltip("Espera a que el jugador aterrice antes de mostrar el botón.")]
    public bool esperarAterrizaje = true;

    [Tooltip("Velocidad vertical por debajo de la cual se considera que ya aterrizó.")]
    public float velocidadVerticalMaxima = 0.5f;

    [Tooltip("Espera a que el Animator entre en el estado de reposo.")]
    public bool esperarIdle = true;

    [Tooltip("Nombre exacto del estado de reposo en el Animator.")]
    public string estadoIdle = "Iddle";

    [Tooltip("Segundos máximos de espera del estado de reposo antes de mostrar igual.")]
    public float esperaMaximaIdle = 1.5f;

    [Header("Transición")]
    [Tooltip("Congela el juego mientras el botón está en pantalla.")]
    public bool pausarAlMostrar = true;

    [Tooltip("Opcional: CanvasGroup de una imagen negra a pantalla completa para el fundido.")]
    public CanvasGroup fundido;

    [Tooltip("Duración del fundido en segundos.")]
    public float duracionFundido = 0.4f;

    private bool cambiando;
    private bool esperando;

    private void Start()
    {
        if (fundido != null)
        {
            fundido.alpha = 0f;
            fundido.blocksRaycasts = false;
        }

        if (botonCambioEscena != null)
        {
            botonCambioEscena.gameObject.SetActive(false);
            botonCambioEscena.onClick.AddListener(CargarSiguienteEscena);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el botón en el Inspector de CambioNivel.");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (cambiando || esperando || botonCambioEscena == null) return;
        if (!other.CompareTag(tagJugador)) return;
        if (botonCambioEscena.gameObject.activeSelf) return;

        if (esperarAterrizaje)
        {
            Rigidbody2D rb = other.attachedRigidbody;

            if (rb != null && Mathf.Abs(rb.linearVelocity.y) > velocidadVerticalMaxima) return;
        }

        esperando = true;
        StartCoroutine(EsperarReposoYMostrar(other));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (cambiando || pausarAlMostrar) return;

        if (other.CompareTag(tagJugador) && botonCambioEscena != null)
        {
            botonCambioEscena.gameObject.SetActive(false);
        }
    }

    private IEnumerator EsperarReposoYMostrar(Collider2D other)
    {
        if (esperarIdle)
        {
            Animator anim = other.GetComponentInChildren<Animator>();

            if (anim != null)
            {
                float t = 0f;

                while (t < esperaMaximaIdle)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName(estadoIdle)) break;

                    t += Time.deltaTime;
                    yield return null;
                }
            }
        }

        esperando = false;

        if (cambiando) yield break;

        MostrarBoton();
    }

    private void MostrarBoton()
    {
        botonCambioEscena.gameObject.SetActive(true);

        if (pausarAlMostrar)
        {
            Time.timeScale = 0f;
        }
    }

    public void CargarSiguienteEscena()
    {
        if (cambiando) return;

        cambiando = true;

        if (botonCambioEscena != null)
        {
            botonCambioEscena.interactable = false;
        }

        StartCoroutine(Transicion());
    }

    private IEnumerator Transicion()
    {
        if (fundido != null && duracionFundido > 0f)
        {
            fundido.blocksRaycasts = true;

            float t = 0f;

            while (t < duracionFundido)
            {
                t += Time.unscaledDeltaTime;
                fundido.alpha = Mathf.Clamp01(t / duracionFundido);
                yield return null;
            }
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
