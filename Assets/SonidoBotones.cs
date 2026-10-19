using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SonidoBotones : MonoBehaviour
{
    public AudioClip sonidoClic;
    private AudioSource audioSource;

    void Awake()
    {
        // Esto evita que el Manager de sonido se destruya al cargar otra escena
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        Button[] botones = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button boton in botones)
        {
            boton.onClick.AddListener(ReproducirSonido);
        }
    }

    void ReproducirSonido()
    {
        if (sonidoClic != null)
        {
            audioSource.PlayOneShot(sonidoClic);
        }
    }
}