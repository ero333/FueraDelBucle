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

    [Header("Comportamiento")]
    [Tooltip("Si está activado, dispara desde el arranque en bucle sin necesitar detectar al jugador con el raycast.")]
    public bool dispararSiempre = false;

    private float cronometro;

    private void Start()
    {
        cronometro = 0f;

        // Congela la rotación física: sin esto, un empujón del jugador le mete
        // torque y lo hace girar sin control como un trompo (y la caída se ve
        // distinta y errática según el ángulo del golpe).
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Update()
    {
        if (controladorDisparo == null) return;

        if (dispararSiempre)
        {
            jugadorEnRango = true;
        }
        else
        {
            bool originalSetting = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = false;


            RaycastHit2D hit = Physics2D.Raycast(controladorDisparo.position, transform.right, distanciaLinea, capaJugador);


            Physics2D.queriesStartInColliders = originalSetting;

            jugadorEnRango = hit.collider != null;
        }

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

        GameObject copia = Instantiate(proyectil, controladorDisparo.position, controladorDisparo.rotation);
        copia.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        if (controladorDisparo == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(controladorDisparo.position, controladorDisparo.position + transform.right * distanciaLinea);
    }
}