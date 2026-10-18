using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditosScroll : MonoBehaviour, IBeginDragHandler
{
    [Header("Partes del scroll")]
    public ScrollRect scroll;
    public RectTransform contenido;
    public TMP_Text texto;

    [Header("Movimiento")]
    [Tooltip("Segundos que tarda en recorrer los creditos de arriba a abajo.")]
    public float duracion = 30f;

    [Tooltip("Segundos de espera antes de que arranque solo.")]
    public float esperaInicial = 1.5f;

    [Tooltip("Espacio que queda debajo de la ultima linea.")]
    public float margenFinal = 40f;

    private float transcurrido;
    private bool automatico = true;

    void Start()
    {
        if (scroll == null)
        {
            scroll = GetComponent<ScrollRect>();
        }

        AjustarAlto();

        if (scroll != null)
        {
            scroll.verticalNormalizedPosition = 1f;
        }
    }

    void Update()
    {
        if (!automatico || scroll == null) return;

        transcurrido += Time.deltaTime;

        if (transcurrido < esperaInicial) return;

        float avance = (transcurrido - esperaInicial) / Mathf.Max(0.01f, duracion);

        scroll.verticalNormalizedPosition = Mathf.Clamp01(1f - avance);

        if (avance >= 1f)
        {
            automatico = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        automatico = false;
    }

    private void AjustarAlto()
    {
        if (contenido == null || texto == null) return;

        texto.ForceMeshUpdate();

        float alto = texto.preferredHeight + margenFinal;

        contenido.sizeDelta = new Vector2(contenido.sizeDelta.x, alto);
    }
}
