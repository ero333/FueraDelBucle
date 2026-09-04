using UnityEngine;

public class Proyectil : MonoBehaviour
{

    public float velocidad;
    public int daño;
    public float tiempoDeVida = 5f;

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
            vidaJugador.TomarDaño(daño);
            Destroy(gameObject);
        }
    }
}
