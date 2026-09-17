using UnityEngine;

public class RepeaterSegundo : MonoBehaviour
{
    [Header("DISPARO")]
    public Transform controladorDisparo;
    public float distanciaLinea = 10f;
    public LayerMask capaJugador;
    public bool jugadorEnRango;

    [Header("Configuración de Disparo")]
    public GameObject proyectil;
    public float tiempoEntreDisparos = 1.5f;

    [Header("Comportamiento")]
    [Tooltip("Si está activado, dispara siempre sin necesitar raycast.")]
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
        // MIRAR AL JUGADOR
        // ============================

        if (girarHaciaJugador)
        {
            MirarAlJugador();
        }

        if (controladorDisparo == null)
            return;


        // ============================
        // DETECCIÓN
        // ============================

        if (dispararSiempre)
        {
            jugadorEnRango = true;
        }
        else
        {
            Vector2 direccion = mirandoDerecha
                ? Vector2.right
                : Vector2.left;

            bool originalSetting =
                Physics2D.queriesStartInColliders;

            Physics2D.queriesStartInColliders = false;

            RaycastHit2D hit = Physics2D.Raycast(
                controladorDisparo.position,
                direccion,
                distanciaLinea,
                capaJugador
            );

            Physics2D.queriesStartInColliders =
                originalSetting;

            jugadorEnRango = hit.collider != null;
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
            cronometro = 0f;
        }
    }


    // =====================================================
    // BUSCAR JUGADOR
    // =====================================================

    private void BuscarJugador()
    {
        GameObject objetivo =
            GameObject.FindGameObjectWithTag(tagJugador);

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
        {
            BuscarJugador();

            if (jugador == null)
                return;
        }

        float diferenciaX =
            jugador.position.x - transform.position.x;

        jugadorDetectado =
            Mathf.Abs(diferenciaX) <= distanciaGiro;

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

        // IMPORTANTE:
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
        if (proyectil == null ||
            controladorDisparo == null)
            return;


        Quaternion rotacion;


        if (mirandoDerecha)
        {
            // Proyectil.cs avanza hacia su Vector2.right
            rotacion = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            // Giramos el prefab 180° para que su
            // Vector2.right apunte hacia la izquierda
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
        if (girarHaciaJugador)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawLine(
                transform.position +
                Vector3.left * distanciaGiro,

                transform.position +
                Vector3.right * distanciaGiro
            );
        }


        if (controladorDisparo == null)
            return;


        Vector3 direccion =
            mirandoDerecha
            ? Vector3.right
            : Vector3.left;


        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            controladorDisparo.position,

            controladorDisparo.position +
            direccion * distanciaLinea
        );
    }
}