using UnityEngine;

public class RepeaterSegundo : MonoBehaviour
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

    [Header("Giro hacia el jugador")]
    [Tooltip("Si está activado, el enemigo se da vuelta para quedar mirando al jugador.")]
    public bool girarHaciaJugador = true;

    [Tooltip("Distancia horizontal a la que detecta al jugador para darse vuelta.")]
    public float distanciaGiro = 12f;

    [Tooltip("Tag del jugador.")]
    public string tagJugador = "Player";

    public bool jugadorDetectado;

    private float cronometro;
    private Transform jugador;

    private void Start()
    {
        cronometro = 0f;
        BuscarJugador();
    }

    private void Update()
    {
        if (girarHaciaJugador)
        {
            MirarAlJugador();
        }

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

    private void BuscarJugador()
    {
        GameObject objetivo = GameObject.FindGameObjectWithTag(tagJugador);

        if (objetivo != null)
        {
            jugador = objetivo.transform;
        }
    }

    private void MirarAlJugador()
    {
        if (jugador == null)
        {
            BuscarJugador();

            if (jugador == null) return;
        }

        float diferencia = jugador.position.x - transform.position.x;

        jugadorDetectado = Mathf.Abs(diferencia) <= distanciaGiro;

        if (!jugadorDetectado) return;

        Girar(diferencia > 0f);
    }

    private void Girar(bool haciaLaDerecha)
    {
        Vector3 angulos = transform.eulerAngles;
        angulos.z = haciaLaDerecha ? 0f : 180f;
        transform.eulerAngles = angulos;

        Vector3 escala = transform.localScale;
        escala.y = haciaLaDerecha ? Mathf.Abs(escala.y) : -Mathf.Abs(escala.y);
        transform.localScale = escala;
    }

    private void Disparar()
    {
        if (proyectil == null) return;

        GameObject copia = Instantiate(proyectil, controladorDisparo.position, controladorDisparo.rotation);
        copia.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        if (girarHaciaJugador)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.left * distanciaGiro, transform.position + Vector3.right * distanciaGiro);
        }

        if (controladorDisparo == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(controladorDisparo.position, controladorDisparo.position + transform.right * distanciaLinea);
    }
}
