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
            sliderMusica.SetValueWithoutNotify(Volumen.Musica);
        }

        if (sliderEfectos != null)
        {
            sliderEfectos.SetValueWithoutNotify(Volumen.Efectos);
        }

        MostrarPorcentajes();
    }

    public void CambiarMusica(float valor)
    {
        Volumen.PonerMusica(valor);

        if (valor > 0f && MusicManager.Instance != null && MusicManager.Instance.IsMuted)
        {
            MusicManager.Instance.CambiarMute(false);
        }

        MostrarPorcentajes();
    }

    public void CambiarEfectos(float valor)
    {
        Volumen.PonerEfectos(valor);
        MostrarPorcentajes();
    }

    private void MostrarPorcentajes()
    {
        if (textoMusica != null)
        {
            textoMusica.text = Mathf.RoundToInt(Volumen.Musica * 100f) + "%";
        }

        if (textoEfectos != null)
        {
            textoEfectos.text = Mathf.RoundToInt(Volumen.Efectos * 100f) + "%";
        }
    }
}
