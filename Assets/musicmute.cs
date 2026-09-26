using UnityEngine;
using UnityEngine.UI; 

public class musicmute : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;

    [Header("Sprites UI")]
    [SerializeField] private Image buttonImage; 
    [SerializeField] private Sprite musicOnSprite;  
    [SerializeField] private Sprite musicOffSprite; 

    private bool isMuted = false;

    private void Start()
    {
        
        isMuted = false;

        
        PlayerPrefs.SetInt("MusicMuted", 0);
        PlayerPrefs.Save();

        UpdateAudioAndUI();
    }

    public void ToggleMusic()
    {
        isMuted = !isMuted;

        
        PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        UpdateAudioAndUI();
    }

    private void UpdateAudioAndUI()
    {
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }

        if (buttonImage != null)
        {
           
            buttonImage.sprite = isMuted ? musicOffSprite : musicOnSprite;
        }
    }
}