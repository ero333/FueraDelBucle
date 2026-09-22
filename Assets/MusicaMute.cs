using UnityEngine;

public class MusicaMute : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;

    [Header("Botones")]
    [SerializeField] private GameObject otroBoton;

    [Header("Estado de este botón")]
    [SerializeField] private bool prenderMusica;

    public void ToggleMusic()
    {
        if (musicSource != null)
        {
            // Este botón decide directamente el estado
            musicSource.mute = !prenderMusica;

            PlayerPrefs.SetInt("MusicMuted", prenderMusica ? 0 : 1);
            PlayerPrefs.Save();
        }

        // Activa el otro botón
        if (otroBoton != null)
            otroBoton.SetActive(true);

        // Desactiva este
        gameObject.SetActive(false);
    }
}