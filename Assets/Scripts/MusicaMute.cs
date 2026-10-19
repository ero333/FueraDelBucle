using UnityEngine;

public class MusicaMute : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private GameObject otroBoton;

    [Header("Estado de este botón")]
    [SerializeField] private bool prenderMusica;

    private void Start()
    {
        SincronizarBoton();
    }

    private void OnEnable()
    {
        Volumen.Cambio += SincronizarBoton;
        SincronizarBoton();
    }

    private void OnDisable()
    {
        Volumen.Cambio -= SincronizarBoton;
    }

    private void SincronizarBoton()
    {
        bool estaMuteado = Volumen.Silenciado;

        // Determina si este botón en particular debe estar encendido o apagar
        bool deboMostrarme = prenderMusica ? estaMuteado : !estaMuteado;

        if (!deboMostrarme && otroBoton != null && !otroBoton.activeSelf)
        {
            otroBoton.SetActive(true);
        }

        gameObject.SetActive(deboMostrarme);
    }

    public void ToggleMusic()
    {
        Volumen.PonerSilencio(!prenderMusica);

        if (otroBoton != null)
        {
            otroBoton.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}