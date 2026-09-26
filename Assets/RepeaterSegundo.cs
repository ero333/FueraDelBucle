using UnityEngine;

public class RepeaterSegundo : MonoBehaviour
{
    [Header("DISPARO")]
    public Transform controladorDisparo;
    public float distanciaLinea = 10f;
    [Tooltip("Altura máxima a la que detecta al jugador verticalmente (para que no dispare si está muy arriba/abajo).")]
    public float toleranciaVertical = 2.5f;
    public LayerMask capaJugador;
    public bool jugadorEnRango;

    [Header("Configuración de Disparo")]
    public GameObject proyectil;
    public float tiempoEntreDisparos = 1.5f;

    [Header("Comportamiento")]
    [Tooltip("Si está activado, dispara siempre sin depender del jugador.")]
    public bool dispararSiempre = false;

    [Header("Giro hacia el jugador")]
    public bool girarHaciaJugador = true;

    public float distanciaGiro = 12f;

    public string tagJugador = "Player";

    public bool jugadorDetectado;

    private float cronometro;
    private Transform jugador;

    private Vector3 escalaOriginal;

    // true = actualmente mira a la derecha
    private bool mirandoDerecha;

    private void Start()
    {
        cronometro = 0f;

        escalaOriginal = transform.localScale;

        BuscarJugador();
    }

    private void Update()
    {
        // ============================
        // BUSCAR JUGADOR SI NO EXISTE
        // ============================

        if (jugador == null)
        {
            BuscarJugador();
        }

        // ============================
        // MIRAR AL JUGADOR
        // ============================

        if (girarHaciaJugador)
        {
            MirarAlJugador();
        }

        if (controladorDisparo == null)
            return;


        // ============================
        // DETECCIÓN DEL JUGADOR
        // ============================

        if (dispararSiempre)
        {
            jugadorEnRango = true;
        }
        else if (jugador != null)
        {
            Vector3 origen = controladorDisparo.position;
            Vector3 posJugador = jugador.position;

            // Drenaje de distancia en X y en Y
            float diferenciaX = posJugador.x - origen.x;
            float diferenciaY = Mathf.Abs(posJugador.y - origen.y);

            // 1. ¿Está en el rango de altura aceptable?
            bool enAlturaCorrecta = diferenciaY <= toleranciaVertical;

            // 2. ¿El jugador está ENFRENTE de la mirada del enemigo?
            bool estaEnfrente = mirandoDerecha ? (diferenciaX > 0f) : (diferenciaX < 0f);

            // 3. ¿Está dentro de la distancia máxima de disparo?
            bool estaEnDistancia = Mathf.Abs(diferenciaX) <= distanciaLinea;

            // Solo entra en rango si cumple las 3 condiciones
            jugadorEnRango = enAlturaCorrecta && estaEnfrente && estaEnDistancia;
        }
        else
        {
            jugadorEnRango = false;
        }


        // ============================
        // DISPARO
        // ============================

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
            // Reiniciar el cronómetro si el jugador se escapa/sale del rango
            cronometro = 0f;
        }
    }


    // =====================================================
    // BUSCAR JUGADOR
    // =====================================================

    private void BuscarJugador()
    {
        GameObject objetivo = GameObject.FindGameObjectWithTag(tagJugador);

        if (objetivo != null)
        {
            jugador = objetivo.transform;
        }
    }


    // =====================================================
    // MIRAR JUGADOR
    // =====================================================

    private void MirarAlJugador()
    {
        if (jugador == null)
            return;

        float diferenciaX = jugador.position.x - transform.position.x;

        jugadorDetectado = Mathf.Abs(diferenciaX) <= distanciaGiro;

        if (!jugadorDetectado)
            return;


        if (diferenciaX > 0f)
        {
            // JUGADOR A LA DERECHA
            MirarDerecha();
        }
        else if (diferenciaX < 0f)
        {
            // JUGADOR A LA IZQUIERDA
            MirarIzquierda();
        }
    }


    // =====================================================
    // GIRO DEL RIG
    // =====================================================

    private void MirarDerecha()
    {
        mirandoDerecha = true;

        Vector3 escala = escalaOriginal;

        // En tu rig la orientación original está invertida.
        escala.x = -Mathf.Abs(escalaOriginal.x);

        transform.localScale = escala;
    }


    private void MirarIzquierda()
    {
        mirandoDerecha = false;

        Vector3 escala = escalaOriginal;

        escala.x = Mathf.Abs(escalaOriginal.x);

        transform.localScale = escala;
    }


    // =====================================================
    // DISPARAR
    // =====================================================

    private void Disparar()
    {
        if (proyectil == null || controladorDisparo == null)
            return;


        Quaternion rotacion;


        if (mirandoDerecha)
        {
            // Proyectil avanza hacia su Vector2.right
            rotacion = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            // Giramos el prefab 180° para que apunte hacia la izquierda
            rotacion = Quaternion.Euler(0f, 0f, 180f);
        }


        GameObject copia = Instantiate(
            proyectil,
            controladorDisparo.position,
            rotacion
        );

        copia.SetActive(true);
    }


    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmos()
    {
        Vector3 puntoOrigen = controladorDisparo != null ? controladorDisparo.position : transform.position;

        if (girarHaciaJugador)
        {
            Gizmos.color = Color.yellow;

            // La línea se moverá a donde muevas el controladorDisparo
            Gizmos.DrawLine(
                puntoOrigen + Vector3.left * distanciaGiro,
                puntoOrigen + Vector3.right * distanciaGiro
            );
        }


        if (controladorDisparo == null)
            return;


        Vector3 direccion = mirandoDerecha ? Vector3.right : Vector3.left;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            controladorDisparo.position,
            controladorDisparo.position + direccion * distanciaLinea
        );
    }
}