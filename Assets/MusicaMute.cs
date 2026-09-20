using UnityEngine;

public class MusicaMute : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    private bool isMuted = true;

    private void Start()
    {
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
        }
    }
}
