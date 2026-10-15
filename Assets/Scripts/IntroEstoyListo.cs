using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroEstoyListo : MonoBehaviour
{
    public Button botonEstoyListo;
    public CanvasGroup canvasGroup;

    public float tiempoParaMostrar = 3f;
    public float duracionFadeIn = 1f;

    public string nombreNivel1 = "Nivel1";

    private bool introTerminada = false;

    void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        botonEstoyListo.onClick.AddListener(SaltarIntro);

        StartCoroutine(MostrarBoton());
        StartCoroutine(TerminaEscena());
    }

    IEnumerator MostrarBoton()
    {
        yield return new WaitForSeconds(tiempoParaMostrar);

        float tiempo = 0f;

        while (tiempo < duracionFadeIn)
        {
            tiempo += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(tiempo / duracionFadeIn);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void SaltarIntro()
    {
        if (introTerminada) return;

        introTerminada = true;
        SceneManager.LoadScene(nombreNivel1);
    }

    public IEnumerator TerminaEscena()
    {
        yield return new WaitForSeconds (18f);
        SceneManager.LoadScene(nombreNivel1);

    }
}