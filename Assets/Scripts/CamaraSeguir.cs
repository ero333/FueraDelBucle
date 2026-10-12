using System.Collections;
using UnityEngine;

public class CamaraSeguir : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo;
    public Vector2 desplazamiento = new Vector2(0f, 1f);

    [Header("Suavizado")]
    [Range(0f, 1f)]
    public float suavizado = 0.15f;

    [Header("Limites del nivel")]
    public bool usarLimites = true;
    public float limiteIzquierdo = 0f;
    public float limiteDerecho = 86.5f;
    public float limiteAbajo = -4f;
    public float limiteArriba = 2f;

    [Header("Caída libre")]
    [Tooltip("PlayerPhysics del objetivo. Si se asigna, la cámara deja de bajar cuando el jugador cae más de 'Distancia Seguimiento Caida' sin tocar el suelo, para que la muerte por caída pase fuera de pantalla.")]
    public PlayerPhysics jugador;
    [Tooltip("Cuánto puede caer el jugador en el aire antes de que la cámara se quede fija en Y.")]
    public float distanciaSeguimientoCaida = 3f;

    private Vector3 velocidadActual;
    private bool cayendoLibre;
    private float yInicioCaida;

    // Paneo de presentación (ver Paneo): mientras la cámara se mueve sola no sigue al objetivo.
    private bool paneando;

    // Instante que sigue al aterrizaje antes de que arranque el paneo, para que la cámara termine de
    // acomodarse sobre el jugador y el movimiento empiece desde quieto.
    private const float DemoraInicialPaneo = 0.4f;

    // Verdadero desde que el jugador llega al lugar de la presentación hasta que termina el paneo.
    // PlayerPhysics lo lee para dejar al jugador quieto (en Idle); el juego no se pausa.
    public static bool PaneoActivo { get; private set; }

    private void LateUpdate()
    {
        if (objetivo == null || paneando) return;

        Vector3 destino = new Vector3(
            objetivo.position.x + desplazamiento.x,
            objetivo.position.y + desplazamiento.y,
            transform.position.z
        );

        if (usarLimites)
        {
            destino.x = Mathf.Clamp(destino.x, limiteIzquierdo, limiteDerecho);
            destino.y = Mathf.Clamp(destino.y, limiteAbajo, limiteArriba);
        }

        destino.y = LimitarCaidaLibre(destino.y);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            destino,
            ref velocidadActual,
            suavizado
        );
    }

    // Lleva la cámara hasta 'foco' con un movimiento suave (arranca y frena de a poco), la deja ahí
    // 'segundosEnFoco' y después la devuelve al objetivo, también suave. Mientras dura, la cámara no
    // sigue al objetivo y PaneoActivo es verdadero. No pausa el juego.
    // 'alTerminar' (opcional) se llama justo cuando la cámara termina de volver al objetivo, todavía dentro del
    // paneo: lo que arranque ahí empalma sin que el jugador se pueda mover entre una cosa y la otra.
    public void Paneo(Transform foco, float duracionIda, float segundosEnFoco, float duracionVuelta, System.Action alTerminar = null)
    {
        if (PaneoActivo || foco == null) return;

        StartCoroutine(RutinaPaneo(foco, duracionIda, segundosEnFoco, duracionVuelta, alTerminar));
    }

    private IEnumerator RutinaPaneo(Transform foco, float duracionIda, float segundosEnFoco, float duracionVuelta, System.Action alTerminar)
    {
        PaneoActivo = true;

        try
        {
            // La cámara sigue acomodándose sobre el jugador un instante antes de empezar.
            for (float t = 0f; t < DemoraInicialPaneo; t += Time.deltaTime)
                yield return null;

            paneando = true;

            Vector3 origen = transform.position;
            Vector3 posicionFoco = foco.position;

            // Ida hacia el foco.
            for (float t = 0f; t < duracionIda; t += Time.deltaTime)
            {
                if (foco != null) posicionFoco = foco.position;

                transform.position = Vector3.Lerp(origen, PosicionMirando(posicionFoco), Suavizado(t / duracionIda));
                yield return null;
            }

            transform.position = PosicionMirando(posicionFoco);

            // Se queda mirando el foco.
            for (float t = 0f; t < segundosEnFoco; t += Time.deltaTime)
                yield return null;

            // Vuelta al objetivo (si ya no hay objetivo, vuelve a donde estaba).
            Vector3 desde = transform.position;

            for (float t = 0f; t < duracionVuelta; t += Time.deltaTime)
            {
                Vector3 destino = objetivo != null ? PosicionMirando(objetivo.position) : origen;

                transform.position = Vector3.Lerp(desde, destino, Suavizado(t / duracionVuelta));
                yield return null;
            }

            transform.position = objetivo != null ? PosicionMirando(objetivo.position) : origen;

            if (alTerminar != null) alTerminar();
        }
        finally
        {
            // Queda justo donde la dejaría el seguimiento, así retoma sin tirones.
            velocidadActual = Vector3.zero;
            paneando = false;
            PaneoActivo = false;
        }
    }

    // Curva suave que arranca y termina despacio (0 a 1).
    private static float Suavizado(float k)
    {
        k = Mathf.Clamp01(k);

        return k * k * k * (k * (k * 6f - 15f) + 10f);
    }

    // Dónde queda la cámara al mirar 'posicion': igual que al seguir al objetivo (mismo
    // desplazamiento y mismos límites del nivel).
    private Vector3 PosicionMirando(Vector3 posicion)
    {
        Vector3 destino = new Vector3(
            posicion.x + desplazamiento.x,
            posicion.y + desplazamiento.y,
            transform.position.z
        );

        if (usarLimites)
        {
            destino.x = Mathf.Clamp(destino.x, limiteIzquierdo, limiteDerecho);
            destino.y = Mathf.Clamp(destino.y, limiteAbajo, limiteArriba);
        }

        return destino;
    }

    private void OnDisable()
    {
        // Si la cámara se apaga en medio del paneo, no se queda trabado el jugador.
        if (paneando || PaneoActivo)
        {
            paneando = false;
            PaneoActivo = false;
        }
    }

    // Mientras el jugador esté en el suelo sigue en Y sin restricción. Apenas
    // deja de tocar el suelo (salto o se cae de una plataforma) se guarda el Y
    // en el que empezó a estar en el aire; a partir de ahí la cámara puede
    // seguir bajando hasta esa marca menos 'distanciaSeguimientoCaida', y de
    // ahí no baja más aunque el jugador siga cayendo. Al volver a tocar el
    // suelo (aterriza en otra plataforma) se libera y sigue normal de nuevo.
    private float LimitarCaidaLibre(float destinoY)
    {
        if (jugador == null) return destinoY;

        if (jugador.EstaEnElSuelo)
        {
            cayendoLibre = false;
            return destinoY;
        }

        if (!cayendoLibre)
        {
            cayendoLibre = true;
            yInicioCaida = destinoY;
        }

        float pisoDeSeguimiento = yInicioCaida - distanciaSeguimientoCaida;
        return Mathf.Max(destinoY, pisoDeSeguimiento);
    }
}
