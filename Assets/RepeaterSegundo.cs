using UnityEngine;

public class RepeaterSegundo : MonoBehaviour
{
    [Header("DISPARO")]
    public Transform controladorDisparo;
    [Tooltip("Alcance horizontal a cada lado del enemigo cuando NO hay plataforma debajo.")]
    public float distanciaLinea = 10f;
    public LayerMask capaJugador;
    public bool jugadorEnRango;

    [Header("Rango 360°")]
    [Tooltip("Altura máxima, hacia arriba y hacia abajo del cañón, a la que dispara al jugador.")]
    public float alturaMaxima = 10f;
    [Tooltip("Metros extra a cada lado de las puntas de la plataforma (0 = justo de punta a punta). Con un valor negativo el rango se achica.")]
    public float margenHorizontal = 0f;
    [Tooltip("Hasta dónde busca la plataforma que está debajo del enemigo. Su ancho, de punta a punta, es el rango horizontal.")]
    public float distanciaBusquedaPlataforma = 10f;

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
    private Collider2D colliderJugador;

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
            jugadorEnRango = JugadorEnRango360();
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
            colliderJugador = objetivo.GetComponentInChildren<Collider2D>();
        }
    }


    // =====================================================
    // RANGO 360°
    // =====================================================

    // Punto al que se apunta: el centro del jugador (no sus pies).
    private Vector3 PuntoDeApuntado()
    {
        return colliderJugador != null ? colliderJugador.bounds.center : jugador.position;
    }

    // El jugador está en rango si está entre las dos puntas de la plataforma que hay
    // debajo del enemigo (sin importar si está arriba o abajo) y dentro de la altura máxima.
    private bool JugadorEnRango360()
    {
        Vector3 origen = controladorDisparo.position;
        Vector3 objetivo = PuntoDeApuntado();

        ObtenerLimitesHorizontales(origen, out float minX, out float maxX);

        bool dentroDelTramo = objetivo.x >= minX && objetivo.x <= maxX;
        bool dentroDeLaAltura = Mathf.Abs(objetivo.y - origen.y) <= alturaMaxima;

        return dentroDelTramo && dentroDeLaAltura;
    }

    // Borde izquierdo y derecho de la plataforma de abajo. Si no hay ninguna,
    // se usa distanciaLinea a cada lado del enemigo.
    private void ObtenerLimitesHorizontales(Vector3 origen, out float minX, out float maxX)
    {
        Collider2D plataforma = BuscarPlataformaDebajo(origen);

        if (plataforma != null)
        {
            Bounds limites = plataforma.bounds;
            minX = limites.min.x;
            maxX = limites.max.x;
        }
        else
        {
            minX = origen.x - distanciaLinea;
            maxX = origen.x + distanciaLinea;
        }

        minX -= margenHorizontal;
        maxX += margenHorizontal;

        // Un margen muy negativo no puede dar un rango invertido.
        if (minX > maxX)
        {
            minX = maxX = (minX + maxX) * 0.5f;
        }
    }

    // Primer collider "de plataforma" hacia abajo: se ignoran triggers, el propio enemigo,
    // al jugador y todo lo que tenga un Rigidbody2D dinámico (otros enemigos, objetos sueltos).
    private Collider2D BuscarPlataformaDebajo(Vector3 origen)
    {
        RaycastHit2D[] impactos = Physics2D.RaycastAll(origen, Vector2.down, distanciaBusquedaPlataforma);

        Collider2D masCercano = null;
        float distanciaMinima = float.MaxValue;

        for (int i = 0; i < impactos.Length; i++)
        {
            Collider2D col = impactos[i].collider;

            if (col == null || col.isTrigger)
                continue;

            if (col.transform.IsChildOf(transform) || col.CompareTag(tagJugador))
                continue;

            Rigidbody2D rb = col.attachedRigidbody;

            if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
                continue;

            if (impactos[i].distance < distanciaMinima)
            {
                distanciaMinima = impactos[i].distance;
                masCercano = col;
            }
        }

        return masCercano;
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


        // Dirección del disparo: hacia el jugador, en cualquier ángulo. Si dispara "siempre"
        // (sin depender del jugador) sale en horizontal, hacia donde mira el enemigo.
        Vector2 direccion = mirandoDerecha ? Vector2.right : Vector2.left;

        if (!dispararSiempre && jugador != null)
        {
            Vector2 haciaJugador = PuntoDeApuntado() - controladorDisparo.position;

            if (haciaJugador.sqrMagnitude > 0.0001f)
                direccion = haciaJugador;
        }

        // El proyectil avanza hacia su Vector2.right: rotándolo al crearlo, vuela recto
        // en esa dirección (no persigue, la dirección queda fija al disparar).
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        Quaternion rotacion = Quaternion.Euler(0f, 0f, angulo);


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


        // Rango 360°: de punta a punta de la plataforma de abajo, y hasta alturaMaxima arriba/abajo.
        ObtenerLimitesHorizontales(controladorDisparo.position, out float minX, out float maxX);

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(
            new Vector3((minX + maxX) * 0.5f, controladorDisparo.position.y, controladorDisparo.position.z),
            new Vector3(maxX - minX, alturaMaxima * 2f, 0f)
        );
    }
}