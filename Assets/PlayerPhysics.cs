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

    [Header("Sprite")]
    [Tooltip("Marcá esto si el personaje mira hacia la derecha con escala X positiva. Desmarcá si mira a la izquierda.")]
    [SerializeField] private bool spriteMiraDerecha = true;
    [Tooltip("Objeto que se gira al cambiar de direccion. Si lo dejás vacío se usa este mismo GameObject " +
             "(la raíz del rig). Se invierte su escala en X: espeja todo el rig sin cambiar su tamaño.")]
    [SerializeField] private Transform visualAGirar;

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D colisionadorJugador;
    private Transform transformAGirar;
    private Vector3 escalaAGirarInicial;
    private bool estaEnElSuelo;
    private float inputHorizontal;
    private bool quiereSaltar;

    private bool estaDasheando;
    private float tiempoRestanteDash;
    private float tiempoRestanteCooldown;
    private float direccionDash;

    // ---- Propiedades públicas para la UI del cooldown ----
    public float ProgresoCooldownDash
    {
        get
        {
            if (cooldownDash <= 0f) return 1f;
            return 1f - Mathf.Clamp01(tiempoRestanteCooldown / cooldownDash);
        }
    }

    public float TiempoRestanteCooldown => tiempoRestanteCooldown;
    public bool PuedeDashear => tiempoRestanteCooldown <= 0f;
    // --------------------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Sin fricción: evita que el jugador quede "pegado" al collider de una
        // plataforma cuando el input lo empuja contra ella. El movimiento ya lo
        // controlamos 100% seteando la velocidad a mano, así que la fricción no
        // hace falta para nada y solo generaba ese enganche.
        colisionadorJugador = GetComponent<Collider2D>();
        if (colisionadorJugador != null)
        {
            colisionadorJugador.sharedMaterial = new PhysicsMaterial2D("PlayerSinFriccion")
            {
                friction = 0f,
                bounciness = 0f
            };
        }

        // Si no se asigna nada, se gira este mismo objeto (la raíz del rig).
        transformAGirar = (visualAGirar != null) ? visualAGirar : transform;
        escalaAGirarInicial = transformAGirar.localScale;
    }

    private void OrientarSprite()
    {
        if (inputHorizontal == 0f) return;

        bool mirandoDerecha = inputHorizontal > 0f;

        // Espeja el rig invirtiendo el signo de la escala en X (no cambia su tamaño).
        float signo = (mirandoDerecha == spriteMiraDerecha) ? 1f : -1f;

        Vector3 escala = transformAGirar.localScale;
        float objetivoX = Mathf.Abs(escalaAGirarInicial.x) * signo;
        if (!Mathf.Approximately(escala.x, objetivoX))
        {
            escala.x = objetivoX;
            transformAGirar.localScale = escala;
        }
    }

    // El punto de chequeo queda siempre centrado en X bajo el personaje (usa
    // transform.position, que no cambia al girar). La altura Y se toma del
    // borde inferior REAL del collider del jugador en este mismo frame, no de
    // un Transform calibrado a mano: así da igual el alto/grosor de cada
    // plataforma, siempre se compara contra donde están literalmente los pies.
    // (Antes, al ser detectorSuelo hijo del objeto que espejamos en
    // OrientarSprite(), su X se corría al cambiar de lado y en las esquinas
    // quedaba fuera de la plataforma aunque el personaje siguiera apoyado.)
    private Vector2 PuntoDeteccionSuelo()
    {
        float y = colisionadorJugador != null ? colisionadorJugador.bounds.min.y : detectorSuelo.position.y;
        return new Vector2(transform.position.x, y);
    }

    void Update()
    {
        // Detección de suelo
        estaEnElSuelo = Physics2D.OverlapCircle(PuntoDeteccionSuelo(), radioDeteccion, capaPlataformas);

        // Animación de Salto y Caida
        anim.SetBool("enSuelo", !estaEnElSuelo);
        anim.SetFloat("velocidadY", rb.linearVelocity.y);

        if (estaEnElSuelo)
        {
            anim.SetFloat("velocidadY", 0f);
        }
        else
        {
            anim.SetFloat("velocidadY", rb.linearVelocity.y);
        }

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
            anim.SetBool("mover", true);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            inputHorizontal = -1f;
            anim.SetBool("mover", true);
        }
        else
        {
            anim.SetBool("mover", false);
        }

        // Orientar el sprite segun la ultima direccion pulsada (D = derecha, A = izquierda)
        OrientarSprite();

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

            anim.SetTrigger("dash");
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
            Gizmos.DrawWireSphere(PuntoDeteccionSuelo(), radioDeteccion);
        }
    }
}