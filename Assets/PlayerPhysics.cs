using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [Header("Configuración de Caída")]
    // Ajustar velocidad de caída
    [SerializeField] private float multiplicadorCaida = 2.5f;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform detectorSuelo;
    [SerializeField] private float radioDeteccion = 0.2f;
    [SerializeField] private LayerMask capaPlataformas;

    private Rigidbody2D rb;
    private bool estaEnElSuelo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Verificar caída desde plataformas
        estaEnElSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaPlataformas);

        // Evitar flotación / Ajustar velocidad de caída
        if (rb.linearVelocity.y < 0 && !estaEnElSuelo)
        {
            // Programar gravedad del personaje
            rb.gravityScale = multiplicadorCaida;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectorSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion);
        }
    }
}
