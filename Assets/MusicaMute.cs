using UnityEngine;

public class MusicaMute : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    private bool isMuted = true;

    private void Start()
    {
        int savedMute = PlayerPrefs.GetInt("MusicMuted", 1);

        isMuted = (savedMute == 1);

        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }
    }

    public void ToggleMusic()
    {
        if (musicSource != null)
        {
            isMuted = !isMuted;
            musicSource.mute = isMuted;

            PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
