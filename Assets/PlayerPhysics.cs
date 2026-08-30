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
    [Tooltip("Gravedad durante la subida. Mayor a 1 = llega al punto más alto más rápido, sin cambiar la altura del salto.")]
    [SerializeField] private float multiplicadorSubida = 2f;

    [Header("Dash")]
    [SerializeField] private float fuerzaDash = 15f;
    [SerializeField] private float duracionDash = 0.2f;
    [SerializeField] private float cooldownDash = 1f;
    [SerializeField] private float velocidadCaidaDespuesDash = 2f;

    private Rigidbody2D rb;
    private bool estaEnElSuelo;
    private float inputHorizontal;
    private bool quiereSaltar;

    private bool estaDasheando;
    private float tiempoRestanteDash;
    private float tiempoRestanteCooldown;
    private float direccionDash;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Detección de suelo
        estaEnElSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaPlataformas);

        // Gravedad segun la fase del salto
        if (rb.linearVelocity.y > 0.01f && !estaEnElSuelo)
        {
            rb.gravityScale = multiplicadorSubida;   // subida: apex mas rapido
        }
        else if (rb.linearVelocity.y < 0f && !estaEnElSuelo)
        {
            rb.gravityScale = multiplicadorCaida;    // caida: igual que antes
        }
        else
        {
            rb.gravityScale = 1f;
        }

        // Movimiento horizontal (A = izquierda, D = derecha)
        inputHorizontal = 0f;
        if (Input.GetKey(KeyCode.D))
        {
            inputHorizontal = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            inputHorizontal = -1f;
        }

        // Salto (Espacio o W), solo si está en el suelo
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && estaEnElSuelo)
        {
            quiereSaltar = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Tecla de salto detectada. estaEnElSuelo = " + estaEnElSuelo);
        }
        // Cooldown del dash
        if (tiempoRestanteCooldown > 0f)
        {
            tiempoRestanteCooldown -= Time.deltaTime;
        }
        if (estaDasheando)
        {
            tiempoRestanteDash -= Time.deltaTime;

            if (tiempoRestanteDash <= 0f)
            {
                estaDasheando = false;

                rb.gravityScale = multiplicadorCaida;

                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    -velocidadCaidaDespuesDash
                );
            }
        }

        // Dash (Shift), solo si A o D están sostenidas
        // Dash (Shift), solo si A o D están sostenidas
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("SHIFT presionado | inputHorizontal = " + inputHorizontal + " | estaDasheando = " + estaDasheando + " | cooldown restante = " + tiempoRestanteCooldown);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && inputHorizontal != 0f && !estaDasheando && tiempoRestanteCooldown <= 0f)
        {
            estaDasheando = true;
            tiempoRestanteDash = duracionDash;
            tiempoRestanteCooldown = cooldownDash;
            direccionDash = inputHorizontal;
            Debug.Log("DASH ACTIVADO, dirección = " + direccionDash);
        }
    }

    void FixedUpdate()
    {
        if (estaDasheando)
        {
            rb.linearVelocity = new Vector2(
                direccionDash * fuerzaDash,
                0f
            );
        }
        else
        {
            rb.linearVelocity = new Vector2(
                inputHorizontal * velocidadMovimiento,
                rb.linearVelocity.y
            );
        }

        if (quiereSaltar)
        {
            float velSalto = fuerzaSalto * Mathf.Sqrt(multiplicadorSubida);

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                velSalto
            );

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