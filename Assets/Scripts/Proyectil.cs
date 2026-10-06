using UnityEngine;

public class Proyectil : MonoBehaviour
{

    public float velocidad;
    public int daño;
    public float tiempoDeVida = 5f;

    [Tooltip("Capas contra las que el disparo se rompe (plataformas, suelo). Vacío = atraviesa todo hasta que se acaba el tiempo de vida.")]
    public LayerMask capasQueFrenan;

     void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * velocidad * Vector2.right);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out VidaJugador vidaJugador))
        {
            // En modo Phase el proyectil atraviesa al jugador: no hace daño ni se destruye.
            if (other.TryGetComponent(out PlayerPhysics fisicaJugador) && fisicaJugador.EstaEnPhase)
                return;

            vidaJugador.TomarDaño(daño);
            Destroy(gameObject);
            return;
        }

        if (((1 << other.gameObject.layer) & capasQueFrenan.value) != 0)
        {
            Destroy(gameObject);
        }
    }
}
