using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class ControladorVideoSimulado : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Segundos que dura la pantalla de la animación antes de pasar a la escena siguiente.")]
    public float tiempoDeEspera = 9f;
    public string escenaSiguiente;

    [Tooltip("Imagen del primer cuadro del video, dentro de Resources. Tapa el medio segundo que tarda el archivo en abrir.")]
    public string imagenDeEspera = "PrimerCuadroFinal";

    private VideoPlayer videoPlayer;
    private bool cambiando;
    private Coroutine respaldo;
    private GameObject tapa;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Cambiar();
            return;
        }

        // Arranca apagado y se enciende recien cuando el archivo esta listo. Antes estaba
        // en playOnAwake con una ruta de otra computadora grabada en la escena: intentaba
        // abrir eso, fallaba, y recien ahi el script le ponia la ruta buena.
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = false;
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Animacion_final.mp4");

        MostrarPrimerCuadro();

        videoPlayer.prepareCompleted += AlEstarListo;
        videoPlayer.loopPointReached += AlTerminar;
        videoPlayer.errorReceived += AlFallar;

        videoPlayer.Prepare();

        // Si el archivo no abre, no hay que quedarse trabado en una pantalla negra.
        respaldo = StartCoroutine(EsperarYCambiar(tiempoDeEspera));
    }

    private void AlEstarListo(VideoPlayer vp)
    {
        vp.Play();

        StartCoroutine(SacarTapaCuandoHayaImagen(vp));

        // Los segundos se cuentan desde que el video arranca de verdad, no desde que carga
        // la escena: asi la pantalla dura siempre lo mismo, tarde lo que tarde en abrir.
        if (respaldo != null) StopCoroutine(respaldo);

        respaldo = StartCoroutine(EsperarYCambiar(tiempoDeEspera));
    }

    // El primer cuadro del video, quieto, mientras el archivo termina de abrir. El video
    // arranca justo en esa misma imagen, asi que el cambio no se ve. Se arma por codigo
    // para no tener que tocar la escena.
    private void MostrarPrimerCuadro()
    {
        if (string.IsNullOrEmpty(imagenDeEspera)) return;

        Texture2D textura = Resources.Load<Texture2D>(imagenDeEspera);

        if (textura == null) return;

        tapa = new GameObject("TapaPrimerCuadro");

        Canvas lienzo = tapa.AddComponent<Canvas>();
        lienzo.renderMode = RenderMode.ScreenSpaceOverlay;
        lienzo.sortingOrder = 100;

        GameObject objetoImagen = new GameObject("Imagen");
        objetoImagen.transform.SetParent(tapa.transform, false);

        RawImage imagen = objetoImagen.AddComponent<RawImage>();
        imagen.texture = textura;
        imagen.raycastTarget = false;

        // El VideoPlayer esta en Stretch, asi que la imagen se estira igual y coinciden.
        RectTransform rect = imagen.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    // No se saca apenas se llama a Play: se espera a que el video haya sacado su primer
    // cuadro de verdad, si no se cuela un parpadeo negro entre una cosa y la otra.
    private IEnumerator SacarTapaCuandoHayaImagen(VideoPlayer vp)
    {
        float limite = Time.unscaledTime + 2f;

        while (vp.frame < 1 && Time.unscaledTime < limite)
        {
            yield return null;
        }

        SacarTapa();
    }

    private void SacarTapa()
    {
        if (tapa == null) return;

        Destroy(tapa);
        tapa = null;
    }

    private void AlTerminar(VideoPlayer vp)
    {
        Cambiar();
    }

    private void AlFallar(VideoPlayer vp, string mensaje)
    {
        Debug.LogWarning("No se pudo abrir la animacion final: " + mensaje);
        Cambiar();
    }

    IEnumerator EsperarYCambiar(float segundos)
    {
        yield return new WaitForSeconds(segundos);

        Cambiar();
    }

    private void Cambiar()
    {
        if (cambiando) return;

        cambiando = true;
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(escenaSiguiente)) return;

        SceneManager.LoadScene(escenaSiguiente);
    }

    private void OnDestroy()
    {
        if (videoPlayer == null) return;

        videoPlayer.prepareCompleted -= AlEstarListo;
        videoPlayer.loopPointReached -= AlTerminar;
        videoPlayer.errorReceived -= AlFallar;
    }
}
