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

    private void LateUpdate()
    {
        if (objetivo == null) return;

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
