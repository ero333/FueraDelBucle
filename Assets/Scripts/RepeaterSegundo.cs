using UnityEngine;
using System.Collections;
public class RepeaterSegundo : MonoBehaviour
{
    [Header("DISPARO")]
    public Transform controladorDisparo;
    [Tooltip("Hasta dónde llega la línea de disparo, contada desde el cañón.")]
    public float distanciaLinea = 10f;
    public LayerMask capaJugador;
    public bool jugadorEnRango;

    //aturdido
    [Header("Aturdimiento al pisar")]
    [Tooltip("Segundos que el enemigo se queda sin disparar al ser pisado.")]
    public float tiempoAturdido = 3f;

    [Tooltip("Fuerza con la que rebota el jugador hacia arriba al pisarlo.")]
    public float fuerzaRebote = 10f;
    private bool aturdido = false;
    //aturdido

    [Header("Rango de detección")]
    [Tooltip("Diferencia de altura que perdona. Si el jugador está más arriba o más abajo que esto, no lo ve.")]
    public float alturaMaxima = 1.5f;

    [Tooltip("Solo detecta al jugador del lado hacia el que está mirando.")]
    public bool soloAdelante = true;

    [Header("Configuración de Disparo")]
    public GameObject proyectil;
    public float tiempoEntreDisparos = 1.5f;

    [Tooltip("Segundos hasta el primer disparo cuando el jugador entra al rango. En 0 dispara apenas lo detecta.")]
    public float esperaInicial = 0f;

    [Tooltip("Capas contra las que se rompe el disparo de este enemigo (plataformas, suelo). Vacío = atraviesa todo.")]
    public LayerMask capasQueFrenanElDisparo;

    [Tooltip("Segundos que vive el disparo de este enemigo. En 0 se respeta el valor que trae el proyectil.")]
    public float vidaDelDisparo = 0f;

    [Header("Comportamiento")]
    [Tooltip("Si está activado, dispara siempre sin depender del jugador.")]
    public bool dispararSiempre = false;

    [Tooltip("Activado: el disparo sale hacia el jugador en cualquier ángulo. Desactivado: sale en línea recta hacia donde mira el enemigo.")]
    public bool apuntarAlJugador = false;

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
        cronometro = esperaInicial;

        escalaOriginal = transform.localScale;

        BuscarJugador();
    }

    private void Update()
    {
        //aturdido
        if (aturdido)
            return;
        //aturdido
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
            jugadorEnRango = JugadorEnLaLinea();
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
            cronometro = esperaInicial;
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
    // LINEA DE DISPARO
    // =====================================================

    // Punto al que se apunta: el centro del jugador (no sus pies).
    private Vector3 PuntoDeApuntado()
    {
        return colliderJugador != null ? colliderJugador.bounds.center : jugador.position;
    }

    // El jugador está en rango cuando pasa por delante del cañón: a la misma altura
    // (con lo que perdona alturaMaxima) y dentro de distanciaLinea hacia adelante.
    private bool JugadorEnLaLinea()
    {
        Vector3 origen = controladorDisparo.position;
        Vector3 objetivo = PuntoDeApuntado();

        if (Mathf.Abs(objetivo.y - origen.y) > alturaMaxima) return false;

        float diferenciaX = objetivo.x - origen.x;

        if (Mathf.Abs(diferenciaX) > distanciaLinea) return false;

        if (!soloAdelante) return true;

        return mirandoDerecha ? diferenciaX >= 0f : diferenciaX <= 0f;
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

        if (apuntarAlJugador && !dispararSiempre && jugador != null)
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

        Proyectil datos = copia.GetComponent<Proyectil>();

        if (datos != null)
        {
            datos.capasQueFrenan = capasQueFrenanElDisparo;

            if (vidaDelDisparo > 0f)
                datos.tiempoDeVida = vidaDelDisparo;
        }

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


        // Franja que ve el cañón: distanciaLinea hacia adelante y alturaMaxima arriba/abajo.
        Vector3 centro = controladorDisparo.position;
        float ancho = distanciaLinea * 2f;

        if (soloAdelante)
        {
            float lado = Application.isPlaying && !mirandoDerecha ? -1f : 1f;
            centro += Vector3.right * (distanciaLinea * 0.5f * lado);
            ancho = distanciaLinea;
        }

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(centro, new Vector3(ancho, alturaMaxima * 2f, 0f));
    }

    //Aturdimiento 

    
    private Coroutine corrutinaAturdimiento;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagJugador))
        {
            
            foreach (ContactPoint2D punto in collision.contacts)
            {
                if (punto.normal.y < -0.5f)
                {
                    
                    Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (rbJugador != null)
                    {
                        rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, fuerzaRebote);
                    }

                    
                    if (corrutinaAturdimiento != null)
                    {
                        StopCoroutine(corrutinaAturdimiento);
                    }

                    
                    corrutinaAturdimiento = StartCoroutine(AturdirEnemigo());
                    break;
                }
            }
        }
    }

    private IEnumerator AturdirEnemigo()
    {
        aturdido = true;
        jugadorEnRango = false;

        yield return new WaitForSeconds(tiempoAturdido);

        aturdido = false;
        cronometro = esperaInicial;
        corrutinaAturdimiento = null; 
    }
}