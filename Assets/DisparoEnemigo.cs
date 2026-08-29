using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{
    public Transform controladorDisparo;
    public float distanciaLinea = 10f;
    public LayerMask capaJugador;
    public bool jugadorEnRango;

    [Header("Configuración de Disparo")]
    public GameObject proyectil;
    public float tiempoEntreDisparos = 1.5f;
    private float cronometro;

    private void Start()
    {
        cronometro = 0f; 
    }

    private void Update()
    {
        if (controladorDisparo == null) return;

        
        bool originalSetting = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;

        
        RaycastHit2D hit = Physics2D.Raycast(controladorDisparo.position, transform.right, distanciaLinea, capaJugador);

       
        Physics2D.queriesStartInColliders = originalSetting;

        jugadorEnRango = hit.collider != null;

        if (jugadorEnRango)
        {
            cronometro -= Time.deltaTime;

            if (cronometro <= 0f)
            {
                Disparar();
                cronometro = tiempoEntreDisparos;
            }
        }
        else
        {
            cronometro = 0f; 
        }
    }

    private void Disparar()
    {
        if (proyectil == null) return;

        Instantiate(proyectil, controladorDisparo.position, controladorDisparo.rotation);
    }

    private void OnDrawGizmos()
    {
        if (controladorDisparo == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(controladorDisparo.position, controladorDisparo.position + transform.right * distanciaLinea);
    }
}