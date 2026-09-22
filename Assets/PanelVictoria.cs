using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelVictoria : MonoBehaviour
{
    [Header("Configuración de Progreso")]
    public string claveNivel = "Nivel1";

    [Header("Panel")]
    public GameObject panelVictoria;

    [Header("Resultados")]
    public Text textoTiempo;
    public Text textoVidas;
    public VidaJugador vidaJugador;

    [Header("Diálogo final")]
    public DialogoLog dialogoFinal;

    [Header("Activación")]
    public string tagJugador = "Player";
    public bool activarPorContacto = true;

    [Header("Espera para Lectura Normal (Sin Skip)")]
    public bool esperarAterrizaje = true;
    public float velocidadVerticalMaxima = 0.5f;
    public bool esperarIdle = true;
    public string estadoIdle = "Iddle";
    public float esperaMaximaIdle = 1.5f;

    [Header("Gameplay")]
    public bool pausarAlGanar = false;

    private bool activado;
    private bool esperando;
    private Coroutine rutinaEsperando;

    private void Start()
    {
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (activado || esperando || !activarPorContacto) return;
        if (!other.CompareTag(tagJugador)) return;

        if (esperarAterrizaje)
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null && Mathf.Abs(rb.linearVelocity.y) > velocidadVerticalMaxima) return;
        }

        esperando = true;
        rutinaEsperando = StartCoroutine(EsperarReposoYMostrar(other));
    }

    private IEnumerator EsperarReposoYMostrar(Collider2D other)
    {
        if (esperarIdle)
        {
            Animator anim = other.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                float t = 0f;
                // Usamos Realtime para que no se congele si hay pausas
                while (t < esperaMaximaIdle)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName(estadoIdle)) break;
                    t += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
        }

        if (dialogoFinal != null)
        {
            bool dialogoTerminado = false;
            System.Action alTerminar = () => dialogoTerminado = true;

            dialogoFinal.AlTerminarDialogo += alTerminar;

            dialogoFinal.Interactuar();

            yield return new WaitUntil(() => dialogoTerminado);

            dialogoFinal.AlTerminarDialogo -= alTerminar;
        }

        esperando = false;
        MostrarVictoria();
    }

    /// <summary>
    /// Función ejecutada al hacer clic en el botón SKIP
    /// </summary>
    public void SaltarDialogoYMostrarVictoria()
    {
        // 1. Detiene la corrutina en ejecución si existía
        if (rutinaEsperando != null)
        {
            StopCoroutine(rutinaEsperando);
            rutinaEsperando = null;
        }

        // 2. Apaga el diálogo si está activo
        if (dialogoFinal != null)
        {
            dialogoFinal.TerminarDialog();
        }

        esperando = false;

        // 3. Muestra la pantalla de victoria al instante
        MostrarVictoria();
    }

    public void MostrarVictoria()
    {
        if (activado) return;

        activado = true;

        PlayerPrefs.SetInt(claveNivel, 1);
        PlayerPrefs.Save();

        MostrarResultados();

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }

        if (pausarAlGanar)
        {
            Time.timeScale = 0f;
        }
    }

    private void MostrarResultados()
    {
        if (textoTiempo != null)
        {
            float total = Time.timeSinceLevelLoad;
            int minutos = Mathf.FloorToInt(total / 60f);
            int segundos = Mathf.FloorToInt(total % 60f);
            textoTiempo.text = string.Format("Tiempo: {0:00}:{1:00}", minutos, segundos);
        }

        if (textoVidas != null && vidaJugador != null)
        {
            textoVidas.text = "Vidas restantes: " + vidaJugador.cantidadDeVida;
        }
    }

    private void OnDestroy()
    {
        if (pausarAlGanar && activado)
        {
            Time.timeScale = 1f;
        }
    }
}