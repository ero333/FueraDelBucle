using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutoIniciarDialogoFinal : MonoBehaviour
{
    [Header("Referencias")]
    public DialogoLog sistemaDialogo;
    public Camera camaraPrincipal;

    [Header("Efecto de Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoInicioFinal;

    [Header("Efecto Visual (Flash de Pantalla)")]
    public Image panelFlash;
    public float duracionFlash = 0.5f;

    [Header("Efecto Visual (Cámara Shake)")]
    public bool hacerShakeAlIniciar = true;
    public float duracionShake = 0.8f;
    public float magnitudShake = 0.4f; // Intensidad del temblor

    [Header("Configuración")]
    public float retardoInicial = 0.1f;

    private void Awake()
    {
        // Forzamos que el tiempo del juego corra normalmente por si venía pausado del nivel anterior
        Time.timeScale = 1f;
    }

    private IEnumerator Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;

        yield return new WaitForSecondsRealtime(retardoInicial);

        // 1. Audio
        if (audioSource != null && sonidoInicioFinal != null)
        {
            audioSource.PlayOneShot(sonidoInicioFinal);
        }

        // 2. Flash de Pantalla
        if (panelFlash != null)
        {
            StartCoroutine(EfectoFlashRoutine());
        }

        // 3. Temblor de Cámara
        if (hacerShakeAlIniciar && camaraPrincipal != null)
        {
            StartCoroutine(ShakeCamaraRoutine());
        }

        // 4. Iniciar Diálogo de LOG
        if (sistemaDialogo != null)
        {
            sistemaDialogo.EmpezarDialog();
        }
    }

    private IEnumerator EfectoFlashRoutine()
    {
        panelFlash.gameObject.SetActive(true);
        Color col = panelFlash.color;
        col.a = 1f;
        panelFlash.color = col;

        float tiempo = 0f;
        while (tiempo < duracionFlash)
        {
            tiempo += Time.unscaledDeltaTime;
            col.a = Mathf.Lerp(1f, 0f, tiempo / duracionFlash);
            panelFlash.color = col;
            yield return null;
        }

        panelFlash.gameObject.SetActive(false);
    }

    private IEnumerator ShakeCamaraRoutine()
    {
        Vector3 posOriginal = camaraPrincipal.transform.localPosition;
        float tiempo = 0f;

        while (tiempo < duracionShake)
        {
            tiempo += Time.unscaledDeltaTime;

            Vector2 offset = Random.insideUnitCircle * magnitudShake;
            camaraPrincipal.transform.localPosition = posOriginal + new Vector3(offset.x, offset.y, 0f);

            yield return null;
        }

        camaraPrincipal.transform.localPosition = posOriginal;
    }
}