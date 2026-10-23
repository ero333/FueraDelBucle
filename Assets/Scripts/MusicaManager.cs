using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] private AudioSource musicSource;
    public bool IsMuted => Volumen.Silenciado;

    private void Awake()
    {
        // Musica no se destruya
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
            return;
        }

        // Carga el estado que se guardo
        if (musicSource != null)
        {
            musicSource.mute = Volumen.Silenciado;
        }
    }

    private void OnEnable()
    {
        Volumen.Cambio += AplicarVolumen;
        AplicarVolumen();
    }

    private void OnDisable()
    {
        Volumen.Cambio -= AplicarVolumen;
    }

    private void AplicarVolumen()
    {
        if (musicSource != null)
        {
            musicSource.mute = Volumen.Silenciado;
            musicSource.volume = Volumen.Musica;
        }
    }

    public void CambiarMute(bool mutear)
    {
        Volumen.PonerSilencio(mutear);
    }
}
