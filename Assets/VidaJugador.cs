using UnityEngine;

public class VidaJugador : MonoBehaviour
{

    public int cantidadDeVida;

    public void TomarDaño (int daño)
    {
        cantidadDeVida -= daño;

        if  ( cantidadDeVida <=0)
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
