using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class Pregunta : MonoBehaviour
{
    [Header("Configuración del Fade")]
    public float duracionFade = 2.0f;
    public float delayInicial = 0.5f;

    [Header("Referencias del Glitch")]
    public TextMeshProUGUI textoObjetivo; 

    [Header("Configuración del Glitch de Color")]
    public Color colorBase = Color.white;
    public Color colorGlitch1 = new Color(0.3f, 1f, 1f, 1f); 
    public Color colorGlitch2 = new Color(1f, 0.3f, 0.3f, 1f); 
    [Header("Parámetros del Glitch")]
    public float intensidadGlitch = 4f;
    public float frecuenciaGlitch = 0.05f;
    [Range(0.01f, 0.5f)]
    public float probabilidadGlitch = 0.2f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransformTexto;
    private Vector3 posicionOriginalTexto;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; 

        if (textoObjetivo != null)
        {
            rectTransformTexto = textoObjetivo.GetComponent<RectTransform>();
            if (rectTransformTexto != null)
            {
                posicionOriginalTexto = rectTransformTexto.anchoredPosition;
            }
            textoObjetivo.color = colorBase;
        }
    }

    private void Start()
    {
        StartCoroutine(EjecutarEfectosUI());
    }

    private IEnumerator EjecutarEfectosUI()
    {
        // 1. Espera inicial
        if (delayInicial > 0)
        {
            yield return new WaitForSeconds(delayInicial);
        }

        //  Fade In progresivo del cuadro + texto
        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(tiempo / duracionFade);
            yield return null;
        }

        canvasGroup.alpha = 1f; 

        //  Glitch del texto de forma infinita
        while (true)
        {
            if (textoObjetivo != null)
            {
                if (Random.value < probabilidadGlitch)
                {
                  
                    if (rectTransformTexto != null)
                    {
                        Vector2 offset = new Vector2(
                            Random.Range(-intensidadGlitch, intensidadGlitch),
                            Random.Range(-intensidadGlitch / 2f, intensidadGlitch / 2f)
                        );
                        rectTransformTexto.anchoredPosition = (Vector2)posicionOriginalTexto + offset;
                    }

                  
                    float cambioColor = Random.value;
                    if (cambioColor < 0.5f)
                        textoObjetivo.color = colorGlitch1;
                    else
                        textoObjetivo.color = colorGlitch2;
                }
                else
                {
                    
                    if (rectTransformTexto != null) rectTransformTexto.anchoredPosition = posicionOriginalTexto;
                    textoObjetivo.color = colorBase;
                }
            }

            yield return new WaitForSeconds(frecuenciaGlitch);
        }
    }
}