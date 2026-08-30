using UnityEngine;

public class EnemigoMortal : MonoBehaviour
{
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
}