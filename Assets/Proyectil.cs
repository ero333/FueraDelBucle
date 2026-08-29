using UnityEngine;

public class Proyectil : MonoBehaviour
{

    public float velocidad;
    public int daño;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(Time.deltaTime * velocidad * Vector2.right);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out VidaJugador vidaJugador))
        {
            vidaJugador.TomarDaño(daño);
        }
    }
}
