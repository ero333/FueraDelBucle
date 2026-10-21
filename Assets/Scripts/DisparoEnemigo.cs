using System.Collections;
using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{
    public Transform controladorDisparo;
    public float distanciaLinea = 10f;
    public LayerMask capaJugador;
    public bool jugadorEnRango;

    [Header("Configuración de Disparo")]
    public GameObject proyectil;
    public float tiempoEntreDisparos = 1.5f;

    [Header("Comportamiento")]
    [Tooltip("Si está activado, dispara desde el arranque en bucle sin necesitar detectar al jugador con el raycast.")]
    public bool dispararSiempre = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip muerte;

    // Segundos que dura el destello de la luz roja al disparar.
    private const float DuracionDestello = 0.15f;

    private float carga;
    private float tiempoDeDestello;
    private LuzRoja luzAviso;

    private Animator anim;
    private bool estamuerto = false;

    private Rigidbody2D rb;
    private Collider2D col2D;
    private void Start()
    {

        anim = GetComponent<Animator>();
        luzAviso = GetComponentInChildren<LuzRoja>(true);
        carga = tiempoEntreDisparos; // listo para disparar apenas vea al jugador

        // Congela la rotación física: sin esto, un empujón del jugador le mete
        // torque y lo hace girar sin control como un trompo (y la caída se ve
        // distinta y errática según el ángulo del golpe).
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Update()
    {
        if (estamuerto) return;

        if (controladorDisparo == null) return;

        if (dispararSiempre)
        {
            jugadorEnRango = true;
        }
        else
        {
            bool originalSetting = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = false;


            RaycastHit2D hit = Physics2D.Raycast(controladorDisparo.position, transform.right, distanciaLinea, capaJugador);


            Physics2D.queriesStartInColliders = originalSetting;

            jugadorEnRango = hit.collider != null;
        }

        // La carga (segundos desde el último disparo) sigue subiendo aunque el jugador salga de la línea, más rápido:
        // así no salen dos balas muy seguidas, pero tampoco hay que esperar el intervalo entero al volver a entrar.
        carga = Mathf.Min(tiempoEntreDisparos, carga + Time.deltaTime * (jugadorEnRango ? 1f : 4f));

        // Apenas detecta al jugador (y el enfriamiento terminó) dispara directo; después empieza el enfriamiento.
        if (jugadorEnRango && carga >= tiempoEntreDisparos)
        {
            Disparar();
            carga = 0f;
            tiempoDeDestello = DuracionDestello;
        }

        // La luz roja hace un destello corto al disparar.
        if (tiempoDeDestello > 0f)
            tiempoDeDestello -= Time.deltaTime;

        PrenderAviso(tiempoDeDestello > 0f);
    }

    private void PrenderAviso(bool encendida)
    {
        if (luzAviso == null || luzAviso.luzRoja == null) return;

        if (luzAviso.luzRoja.activeSelf != encendida)
            luzAviso.luzRoja.SetActive(encendida);
    }

    private void Disparar()
    {
        if (proyectil == null) return;

        GameObject copia = Instantiate(proyectil, controladorDisparo.position, controladorDisparo.rotation);
        copia.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        if (controladorDisparo == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(controladorDisparo.position, controladorDisparo.position + transform.right * distanciaLinea);
    }

    public void Muere()
    {
        if (estamuerto) return;
        StartCoroutine(DestruirEnemigo());
    }

    IEnumerator DestruirEnemigo()
    {
        estamuerto = true;
        PrenderAviso(false);

        if (muerte != null)
        {
            Volumen.SonarEfecto(muerte, 1f);
        }

        if (col2D != null) col2D.enabled = false;
        if (rb != null) rb.simulated = false;

        anim.SetTrigger("muerteRepeater");

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    //Dejo esta función abajo por las dudas que falle algo de la colisión
    //private void OnCollisionEnter2D(Collision2D colision)
    //{

    //    if (colision.gameObject.CompareTag("Player"))
    //    {

    //        ContactPoint2D contacto = colision.GetContact(0);


    //        if (contacto.normal.y <= -0.5f)
    //        {


    //            StartCoroutine(DestruirEnemigo());


    //        }
    //    }
    //}
}