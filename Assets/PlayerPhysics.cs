using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [Header("Configuración de Caída")]

    [SerializeField] private float multiplicadorCaida = 2.5f;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform detectorSuelo;
    [SerializeField] private float radioDeteccion = 0.2f;
    [SerializeField] private LayerMask capaPlataformas;

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 8f;

    private Rigidbody2D rb;
    private bool estaEnElSuelo;
    private float inputHorizontal;
    private bool quiereSaltar;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        estaEnElSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaPlataformas);


        if (rb.linearVelocity.y < 0 && !estaEnElSuelo)
        {

            rb.gravityScale = multiplicadorCaida;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        // Lectura del input de movimiento horizontal (A = izquierda, D = derecha)
        inputHorizontal = 0f;
        if (Input.GetKey(KeyCode.D))
        {
            inputHorizontal = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            inputHorizontal = -1f;
        }

        // Lectura del input de salto
        if (Input.GetKeyDown(KeyCode.Space) && estaEnElSuelo)
        {
            quiereSaltar = true;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);

        if (quiereSaltar)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            quiereSaltar = false;
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