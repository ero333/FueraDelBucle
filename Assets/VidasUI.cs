using UnityEngine;
using UnityEngine.UI;


public class VidasUI : MonoBehaviour
{
    public Sprite[] imagenes;

    public VidaJugador jugador;

    public Image imagen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj != null)
        {
            jugador = jugadorObj.GetComponent<VidaJugador>();
        }
        //jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Movimiento>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (jugador != null && imagen != null && jugador.cantidadDeVida >= 0 && jugador.cantidadDeVida < imagenes.Length)
        {
            imagen.sprite = imagenes[jugador.cantidadDeVida];
        }
        //imagen.sprite = imagenes[jugador.vidas];

    }
}
