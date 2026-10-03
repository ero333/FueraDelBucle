using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] private AudioSource musicSource;
    public bool IsMuted => musicSource != null && musicSource.mute;

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
        bool muteado = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        if (musicSource != null)
        {
            musicSource.mute = muteado;
        }
    }

    public void CambiarMute(bool mutear)
    {
        if (musicSource != null)
        {
            musicSource.mute = mutear;
        }
        PlayerPrefs.SetInt("MusicMuted", mutear ? 1 : 0);
        PlayerPrefs.Save();
    }
}
