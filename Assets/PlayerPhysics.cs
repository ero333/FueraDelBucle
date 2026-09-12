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
    [SerializeField] private float velocidadAgachado = 2.5f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 8f;
    [Tooltip("Gravedad durante la subida. Mayor a 1 = llega al punto más alto más rápido, sin cambiar la altura del salto.")]
    [SerializeField] private float multiplicadorSubida = 2f;

    [Header("Dash")]
    [SerializeField] private float fuerzaDash = 15f;
    [SerializeField] private float duracionDash = 0.2f;
    [SerializeField] private float cooldownDash = 1f;
    [SerializeField] private float velocidadCaidaDespuesDash = 2f;

    [Header("Habilidad Phase (Traspasar Paredes)")]
    [SerializeField] private KeyCode teclaPhase = KeyCode.P;
    [SerializeField] private float cooldownPhase = 3f;
    [SerializeField] private LayerMask capaParedesAtravesables;
    [SerializeField] private float opacidadPhase = 0.5f; // Transparencia visual durante el Phase

    [Header("Sprite")]
    [Tooltip("Marcá esto si el personaje mira hacia la derecha con escala X positiva. Desmarcá si mira a la izquierda.")]
    [SerializeField] private bool spriteMiraDerecha = true;
    [Tooltip("Objeto que se gira al cambiar de direccion. Si lo dejás vacío se usa este mismo GameObject " +
             "(la raíz del rig). Se invierte su escala en X: espeja todo el rig sin cambiar su tamaño.")]
    [SerializeField] private Transform visualAGirar;

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D colisionadorJugador;
    private SpriteRenderer spriteRenderer;
    private Transform transformAGirar;
    private Vector3 escalaAGirarInicial;
    private bool estaEnElSuelo;
    private float inputHorizontal;
    private bool quiereSaltar;

    private bool estaDasheando;
    private float tiempoRestanteDash;
    private float tiempoRestanteCooldown;
    private float direccionDash;

    // Variables Phase
    private bool estaEnPhase = false;
    private float tiempoRestanteCooldownPhase;

    private bool estaAgachado;
    private BoxCollider2D boxCollider;
    private Vector2 tamanoOriginalCollider;
    private Vector2 offsetOriginalCollider;

    // ---- Propiedades públicas para la UI ----
    public float ProgresoCooldownDash
    {
        get
        {
            if (cooldownDash <= 0f) return 1f;
            return 1f - Mathf.Clamp01(tiempoRestanteCooldown / cooldownDash);
        }
    }

    public float ProgresoCooldownPhase
    {
        get
        {
            if (cooldownPhase <= 0f) return 1f;
            return 1f - Mathf.Clamp01(tiempoRestanteCooldownPhase / cooldownPhase);
        }
    }

    public float TiempoRestanteCooldown => tiempoRestanteCooldown;
    public bool PuedeDashear => tiempoRestanteCooldown <= 0f;
    public bool PuedeHacerPhase => tiempoRestanteCooldownPhase <= 0f && !estaEnPhase;
    public bool EstaEnElSuelo => estaEnElSuelo;

    [Header("Fuerza de Rebote sobre el enemigo")]
    public float Rebote = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        colisionadorJugador = GetComponent<Collider2D>();
        if (colisionadorJugador != null)
        {
            colisionadorJugador.sharedMaterial = new PhysicsMaterial2D("PlayerSinFriccion")
            {
                friction = 0f,
                bounciness = 0f
            };
        }

        boxCollider = colisionadorJugador as BoxCollider2D;
        if (boxCollider != null)
        {
            tamanoOriginalCollider = boxCollider.size;
            offsetOriginalCollider = boxCollider.offset;
        }

        transformAGirar = (visualAGirar != null) ? visualAGirar : transform;
        escalaAGirarInicial = transformAGirar.localScale;
    }

    private void OrientarSprite()
    {
        if (inputHorizontal == 0f) return;

        bool mirandoDerecha = inputHorizontal > 0f;
        float signo = (mirandoDerecha == spriteMiraDerecha) ? 1f : -1f;

        Vector3 escala = transformAGirar.localScale;
        float objetivoX = Mathf.Abs(escalaAGirarInicial.x) * signo;
        if (!Mathf.Approximately(escala.x, objetivoX))
        {
            escala.x = objetivoX;
            transformAGirar.localScale = escala;
        }
    }

    private Vector2 PuntoDeteccionSuelo()
    {
        float y = colisionadorJugador != null ? colisionadorJugador.bounds.min.y : detectorSuelo.position.y;
        return new Vector2(transform.position.x, y);
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        estaEnElSuelo = Physics2D.OverlapCircle(PuntoDeteccionSuelo(), radioDeteccion, capaPlataformas);

        if (Input.GetKey(KeyCode.S) && estaEnElSuelo && !estaDasheando)
        {
            estaAgachado = true;
        }
        else
        {
            estaAgachado = false;
        }

        AjustarColliderAgachado();

        anim.SetBool("agachado", estaAgachado);
        anim.SetBool("enSuelo", !estaEnElSuelo);
        anim.SetFloat("velocidadY", estaEnElSuelo ? 0f : rb.linearVelocity.y);

        // Gravedad según la fase del salto
        if (rb.linearVelocity.y > 0.01f && !estaEnElSuelo)
        {
            rb.gravityScale = multiplicadorSubida;
        }
        else if (rb.linearVelocity.y < 0f && !estaEnElSuelo)
        {
            rb.gravityScale = multiplicadorCaida;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        // Movimiento horizontal
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

        OrientarSprite();

        // Salto
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && estaEnElSuelo && !estaAgachado)
        {
            quiereSaltar = true;
        }

        // Cooldown Dash
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
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -velocidadCaidaDespuesDash);
            }
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && inputHorizontal != 0f && !estaDasheando && tiempoRestanteCooldown <= 0f && !estaAgachado)
        {
            estaDasheando = true;
            tiempoRestanteDash = duracionDash;
            tiempoRestanteCooldown = cooldownDash;
            direccionDash = inputHorizontal;
            anim.Play("DASH", 0, 0f);
        }

        // Cooldown del Phase
        if (tiempoRestanteCooldownPhase > 0f)
        {
            tiempoRestanteCooldownPhase -= Time.deltaTime;
        }

        // Activa el Phase mientras se sostiene la tecla P
        if (Input.GetKey(teclaPhase) && tiempoRestanteCooldownPhase <= 0f)
        {
            if (!estaEnPhase)
            {
                ActivarPhase();
            }
        }

        // Desactiva el Phase al soltar la tecla P
        if (Input.GetKeyUp(teclaPhase) && estaEnPhase)
        {
            DesactivarPhase();
            tiempoRestanteCooldownPhase = cooldownPhase; // Inicia el cooldown
        }
    }

    void FixedUpdate()
    {
        if (estaDasheando)
        {
            rb.linearVelocity = new Vector2(direccionDash * fuerzaDash, 0f);
        }
        else
        {
            float velActual = estaAgachado ? velocidadAgachado : velocidadMovimiento;
            rb.linearVelocity = new Vector2(inputHorizontal * velActual, rb.linearVelocity.y);
        }

        if (quiereSaltar)
        {
            float velSalto = fuerzaSalto * Mathf.Sqrt(multiplicadorSubida);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, velSalto);
            quiereSaltar = false;
        }
    }

    private void ActivarPhase()
    {
        estaEnPhase = true;

        // Desactiva colisiones con la capa asignada
        IgnorarColisionesParedes(true);

        // Feedback visual (Semi-transparente)
        if (spriteRenderer != null)
        {
            Color colorActual = spriteRenderer.color;
            colorActual.a = opacidadPhase;
            spriteRenderer.color = colorActual;
        }
    }

    private void DesactivarPhase()
    {
        estaEnPhase = false;

        // Restablece colisiones
        IgnorarColisionesParedes(false);

        // Restaura opacidad completa
        if (spriteRenderer != null)
        {
            Color colorActual = spriteRenderer.color;
            colorActual.a = 1f;
            spriteRenderer.color = colorActual;
        }
    }

    private void IgnorarColisionesParedes(bool ignorar)
    {
        int playerLayer = gameObject.layer;

        for (int i = 0; i < 32; i++)
        {
            if ((capaParedesAtravesables.value & (1 << i)) != 0)
            {
                Physics2D.IgnoreLayerCollision(playerLayer, i, ignorar);
            }
        }
    }

    private void AjustarColliderAgachado()
    {
        if (boxCollider == null) return;

        if (estaAgachado)
        {
            boxCollider.size = new Vector2(tamanoOriginalCollider.x, tamanoOriginalCollider.y * 0.5f);
            boxCollider.offset = new Vector2(offsetOriginalCollider.x, offsetOriginalCollider.y - (tamanoOriginalCollider.y * 0.25f));
        }
        else
        {
            boxCollider.size = tamanoOriginalCollider;
            boxCollider.offset = offsetOriginalCollider;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CabezaEnemigo") && rb.linearVelocity.y <= 0f)
        {
            Rebotar();
        }
    }

    private void Rebotar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Rebote);
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