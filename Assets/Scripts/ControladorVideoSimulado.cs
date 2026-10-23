using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class ControladorVideoSimulado : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoDeEspera = 5f;
    public string escenaSiguiente;

    void Start()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Animacion_final.mp4");
        StartCoroutine(EsperarYCambiar());
    }

    IEnumerator EsperarYCambiar()
    {
        yield return new WaitForSeconds(tiempoDeEspera);
        SceneManager.LoadScene(escenaSiguiente);
    }
}