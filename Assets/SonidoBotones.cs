using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SonidoBotones : MonoBehaviour
{
    public static SonidoBotones Instancia { get; private set; }

    public AudioClip sonidoClic;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        // Esto evita que el Manager de sonido se destruya al cargar otra escena
        DontDestroyOnLoad(this.gameObject);

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
        Button[] botones = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

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
            audioSource.PlayOneShot(sonidoClic, Volumen.DeEfecto(1f));
        }
    }
}