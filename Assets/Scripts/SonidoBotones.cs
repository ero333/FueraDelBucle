
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SonidoBotones : MonoBehaviour
{
    public static SonidoBotones Instancia { get; private set; }

    [Header("Sonido de botones")]
    public AudioClip sonidoClic;

    [Header("Sonido del glitch")]
    public AudioClip sonidoGlitch;
    [Range(0f, 1f)]
    public float volumenGlitch = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        SceneManager.sceneLoaded += AlCargarEscena;
    }

    void Start()
    {
        if (Instancia != this) return;

        EngancharBotones();
    }

    void OnDestroy()
    {
        if (Instancia != this) return;

        SceneManager.sceneLoaded -= AlCargarEscena;
        Instancia = null;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        EngancharBotones();
    }

    private void EngancharBotones()
    {
        Button[] botones = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button boton in botones)
        {
            boton.onClick.RemoveListener(ReproducirSonido);
            boton.onClick.AddListener(ReproducirSonido);
        }
    }

    void ReproducirSonido()
    {
        if (sonidoClic != null)
        {
            audioSource.PlayOneShot(
                sonidoClic,
                Volumen.DeEfecto(1f)
            );
        }
    }

    // Llamar cuando comienza el efecto visual glitch
    public void ReproducirGlitch()
    {
        if (sonidoGlitch != null)
        {
            audioSource.PlayOneShot(
                sonidoGlitch,
                Volumen.DeEfecto(volumenGlitch)
            );
        }
    }
}
