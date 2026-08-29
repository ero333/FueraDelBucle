using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{

    public Transform controladorDisparo;
    public float distanciaLinea;

    public LayerMask capaJugador;

    public bool jugadorEnRango;



    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        jugadorEnRango = Physics2D.Raycast(controladorDisparo.position, transform.right, distanciaLinea, capaJugador);
        if (jugadorEnRango) 
        { 
         
        }
    }

    private void OnDrawGizmos () 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(controladorDisparo.position, controladorDisparo.position + transform.right * distanciaLinea);
    }
}
