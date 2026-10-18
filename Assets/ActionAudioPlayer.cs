using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ActionAudioPlayer : MonoBehaviour
{
    [Header("Configuración de Sonido")]
    [SerializeField] private AudioClip sonidoSalto;
    [SerializeField][Range(0f, 1f)] private float volumenSalto = 1f;

    [Header("Referencias Opcionales")]
    [SerializeField] private PlayerPhysics playerPhysics;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (playerPhysics == null)
        {
            playerPhysics = GetComponent<PlayerPhysics>();
        }
    }

    void Update()
    {
        bool intentoSaltar = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);

        if (intentoSaltar)
        {
            if (playerPhysics != null)
            {
                if (playerPhysics.EstaEnElSuelo)
                {
                    ReproducirSalto();
                }
            }
            else
            {
                ReproducirSalto();
            }
        }
    }

    public void ReproducirSalto()
    {
        if (sonidoSalto != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoSalto, volumenSalto);
        }
    }
}