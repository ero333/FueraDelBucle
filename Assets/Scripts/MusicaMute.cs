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
        SincronizarBoton();
    }

    private void SincronizarBoton()
    {
        bool estaMuteado = false;

        if (MusicManager.Instance != null)
        {
            estaMuteado = MusicManager.Instance.IsMuted;
        }
        else
        {
            estaMuteado = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        }

        // Determina si este botón en particular debe estar encendido o apagar
        bool deboMostrarme = prenderMusica ? estaMuteado : !estaMuteado;
        gameObject.SetActive(deboMostrarme);
    }

    public void ToggleMusic()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.CambiarMute(!prenderMusica);
        }

        if (otroBoton != null)
        {
            otroBoton.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}