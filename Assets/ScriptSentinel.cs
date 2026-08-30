using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptSentinel : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private float distancia = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool moviendoDerecha = true;
    private float tiempoGiro = 0.2f;
    private float ultimoGiro;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        Physics2D.queriesStartInColliders = false;

        // Movimiento según dirección
        float dir = moviendoDerecha ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * Mathf.Abs(velocidad), rb.linearVelocity.y);

        // Raycast
        RaycastHit2D informacionSuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distancia);

        // Solo gira si no hay suelo Y pasó el tiempo suficiente desde el último giro
        if (informacionSuelo.collider == null && Time.time >= ultimoGiro + tiempoGiro)
        {
            Girar();
        }
    }

    private void Girar()
    {
        ultimoGiro = Time.time;
        moviendoDerecha = !moviendoDerecha;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !moviendoDerecha;
        }

        // Mueve el punto detector hacia el otro extremo en X
        Vector3 posRelativa = controladorSuelo.localPosition;
        posRelativa.x = Mathf.Abs(posRelativa.x) * (moviendoDerecha ? 1 : -1);
        controladorSuelo.localPosition = posRelativa;
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
    }
}