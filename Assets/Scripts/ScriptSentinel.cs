using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptSentinel : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private float distancia = 0.5f;

    [Header("Seguir al jugador")]
    [Tooltip("Si está activado, cuando ve al jugador se da vuelta hacia él y sigue patrullando en esa dirección.")]
    [SerializeField] private bool seguirJugador = false;
    [SerializeField] private float distanciaDeteccion = 8f;
    [SerializeField] private float alturaDeteccion = 2.5f;
    [SerializeField] private float velocidadPersecucion = 3f;
    [SerializeField] private string tagJugador = "Player";
    public bool jugadorDetectado;

    [Header("Sonido")]
    [SerializeField] private AudioClip sonidoCaminata;
    [SerializeField, Range(0f, 1f)] private float volumenCaminata = 1f;
    [Tooltip("0 = se escucha parejo en todo el nivel, 1 = se escucha más fuerte cuanto más cerca está de la cámara.")]
    [SerializeField, Range(0f, 1f)] private float mezclaEspacial = 1f;
    [SerializeField] private float distanciaMaximaSonido = 15f;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Transform jugador;
    private bool moviendoDerecha = true;
    private float tiempoGiro = 0.2f;
    private float ultimoGiro;
    private const float zonaMuerta = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ConfigurarAudio();
        BuscarJugador();
        AplicarOrientacion();
    }

    void FixedUpdate()
    {
        Physics2D.queriesStartInColliders = false;

        jugadorDetectado = seguirJugador && DetectarJugador();

        if (jugadorDetectado)
        {
            float diferencia = jugador.position.x - transform.position.x;
            bool jugadorALaDerecha = diferencia > 0f;

            if (Mathf.Abs(diferencia) > zonaMuerta && jugadorALaDerecha != moviendoDerecha && Time.time >= ultimoGiro + tiempoGiro)
            {
                Girar();
            }
        }

        float dir = moviendoDerecha ? 1f : -1f;
        float rapidez = jugadorDetectado ? velocidadPersecucion : velocidad;
        bool caminando = false;

        // Raycast
        RaycastHit2D informacionSuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distancia);
        bool haySuelo = informacionSuelo.collider != null;

        if (haySuelo)
        {
            rb.linearVelocity = new Vector2(dir * Mathf.Abs(rapidez), rb.linearVelocity.y);
            caminando = Mathf.Abs(rapidez) > 0.01f;
        }
        else if (jugadorDetectado)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(dir * Mathf.Abs(rapidez), rb.linearVelocity.y);

            // Solo gira si no hay suelo Y pasó el tiempo suficiente desde el último giro
            if (Time.time >= ultimoGiro + tiempoGiro)
            {
                Girar();
            }
        }

        ActualizarSonidoCaminata(caminando);
    }

    private void ConfigurarAudio()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = sonidoCaminata;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volumenCaminata;
        audioSource.spatialBlend = mezclaEspacial;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = distanciaMaximaSonido;
    }

    private void ActualizarSonidoCaminata(bool caminando)
    {
        if (audioSource == null || sonidoCaminata == null) return;

        if (caminando && !audioSource.isPlaying)
        {
            audioSource.volume = Volumen.DeEfecto(volumenCaminata);
            audioSource.Play();
        }
        else if (!caminando && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    private void BuscarJugador()
    {
        GameObject objetivo = GameObject.FindGameObjectWithTag(tagJugador);

        if (objetivo != null)
        {
            jugador = objetivo.transform;
        }
    }

    private bool DetectarJugador()
    {
        if (jugador == null)
        {
            BuscarJugador();

            if (jugador == null) return false;
        }

        Vector2 diferencia = jugador.position - transform.position;

        return Mathf.Abs(diferencia.x) <= distanciaDeteccion && Mathf.Abs(diferencia.y) <= alturaDeteccion;
    }

    private void Girar()
    {
        ultimoGiro = Time.time;
        moviendoDerecha = !moviendoDerecha;
        AplicarOrientacion();
    }

    private void AplicarOrientacion()
    {
        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * (moviendoDerecha ? 1f : -1f);
        transform.localScale = escala;

        if (controladorSuelo != null)
        {
            Vector3 posRelativa = controladorSuelo.localPosition;
            posRelativa.x = Mathf.Abs(posRelativa.x);
            controladorSuelo.localPosition = posRelativa;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        VerificarYMatarJugador(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Se ejecuta en cada cuadro de física que el jugador siga superpuesto/en contacto
        VerificarYMatarJugador(collision.gameObject);
    }

    private void VerificarYMatarJugador(GameObject objeto)
    {
        if (objeto.CompareTag("Player"))
        {
            PlayerPhysics playerPhysics = objeto.GetComponent<PlayerPhysics>();

            // Si el jugador está activo en Phase, ignoramos el daño
            if (playerPhysics != null && playerPhysics.EstaEnPhase) return;

            VidaJugador vida = objeto.GetComponent<VidaJugador>();

            if (vida != null)
            {
                vida.TomarDaño(vida.cantidadDeVida);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(controladorSuelo.position, controladorSuelo.position + Vector3.down * distancia);
        }

        if (seguirJugador)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(distanciaDeteccion * 2f, alturaDeteccion * 2f, 0f));
        }
    }
}