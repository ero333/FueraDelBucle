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

    private Rigidbody2D rb;
    private Transform jugador;
    private bool moviendoDerecha = true;
    private float tiempoGiro = 0.2f;
    private float ultimoGiro;
    private const float zonaMuerta = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        // Raycast
        RaycastHit2D informacionSuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distancia);
        bool haySuelo = informacionSuelo.collider != null;

        if (haySuelo)
        {
            rb.linearVelocity = new Vector2(dir * Mathf.Abs(rapidez), rb.linearVelocity.y);
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
        if (collision.gameObject.CompareTag("Player"))
        {
            VidaJugador vida = collision.gameObject.GetComponent<VidaJugador>();

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