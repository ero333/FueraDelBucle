using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ActionAudioPlayer : MonoBehaviour
{
    [Header("Configuración de Sonido - Salto")]
    [SerializeField] private AudioClip sonidoSalto;
    [SerializeField][Range(0f, 1f)] private float volumenSalto = 1f;

    [Header("Configuración de Sonido - Phase (Bucle)")]
    [SerializeField] private AudioClip sonidoPhase;
    [SerializeField][Range(0f, 1f)] private float volumenPhase = 1f;

    [Header("Referencias")]
    [SerializeField] private PlayerPhysics playerPhysics;

    private AudioSource audioSource;
    private AudioSource audioSourcePhase; // Un canal dedicado para que el bucle no interfiera con otros sonidos
    private bool estaEnPhaseAnterior;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Creamos un segundo AudioSource exclusivo para el sonido en bucle del Phase
        audioSourcePhase = gameObject.AddComponent<AudioSource>();
        audioSourcePhase.loop = true;
        audioSourcePhase.playOnAwake = false;

        if (playerPhysics == null)
        {
            playerPhysics = GetComponent<PlayerPhysics>();
        }

        if (playerPhysics != null)
        {
            estaEnPhaseAnterior = playerPhysics.EstaEnPhase;
        }
    }

    void Update()
    {
        if (playerPhysics == null) return;

        // Detección de salto
        bool intentoSaltar = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        if (intentoSaltar && playerPhysics.EstaEnElSuelo)
        {
            ReproducirSalto();
        }

        // Detección de estado del Phase
        bool estaEnPhaseActual = playerPhysics.EstaEnPhase;

        // Si acaba de activarse el Phase, empieza a sonar en bucle
        if (!estaEnPhaseAnterior && estaEnPhaseActual)
        {
            IniciarSonidoPhase();
        }
        // Si acaba de desactivarse el Phase, se corta el sonido
        else if (estaEnPhaseAnterior && !estaEnPhaseActual)
        {
            DetenerSonidoPhase();
        }

        estaEnPhaseAnterior = estaEnPhaseActual;
    }

    public void ReproducirSalto()
    {
        if (sonidoSalto != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoSalto, Volumen.DeEfecto(volumenSalto));
        }
    }

    private void IniciarSonidoPhase()
    {
        if (sonidoPhase != null && audioSourcePhase != null)
        {
            audioSourcePhase.clip = sonidoPhase;
            audioSourcePhase.volume = Volumen.DeEfecto(volumenPhase);
            audioSourcePhase.Play();
        }
    }

    private void DetenerSonidoPhase()
    {
        if (audioSourcePhase != null && audioSourcePhase.isPlaying)
        {
            audioSourcePhase.Stop();
        }
    }

    private void OnEnable()
    {
        Volumen.Cambio += AplicarVolumen;
    }

    private void OnDisable()
    {
        Volumen.Cambio -= AplicarVolumen;
    }

    // El Phase suena en bucle, así que tiene que seguir el control de volumen mientras está sonando.
    private void AplicarVolumen()
    {
        if (audioSourcePhase != null)
        {
            audioSourcePhase.volume = Volumen.DeEfecto(volumenPhase);
        }
    }
}