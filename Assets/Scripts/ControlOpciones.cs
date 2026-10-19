using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlOpciones : MonoBehaviour
{
    [Header("Musica")]
    public Slider sliderMusica;
    public TMP_Text textoMusica;

    [Header("Efectos")]
    public Slider sliderEfectos;
    public TMP_Text textoEfectos;

    void OnEnable()
    {
        if (sliderMusica != null)
        {
            sliderMusica.SetValueWithoutNotify(Volumen.MusicaGuardada);
        }

        if (sliderEfectos != null)
        {
            sliderEfectos.SetValueWithoutNotify(Volumen.EfectosGuardados);
        }

        MostrarPorcentajes();
    }

    public void CambiarMusica(float valor)
    {
        Volumen.PonerMusica(valor);

        if (valor > 0f)
        {
            Volumen.PonerSilencio(false);
        }

        MostrarPorcentajes();
    }

    public void CambiarEfectos(float valor)
    {
        Volumen.PonerEfectos(valor);

        if (valor > 0f)
        {
            Volumen.PonerSilencio(false);
        }

        MostrarPorcentajes();
    }

    private void MostrarPorcentajes()
    {
        if (textoMusica != null)
        {
            textoMusica.text = Mathf.RoundToInt(Volumen.MusicaGuardada * 100f) + "%";
        }

        if (textoEfectos != null)
        {
            textoEfectos.text = Mathf.RoundToInt(Volumen.EfectosGuardados * 100f) + "%";
        }
    }
}
