using UnityEngine;
using UnityEngine.UI;

public class musicmute : MonoBehaviour
{
    [Header("Sprites UI")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;

    private void Start()
    {
        ActualizarUI();
    }

    private void OnEnable()
    {
        ActualizarUI();
    }

    public void ToggleMusic()
    {
        bool estaMuteado = !ObtenerEstadoMute();

       
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.CambiarMute(estaMuteado);
        }
        else
        {
            
            PlayerPrefs.SetInt("MusicMuted", estaMuteado ? 1 : 0);
            PlayerPrefs.Save();

           
            MusicaNivel musicaLocal = FindFirstObjectByType<MusicaNivel>();
            if (musicaLocal != null)
            {
                AudioSource source = musicaLocal.GetComponent<AudioSource>();
                if (source != null)
                {
                    source.mute = estaMuteado;
                }
            }
        }

        ActualizarUI();
    }

    private void ActualizarUI()
    {
        bool estaMuteado = ObtenerEstadoMute();

        if (buttonImage != null)
        {
            buttonImage.sprite = estaMuteado ? musicOffSprite : musicOnSprite;
        }
    }

    private bool ObtenerEstadoMute()
    {
        if (MusicManager.Instance != null)
        {
            return MusicManager.Instance.IsMuted;
        }

        return PlayerPrefs.GetInt("MusicMuted", 0) == 1;
    }
}